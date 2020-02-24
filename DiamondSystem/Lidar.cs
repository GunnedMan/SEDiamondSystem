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
        public class Lidar : ISubsystem
        {
            public enum LidarState
            {
                None,
                Idle,
                Manual,
                Auto,
                Damaged,
                Inoperable
            }

            const string HINGE_AZIMUTH_TAG = "<AZ>";
            const string HINGE_ELEVATION_TAG = "<EL>";

            Program program;
            

            IMyCameraBlock mainCamera;
            List<IMyCameraBlock> cameras; //cameras array
            public IMyMotorStator hingeAzimuth;
            public IMyMotorStator hingeElevation;
            public bool isDamaged;
            public LidarState state;
            public bool IsOperational
            {
                get
                {
                    return state != LidarState.Inoperable;
                }
            }

            public Lidar(string _tag, Program _program)
            {
                program = _program;
                isDamaged = false;
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                List<IMyTerminalBlock> blocksTemp = new List<IMyTerminalBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block.CustomName.Contains(_tag));
                //main camera
                blocksTemp = blocks.Where<IMyTerminalBlock>(block => (block is IMyCameraBlock)) as List<IMyTerminalBlock>;
                if (blocksTemp.Count == 0)
                {
                    state = LidarState.Inoperable;
                    return;
                }
                mainCamera = blocksTemp[0] as IMyCameraBlock;
                //Azimuth hinge
                blocksTemp = blocks.Where<IMyTerminalBlock>(block => (block is IMyMotorStator && block.CustomName.Contains(HINGE_AZIMUTH_TAG))) as List<IMyTerminalBlock>;
                if (blocksTemp.Count == 0)
                {

                }
                else
                {
                    hingeAzimuth = blocksTemp[0] as IMyMotorStator;
                }
                //Elevation hinge
                blocksTemp = blocks.Where<IMyTerminalBlock>(block => (block is IMyMotorStator && block.CustomName.Contains(HINGE_ELEVATION_TAG))) as List<IMyTerminalBlock>;
                if (blocksTemp.Count == 0)
                {
                    
                }
                else
                {
                    hingeElevation = blocksTemp[0] as IMyMotorStator;
                }
                //Cameras array
                program.GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(cameras, camera => (camera.CubeGrid == mainCamera.CubeGrid && camera.Orientation == mainCamera.Orientation));
                if (cameras.Count == 0)
                {

                }
                else
                {
                    cameras.ForEach(camera => camera.EnableRaycast = true);
                }
            }

            public void Update(TimeSpan currentTime)
            {

            }

            public MyDetectedEntityInfo Scan(Vector3D point)
            {
                MyDetectedEntityInfo foundTarget = new MyDetectedEntityInfo();
                foreach (IMyCameraBlock camera in cameras)
                {
                    if (camera != null && camera.IsFunctional)
                    {
                        if (Vector3D.DistanceSquared(camera.GetPosition(), point) <= LIDAR_MAX_DISTANCE_SQ)
                        {
                            if (camera.CanScan(point))
                            {
                                foundTarget = camera.Raycast(point);

                                break;
                            }
                        }
                    }
                    else
                    {
                        isDamaged = true;
                    }

                }
                return foundTarget;
            }

        }
    }
}
