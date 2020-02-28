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
        const string PAYLOAD_TAG = "<PAYLOAD>";
        const double NAVIGATION_DELAY_AFTER_LAUNCH = 3; //seconds
        public class Torpedo : ISubsystem
        {

            public enum TorpedoState
            {
                None,
                InBay,
                Launch,
                Idle,
                Attack,
                Follow,
                Destructed
            }



            Program program;

            public TorpedoState state;

            public bool activatePayload = false;
            public bool isDamaged = false;

            List<IMyGyro> gyros = new List<IMyGyro>();
            List<IMyThrust> thrusters = new List<IMyThrust>();
            List<IMyWarhead> warheads = new List<IMyWarhead>();
            List<IMyFunctionalBlock> payloads = new List<IMyFunctionalBlock>();
            public IMyGasGenerator gasGenerator;
            


            ITarget target;
            TargetHoming targetHoming;

            TimeSpan launchTime = TimeSpan.Zero;
            TimeSpan lastUpdateTime = TimeSpan.Zero;

            Vector3D lastUpdatePosition = Vector3D.Zero;

            Vector3D velocity = Vector3D.Zero;

            public bool IsOperational
            {
                get
                {
                    return state != TorpedoState.Destructed;
                }
            }

            public Torpedo(string _tag, IMyCubeGrid _grid, Program _program)
            {
                program = _program;
                
                state = TorpedoState.InBay;

                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => (block.CustomName.Contains(_tag)) && (block.CubeGrid == _grid));
                foreach (IMyTerminalBlock block in blocks)
                {
                    if (block.CustomName.Contains(PAYLOAD_TAG))
                    {
                        payloads.Add(block as IMyFunctionalBlock);
                        continue;
                    }
                    gyros.Add(block as IMyGyro);
                    thrusters.Add(block as IMyThrust);
                    gasGenerator = block as IMyGasGenerator;
                    warheads.Add(block as IMyWarhead);
                }
                if ((gyros.Count > 0) && (thrusters.Count > 0) && (gasGenerator != null))
                {
                    gyros.ForEach(g => { g.Enabled = false; g.GyroOverride = true; g.Pitch = 0; g.Yaw = 0; g.Roll = 0; });
                    thrusters.ForEach(t => t.Enabled = false);
                    warheads.ForEach(w => w.IsArmed = false);
                    gasGenerator.Enabled = false;
                    gasGenerator.UseConveyorSystem = false;
                }
                else
                {
                    isDamaged = true;
                }

            }

            public void Launch(TimeSpan _currentTime)
            {
                if (state != TorpedoState.InBay)
                {
                    return;
                }
                lastUpdateTime = _currentTime;
                lastUpdatePosition = gyros[0].GetPosition();
                launchTime = _currentTime;
                gyros.ForEach(g => g.Enabled = true);
                thrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1; });
                gasGenerator.Enabled = true;


            }

            public void Update(TimeSpan _currentTime)
            {
                if (state == TorpedoState.Destructed)
                {
                    return;
                }
                gyros = gyros.Where<IMyGyro>(g => ( g != null && g.IsFunctional)) as List<IMyGyro>;
                thrusters = thrusters.Where<IMyThrust>(t => (t != null && t.IsFunctional)) as List<IMyThrust>;

                if (gasGenerator == null || !gasGenerator.IsWorking || gyros.Count == 0 || thrusters.Count == 0)
                {
                    state = TorpedoState.Destructed;
                }

                if (_currentTime != lastUpdateTime)
                {
                    velocity = (gyros[0].GetPosition() - lastUpdatePosition) / (_currentTime.TotalSeconds - lastUpdateTime.Seconds);
                }

                switch (state)
                {
                    case TorpedoState.InBay:

                        break;
                    case TorpedoState.Launch:
                        if ((_currentTime - launchTime).TotalSeconds >= NAVIGATION_DELAY_AFTER_LAUNCH)
                        {
                            if (target != null)
                            {
                                if (targetHoming.direction == TargetHoming.Direction.Follow)
                                {
                                    state = TorpedoState.Follow;
                                }
                                else
                                {
                                    state = TorpedoState.Attack;
                                }
                                thrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1; });
                            }
                            else
                            {
                                state = TorpedoState.Idle;
                                thrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 0; });
                            }
                        }
                        break;
                    case TorpedoState.Idle:

                        break;
                    case TorpedoState.Follow:

                        break;
                    case TorpedoState.Attack:
                        {
                            Vector3D additionVector = Vector3D.Zero;
                            Vector3D vectorToTarget = target.Position - gyros[0].GetPosition();
                            Vector3D targetForwardDirection = target.EntityInfo.Orientation.Forward;
                            switch (targetHoming.direction)
                            {
                                case TargetHoming.Direction.Side:
                                    additionVector = - Vector3D.ProjectOnPlane(ref vectorToTarget, ref targetForwardDirection);
                                    break;
                                case TargetHoming.Direction.Lateral:
                                    additionVector = - targetForwardDirection;
                                    break;
                                case TargetHoming.Direction.Frontal:
                                    additionVector = targetForwardDirection;
                                    break;
                            }
                            //TODO homing!
                            Vector3D result = Vector3D.Normalize(vectorToTarget) * WORLD_MAX_SPEED - velocity + target.Velocity;
                            Vector3 homingVector = CalculateHoming(gyros[0].WorldMatrix, target.Position);
                            gyros.ForEach(g => { g.Pitch = homingVector.X; g.Yaw = homingVector.Y; });
                        }
                        break;
                }
                return;
            }

            public void SetTarget(ITarget _target, TargetHoming _homing = new TargetHoming())
            {
                if(_target == null)
                {
                    return;
                }
                target = _target;
                targetHoming = _homing;
            }

            public void SelfDestruct()
            {
                if (state == TorpedoState.Attack || state == TorpedoState.Follow || state == TorpedoState.Idle)
                {
                    warheads.ForEach(W => W.Detonate());
                    state = TorpedoState.Destructed;
                }
            }


        }
    }
}
