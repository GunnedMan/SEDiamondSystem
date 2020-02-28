using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public const int LIDAR_MAX_TRACKING_TARGETS = 4;

        public class Lidar : ISubsystem
        {
            public enum LidarState
            {
                None,
                Idle,
                Manual,
                Auto,
                Inoperable
            }


            const string HINGE_AZIMUTH_TAG = "<AZ>";
            const string HINGE_ELEVATION_TAG = "<EL>";
            const float LIDAR_IDLE_ANGLE_DELTA = 0.05f; //rad
            const float LIDAR_HINGE_SENSIVITY = 0.01f;

            Program program;
            

            IMyCameraBlock MainCamera;
            List<IMyCameraBlock> Cameras = new List<IMyCameraBlock>(); //cameras array
            public IMyMotorStator HingeAzimuth;
            public IMyMotorStator HingeElevation;
            public bool IsDamaged;
            public LidarState State;
            bool IsFixed = true;
            public bool IsArray = false;


            public Vector3 HingeControlManual; //x - elevation, y - azimuth
            private Vector3 HingeControlResult; //x - elevation, y - azimuth
            private Vector3D StabilizationVector;
            public bool Stabilization
            {
                set
                {
                    if (value && !IsFixed)
                    { StabilizationVector = MainCamera.WorldMatrix.Forward; }
                    Stabilization = value;
                }
                get
                {
                    return Stabilization;
                }
            }
            public bool IsOperational
            {
                get
                {
                    return State != LidarState.Inoperable;
                }
            }
            public Vector3D Direction
            {
                get
                {
                    if (State != LidarState.Inoperable)
                    { return MainCamera.WorldMatrix.Forward; }
                    return Vector3D.Forward;
                }
            }

            public Lidar(string _tag, Program _program)
            {
                program = _program;
                IsDamaged = false;
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                List<IMyTerminalBlock> blocksTemp = new List<IMyTerminalBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block.CustomName.Contains(_tag));
                //main camera
                blocksTemp = blocks.Where<IMyTerminalBlock>(block => (block is IMyCameraBlock)) as List<IMyTerminalBlock>;
                if (blocksTemp.Count == 0)
                {
                    State = LidarState.Inoperable;
                    return;
                }
                MainCamera = blocksTemp[0] as IMyCameraBlock;
                //Azimuth hinge
                blocksTemp = blocks.Where<IMyTerminalBlock>(block => (block is IMyMotorStator && block.CustomName.Contains(HINGE_AZIMUTH_TAG))) as List<IMyTerminalBlock>;
                if (blocksTemp.Count == 0)
                {
                    IsFixed = true;
                }
                else
                {
                    HingeAzimuth = blocksTemp[0] as IMyMotorStator;
                    IsFixed = false;
                }
                //Elevation hinge
                blocksTemp = blocks.Where<IMyTerminalBlock>(block => (block is IMyMotorStator && block.CustomName.Contains(HINGE_ELEVATION_TAG))) as List<IMyTerminalBlock>;
                if (blocksTemp.Count == 0)
                {
                    IsFixed = true;
                }
                else
                {
                    HingeElevation = blocksTemp[0] as IMyMotorStator;
                    IsFixed = false;
                }
                //Cameras array
                program.GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(Cameras, camera => (camera.CubeGrid == MainCamera.CubeGrid && camera.Orientation == MainCamera.Orientation));
                if (Cameras.Count == 0)
                {
                    IsArray = false;
                }
                else
                {
                    IsArray = true;
                    Cameras.ForEach(camera => camera.EnableRaycast = true);
                }
                State = LidarState.Idle;
            }

            public void Update(TimeSpan currentTime)
            {
                if (!IsFixed)
                {
                    if (HingeAzimuth == null || !HingeAzimuth.IsFunctional || HingeElevation == null || !HingeElevation.IsFunctional)
                    {
                        State = LidarState.Inoperable;
                    }
                }
                if (MainCamera == null || !MainCamera.IsFunctional)
                {
                    State = LidarState.Inoperable;
                }

                switch (State)
                {
                    case LidarState.None:
                        State = LidarState.Inoperable;
                        break;

                    case LidarState.Idle:
                        //TODO
                        //if(HingeAzimuth.Angle <= LIDAR_IDLE_ANGLE_DELTA)
                        //{
                        //
                        //}
                        break;

                    case LidarState.Manual:
                        if(!IsFixed)
                        {
                            if(!Stabilization)
                            {
                                StabilizationVector = MainCamera.WorldMatrix.Forward;
                            }
                            StabilizationVector += (MainCamera.WorldMatrix.Up * HingeControlManual.X + MainCamera.WorldMatrix.Right * HingeControlManual.Y) * LIDAR_HINGE_SENSIVITY;
                            StabilizationVector.Normalize();
                            HingeControlResult = CalculateHoming(MainCamera.WorldMatrix, MainCamera.GetPosition() + StabilizationVector);
                            HingeAzimuth.TargetVelocityRad = HingeControlResult.Y * LIDAR_HINGE_SENSIVITY;
                            HingeElevation.TargetVelocityRad = HingeControlResult.X * LIDAR_HINGE_SENSIVITY;
                        }
                        break;

                    case LidarState.Auto:

                        break;

                    case LidarState.Inoperable:

                        break;
                }


            }

            public void ScanTarget(TimeSpan currentTime, ITarget target)
            {
                target.UpdateEntity(currentTime, ScanPoint(target.Position));
            }
            public Target LockTarget(TimeSpan currentTime, Vector3D point)
            {
                MyDetectedEntityInfo entity = ScanPoint(point);
                if(entity.IsEmpty())
                {
                    return null;
                }
                return new Target(ScanPoint(point), currentTime);
            }
            MyDetectedEntityInfo ScanPoint(Vector3D point)
            {
                MyDetectedEntityInfo foundEntity = new MyDetectedEntityInfo();
                foreach (IMyCameraBlock camera in Cameras)
                {
                    if (camera != null && camera.IsFunctional)
                    {
                        if (Vector3D.DistanceSquared(camera.GetPosition(), point) <= LIDAR_MAX_DISTANCE_SQ)
                        {
                            if (camera.CanScan(point))
                            {
                                foundEntity = camera.Raycast(point);

                                break;
                            }
                        }
                    }
                    else
                    {
                        IsDamaged = true;
                    }

                }
                return foundEntity;
            }

        }
    }
}
