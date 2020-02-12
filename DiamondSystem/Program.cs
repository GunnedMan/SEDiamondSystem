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
    partial class Program : MyGridProgram
    {
        const string LIDAR_TAG = "<LIDAR>";
        const double LIDAR_MAX_DISTANCE_SQ = 6000^2;

        //Timing variables
        TimeSpan currentTime;


        //Lidar object
        Lidar mainLidar;


        public class Lidar
        {
            const string HINGE_AZIMUTH_TAG = "<AZ>";
            const string HINGE_ELEVATION_TAG = "<EL>";

            static Program program;

            List<IMyCameraBlock> cameras; //cameras array
            IMyMotorStator hingeAzimuth;
            IMyMotorStator hingeElevation;
            bool isDamaged;

            public static void Init(Program _program)
            {
                program = _program;
            }
            public Lidar(string _tag)
            {
                isDamaged = false;

                List<IMyMotorStator> motorStatorBlocks = new List<IMyMotorStator>();
                IMyBlockGroup lidarBlocks = program.GridTerminalSystem.GetBlockGroupWithName(_tag);
                lidarBlocks.GetBlocksOfType<IMyCameraBlock>(cameras);
                foreach (IMyCameraBlock camera in cameras)
                {
                    camera.EnableRaycast = true;
                }

                lidarBlocks.GetBlocksOfType<IMyMotorStator>(motorStatorBlocks, block => block.CustomName.Contains(HINGE_AZIMUTH_TAG));
                hingeAzimuth = motorStatorBlocks[0];
                motorStatorBlocks.Clear();
                lidarBlocks.GetBlocksOfType<IMyMotorStator>(motorStatorBlocks, block => block.CustomName.Contains(HINGE_ELEVATION_TAG));
                hingeElevation = motorStatorBlocks[0];
            }
            public MyDetectedEntityInfo Scan(Vector3D point)
            {
                MyDetectedEntityInfo foundTarget = new MyDetectedEntityInfo();
                foreach (IMyCameraBlock camera in cameras)
                {
                    if(camera!=null && camera.IsFunctional)
                    {
                        if(Vector3D.DistanceSquared(camera.GetPosition(), point) <= LIDAR_MAX_DISTANCE_SQ)
                        {
                            if(camera.CanScan(point))
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
        
        public class Target
        {
            //public enum TargetType
            //{
            //    LargeShip,
            //    SmallShip,
            //    Station,
            //    Asteroid,
            //    Wreckage,
            //    Torpedo
            //}

            public MyDetectedEntityInfo entityInfo;
            public TimeSpan scanTime = TimeSpan.Zero;
            //public TargetType type;


            public Target(MyDetectedEntityInfo _entityInfo, TimeSpan _time)
            {
                entityInfo = _entityInfo;
                scanTime = _time;
                //switch (_entityInfo.Type)
                //{
                //    case MyDetectedEntityType.LargeGrid:
                //        type = TargetType.LargeShip;
                //        break;
                //    case MyDetectedEntityType.SmallGrid:
                //        type = TargetType.SmallShip;
                //        break;
                //    case MyDetectedEntityType.Asteroid:
                //        type = TargetType.Asteroid;
                //        break;
                //}
            }

            public Vector3D Predict(TimeSpan _time)
            {
                if(scanTime == TimeSpan.Zero)
                {
                    return entityInfo.Position;

                }
                else
                {
                    return entityInfo.Position + entityInfo.Velocity * ((float)_time.TotalSeconds - (float)scanTime.TotalSeconds);
                }
            }
        }

        public static class Homing
        {
            public static Vector3D Calculate(MatrixD _origin, Vector3D _target)
            {
                Vector3D result = Vector3D.Zero;

                _target = Vector3D.Normalize(_target);

                double dotUp = _origin.Up.Dot(_target);
                double dotRight = _origin.Right.Dot(_target);
                double dotForward = _origin.Forward.Dot(_target);

                //тангаж
                if (dotForward > 0)
                { result.X = dotUp; }
                else if (dotUp > 0)
                { result.X = 1; }
                else
                { result.X = -1; }
                //рысканье
                if (dotForward > 0)
                { result.Y = dotRight; }
                else if (dotRight > 0)
                { result.Y = 1; }
                else
                { result.Y = -1; }
                /*
                //крен
                if (dotUp > -0.5)
                {
                    result.Z = dotLeft;
                }
                else if (dotLeft > 0)
                {
                    result.Z = 1;
                }
                else
                {
                    result.Z = -1;
                }
                */
                return result;
            }
        }

        public class CommandSeatControl
        {
            const double keyFilterTime = 100; //milliseconds

            public enum key
            {
                w = 1,
                a = 2,
                s = 4,
                d = 8,
                space = 16,
                c = 32,
                q = 64,
                e = 128
            }

            static Program program;

            IMyCockpit cockpit;
            TimeSpan lastUpdateTime = TimeSpan.Zero;
            public key keysPressing;
            public key keysPressed;
            
            public static void Init(Program _program)
            {
                program = _program;
            }

            CommandSeatControl(string _tag)
            {
                List<IMyCockpit> cockpits = new List<IMyCockpit>();
                program.GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, C => C.CustomName.Contains(_tag));
                if(cockpits.Count >= 0)
                {
                    cockpit = cockpits[0];
                }
            }

            public bool IsNull()
            {
                if(cockpit != null)
                {return false;}
                else
                {return true;}
            }

            public bool Update(TimeSpan _timestamp)
            {
                if (cockpit != null && cockpit.IsFunctional)
                {
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Program()
        {
            Lidar.Init(this);
            mainLidar = new Lidar(LIDAR_TAG);

            CommandSeatControl.Init(this);

            Echo("Hello world!");
            
            
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            //TIMINGS
            currentTime += Runtime.TimeSinceLastRun;
            //GETTING CONTROL INPUTS
            
            
        }
    }
}
