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
