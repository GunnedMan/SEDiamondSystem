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
        public class Torpedo
        {
            public enum TorpedoState// : byte
            {
                InBay = 1,
                Launch = 2,
                Attack = 4,
                Follow = 8,
                ActivatePayload = 16,
                Reserved1 = 32,
                Reserved2 = 64,
                Destructed = 128
            }

            

            static Program program;

            public TorpedoState state;

            List<IMyGyro> Gyros = new List<IMyGyro>();
            List<IMyThrust> Thrusters = new List<IMyThrust>();
            List<IMyWarhead> Warheads = new List<IMyWarhead>();
            List<IMyFunctionalBlock> Payloads = new List<IMyFunctionalBlock>();

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


            //Target parameters
            long targetId;
            Vector3D targetPosition;
            Vector3 targetVelocity;


            public static void Init(Program _program)
            {
                program = _program;
            }

            public Torpedo(string _tag)
            {
                //TODO

            }

            public void Launch()
            {

            }

            public bool Update()
            {
                return;
            }

            public void SetTarget



        }
    }
}
