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
        public const int TARGET_IS_MISSED_TIME = 2; //seconds

        public struct TargetHoming
        {
            public enum Direction : byte
            {
                None = 0,
                Side = 1,
                Frontal = 2,
                Lateral = 3,
                Follow = 4,
            }

            public Direction direction;
            public double quarter;

            public TargetHoming(Direction _direction = Direction.None, double _quarter = 0.0)
            {
                direction = _direction;
                quarter = _quarter;
            }
        }



        public class Target : ITarget
        {
            public MyDetectedEntityInfo EntityInfo
            {
                get
                {
                    return EntityInfo;
                }
                set
                {
                    EntityInfo = value;
                }
            }
            public TimeSpan LastEntityUpdateTime = TimeSpan.Zero;
            public Vector3D Position
            {
                get
                {
                    return Position;
                }
                private set
                {
                    Position = value;
                }

            }
            public Vector3 Velocity
            {
                get
                {
                    return Velocity;
                }
                private set
                {
                    Velocity = value;
                }
            }

            public bool IsMoveable
            {
                private set { IsMoveable = value; }
                get { return IsMoveable; }
            }

            public bool IsTracked;

            public bool IsMissed
            {
                get
                {
                    return (LastEntityUpdateTime.Seconds >= TARGET_IS_MISSED_TIME);
                }
            }

            public Target(MyDetectedEntityInfo _entityInfo, TimeSpan _time)
            {
                EntityInfo = _entityInfo;
                LastEntityUpdateTime = _time;
                Position = EntityInfo.Position;
                Velocity = EntityInfo.Velocity;
                if (EntityInfo.Type == MyDetectedEntityType.Asteroid || EntityInfo.Type == MyDetectedEntityType.Planet)
                {
                    IsMoveable = false;
                }


            }


            public static bool operator ==(Target T1, Target T2)
            {
                if(T1.EntityInfo.EntityId == T2.EntityInfo.EntityId)
                { return true; }
                return false;
            }

            public static bool operator !=(Target T1, Target T2)
            {
                if (T1.EntityInfo.EntityId != T2.EntityInfo.EntityId)
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
            public void Update(TimeSpan _currentTime)
            {
                if(EntityInfo.IsEmpty() && !IsMoveable)
                { return; }
                Position = EntityInfo.Position + EntityInfo.Velocity * (float)(_currentTime.TotalSeconds - LastEntityUpdateTime.TotalSeconds);
            }

            public void UpdateEntity(TimeSpan _currentTime, MyDetectedEntityInfo detectedEntityInfo)
            {
                if(detectedEntityInfo.IsEmpty() && detectedEntityInfo.EntityId != EntityInfo.EntityId)
                { return; }
                EntityInfo = EntityInfo;
                LastEntityUpdateTime = _currentTime;
                Position = EntityInfo.Position;
                Velocity = EntityInfo.Velocity;
            }
        }
    }
}
