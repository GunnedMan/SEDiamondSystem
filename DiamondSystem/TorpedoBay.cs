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
        public class TorpedoBay : ISubsystem
        {
            public enum TorpedoBayState
            {
                Idle,
                Empty,
                Reloading,
                ReadyToLaunch,
                Launching,
                Damaged
            }

            Program program;

            TorpedoBayState state;

            List<IMyShipWelder> welders = new List<IMyShipWelder>();
            List<IMyProjector> projectors = new List<IMyProjector>();
            IMyProjector activeProjector;
            IMyCargoContainer iceContainer;
            MyInventoryItem ice;

            Torpedo torpedoInBay;
            public bool IsOperational
            {
                get
                {
                    return state != TorpedoBayState.Damaged;
                }
            }


            public TorpedoBay(string _tag, Program _program)
            {
                program = _program;

                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block.CustomName.Contains(_tag));
                foreach (IMyTerminalBlock block in blocks)
                {
                    welders.Add(block as IMyShipWelder);
                    projectors.Add(block as IMyProjector);
                    if (block is IMyCargoContainer)
                    {
                        iceContainer = block as IMyCargoContainer;
                        //iceContainer.SetUseConveyorSystem(false);
                    }
                }
                if ((welders.Count > 0) && (projectors.Count > 0))
                {
                    activeProjector = projectors[0];
                    state = TorpedoBayState.Idle;
                }
                foreach (IMyShipWelder welder in welders)
                {
                    welder.Enabled = false;
                }
                foreach (IMyProjector projector in projectors)
                {
                    projector.Enabled = false;

                }
            }

            public void Update(TimeSpan currentTime)
            {
                //TODO переписать код на целостность торпедного аппарата

                if (iceContainer == null || !iceContainer.IsFunctional)
                {
                    state = TorpedoBayState.Damaged;
                }
                MyInventoryItem? _ice = iceContainer.GetInventory().FindItem(MyItemType.MakeOre("Ice"));


                foreach (IMyShipWelder welder in welders)
                {
                    if (welder == null || !welder.IsFunctional)
                    {
                        state = TorpedoBayState.Damaged;
                    }
                }

                foreach (IMyProjector projector in projectors)
                {
                    if (projector == null || !projector.IsFunctional)
                    {
                        state = TorpedoBayState.Damaged;
                    }
                }
                switch (state)
                {
                    case TorpedoBayState.Idle:
                        if (_ice.HasValue)
                        {
                            if (_ice?.Amount < 200)
                            {
                                state = TorpedoBayState.Empty;
                            }
                        }
                        else
                        {
                            state = TorpedoBayState.Empty;
                        }
                        break;

                    case TorpedoBayState.Empty:
                        if (_ice.HasValue)
                        {
                            if (_ice?.Amount > 200)
                            {
                                state = TorpedoBayState.Idle;
                            }
                        }
                        break;

                    case TorpedoBayState.Damaged:

                        break;

                    case TorpedoBayState.Reloading:
                        if (activeProjector.RemainingBlocks == 0)
                        {
                            foreach (IMyShipWelder welder in welders)
                            {
                                welder.Enabled = false;
                            }
                            torpedoInBay = new Torpedo(TORPEDO_TAG, activeProjector.CubeGrid, program);
                            if (!torpedoInBay.isDamaged)
                            {
                                if (iceContainer.GetInventory(0).TransferItemTo(torpedoInBay.gasGenerator.GetInventory(0), (MyInventoryItem)_ice, 200))
                                {
                                    state = TorpedoBayState.ReadyToLaunch;
                                }
                            }
                            else
                            {
                                state = TorpedoBayState.Damaged;
                            }
                        }
                        break;

                    case TorpedoBayState.ReadyToLaunch:

                        break;

                    case TorpedoBayState.Launching:
                        if (torpedoInBay.state != Torpedo.TorpedoState.Launch)
                        {
                            state = TorpedoBayState.Idle;
                            torpedoInBay = null;
                        }
                        break;

                }
            }

            public Torpedo Launch(TimeSpan _launchTime)
            {
                if (state == TorpedoBayState.ReadyToLaunch)
                {
                    torpedoInBay.Launch(_launchTime);
                    state = TorpedoBayState.Launching;
                    return torpedoInBay;
                }
                return null;
            }

            public bool Reload()
            {
                if (state == TorpedoBayState.Idle)
                {
                    foreach (IMyShipWelder welder in welders)
                    {
                        welder.Enabled = true;
                    }
                    activeProjector.Enabled = true;
                    return true;
                }
                return false;
            }

            public void CycleProjector()
            {
                if (state == TorpedoBayState.Idle)
                {
                    int activeProjectorNumber = +projectors.IndexOf(activeProjector);
                    if (activeProjectorNumber >= projectors.Count)
                    {
                        activeProjectorNumber = 0;
                    }
                    activeProjector = projectors[0];
                }
            }
        }
    }
}
