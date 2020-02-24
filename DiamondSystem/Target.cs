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
            public TimeSpan lastScanTime = TimeSpan.Zero;
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

            public bool IsTracking
            {
                get
                {
                    if (entityInfo.Type == MyDetectedEntityType.Asteroid || entityInfo.Type == MyDetectedEntityType.Planet)
                    {
                        return false;
                    }
                    return IsTracking;
                }
                set
                {
                    IsTracking = value;
                }
            }

            public Target(MyDetectedEntityInfo _entityInfo, TimeSpan _time)
            {
                entityInfo = _entityInfo;
                lastScanTime = _time;
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
            public void Update(TimeSpan _currentTime)
            {
                position = entityInfo.Position + entityInfo.Velocity * (float)(_currentTime.TotalSeconds - lastScanTime.TotalSeconds);
            }

            public void UpdateEntity(TimeSpan _currentTime, MyDetectedEntityInfo detectedEntityInfo)
            {
                entityInfo = entityInfo;
                lastScanTime = _currentTime;
                position = entityInfo.Position;
                velocity = entityInfo.Velocity;
            }
        }
    }
}
