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
        public class Torpedo
        {
            public enum TorpedoState// : byte
            {
                InBay = 1,
                Launch = 2,
                Attack = 4,
                Follow = 8,
                ActivePayload = 16,
                Reserved1 = 32,
                Damaged = 64,
                Destructed = 128
            }

            public enum TorpedoState2// : byte
            {
                InBay,
                Launch,
                Attack,
                Follow,
                Destructed
            }



            static Program program;

            public TorpedoState state;

            List<IMyGyro> gyros = new List<IMyGyro>();
            List<IMyThrust> thrusters = new List<IMyThrust>();
            List<IMyWarhead> warheads = new List<IMyWarhead>();
            List<IMyFunctionalBlock> payloads = new List<IMyFunctionalBlock>();
            public IMyGasGenerator gasGenerator;

            ITarget target;
            TargetHoming targetHoming;

            public bool isInBay
            {
                get
                {
                    if ((state & TorpedoState.InBay) == TorpedoState.InBay)
                    { return true; }
                    return false;
                }
            }

            public bool isDestructed
            {
                get
                {
                    if ((state & TorpedoState.Destructed) == TorpedoState.Destructed)
                    { return true; }
                    return false;
                }
            }

            public static void Init(Program _program)
            {
                program = _program;
            }

            public Torpedo(string _tag, IMyCubeGrid _grid)
            {
                state = 0;
                state ^= TorpedoState.InBay;

                //TODO
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
                }
                else
                {
                    state ^= TorpedoState.Damaged;
                }

            }

            public void Launch()
            {
                if (state != TorpedoState.InBay)
                {
                    return;
                }
                gyros.ForEach(g => g.Enabled = true);
                thrusters.ForEach(t => { t.Enabled = true; t.ThrustOverridePercentage = 1; });
                gasGenerator.Enabled = true;


            }

            public bool Update()
            {
                switch (state)
                {
                    case: 
                }
                return true;
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



        }
    }
}
