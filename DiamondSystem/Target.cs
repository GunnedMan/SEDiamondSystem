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


        public struct TargetHoming
        {
            public enum Direction : byte
            {
                None = 0,
                Side = 1,
                Frontal = 2,
                Lateral = 3
            }

            public Direction direction;
            public double quarter;

            public TargetHoming(Direction _direction = Direction.None, double _quarter = 0.0)
            {
                direction = _direction;
                quarter = _quarter;
            }
        }


        public interface ITarget
        {
            MyDetectedEntityInfo entityInfo { get; }
            Vector3D position { get; }
            Vector3 velocity { get; }

        }

        public class Target : ITarget
        {
            public MyDetectedEntityInfo entityInfo
            {
                get
                {
                    return entityInfo;
                }
                set
                {
                    entityInfo = value;
                }
            }
            public TimeSpan scanTime = TimeSpan.Zero;
            public Vector3D position
            {
                get
                {
                    return position;
                }
                private set
                {
                    position = value;
                }

            }
            public Vector3 velocity
            {
                get
                {
                    return velocity;
                }
                private set
                {
                    velocity = value;
                }
            }

            public Target(MyDetectedEntityInfo _entityInfo, TimeSpan _time)
            {
                entityInfo = _entityInfo;
                scanTime = _time;
                position = entityInfo.Position;
                velocity = entityInfo.Velocity;
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

            /*
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
            */
            public void Update()
            {

            }
        }
    }
}
