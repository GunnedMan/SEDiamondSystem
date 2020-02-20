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


        struct TargetHoming
        {
            enum Direction : byte
            {
                None = 0,
                Side = 1,
                Frontal = 2,
                Lateral = 3
            }

            Direction direction;
            double quarter;
        }


        public class Target
        {
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


            public static bool operator ==(Target T1, Target T2)
            {
                if(T1.entityInfo.EntityId == T2.entityInfo.EntityId)
                { return true; }
                return false;
            }

            public static bool operator !=(Target T1, Target T2)
            {
                if (T1.entityInfo.EntityId != T2.entityInfo.EntityId)
                { return true; }
                return false;
            }


            public Vector3D Predict(TimeSpan _time)
            {
                if (scanTime == TimeSpan.Zero)
                {
                    return entityInfo.Position;

                }
                else
                {
                    return entityInfo.Position + entityInfo.Velocity * ((float)_time.TotalSeconds - (float)scanTime.TotalSeconds);
                }
            }
        }
    }
}
