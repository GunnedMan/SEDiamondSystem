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
        public class CommandSeatControl
        {
            const double keyFilterTime = 100; //milliseconds

            public enum Key
            {
                w = 1,
                a = 2,
                s = 4,
                d = 8,
                space = 16,
                c = 32,
                q = 64,
                e = 128,
                up = 256,
                down = 512,
                left = 1024,
                right = 2048
            }

            static Program program;

            IMyCockpit cockpit;
            TimeSpan lastUpdateTime = TimeSpan.Zero;
            public Key keysPressing;
            public Key keysPressed;
            private Key keysPressingLast;

            public static void Init(Program _program)
            {
                program = _program;
            }

            CommandSeatControl(string _tag)
            {
                List<IMyCockpit> cockpits = new List<IMyCockpit>();
                program.GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, C => C.CustomName.Contains(_tag));
                if (cockpits.Count >= 0)
                {
                    cockpit = cockpits[0];
                }
            }

            public bool IsNull()
            {
                if (cockpit != null)
                { return false; }
                else
                { return true; }
            }

            public bool IsKeyPressing(Key _key)
            {
                if ((keysPressing & _key) == _key)
                {
                    return true;
                }
                return false;
            }

            public bool IsKeyPressed(Key _key)
            {
                if ((keysPressed & _key) == _key)
                {
                    return true;
                }
                return false;
            }


            public bool Update(TimeSpan _timestamp)
            {
                keysPressingLast = keysPressing;

                if (cockpit != null && cockpit.IsFunctional)
                {
                    //////////////X-axis
                    if (cockpit.MoveIndicator.X == 0)
                    {
                        keysPressing &= ~(Key.a);
                        keysPressing &= ~(Key.d);
                    }
                    else if (cockpit.MoveIndicator.X > 0)
                    {
                        keysPressing ^= Key.d;
                        keysPressing &= ~(Key.a);
                    }
                    else
                    {
                        keysPressing &= ~(Key.d);
                        keysPressing ^= Key.a;
                    }
                    //////////////Y-axis
                    if (cockpit.MoveIndicator.Y == 0)
                    {
                        keysPressing &= ~(Key.w);
                        keysPressing &= ~(Key.s);
                    }
                    else if (cockpit.MoveIndicator.Y > 0)
                    {
                        keysPressing ^= Key.w;
                        keysPressing &= ~(Key.s);
                    }
                    else
                    {
                        keysPressing &= ~(Key.w);
                        keysPressing ^= Key.s;
                    }
                    //////////////Z-axis
                    if (cockpit.MoveIndicator.Z == 0)
                    {
                        keysPressing &= ~(Key.space);
                        keysPressing &= ~(Key.c);
                    }
                    else if (cockpit.MoveIndicator.Z > 0)
                    {
                        keysPressing ^= Key.space;
                        keysPressing &= ~(Key.c);
                    }
                    else
                    {
                        keysPressing &= ~(Key.space);
                        keysPressing ^= Key.c;
                    }
                    //////////////Roll-axis
                    if (cockpit.RollIndicator == 0)
                    {
                        keysPressing &= ~(Key.q);
                        keysPressing &= ~(Key.e);
                    }
                    else if (cockpit.RollIndicator > 0)
                    {
                        keysPressing ^= Key.e;
                        keysPressing &= ~(Key.q);
                    }
                    else
                    {
                        keysPressing &= ~(Key.e);
                        keysPressing ^= Key.q;
                    }
                    //////////////Pitch-axis
                    if (cockpit.RotationIndicator.X == 0)
                    {
                        keysPressing &= ~(Key.up);
                        keysPressing &= ~(Key.down);
                    }
                    else if (cockpit.RollIndicator > 0)
                    {
                        keysPressing ^= Key.up;
                        keysPressing &= ~(Key.down);
                    }
                    else
                    {
                        keysPressing &= ~(Key.up);
                        keysPressing ^= Key.down;
                    }
                    //////////////Yaw-axis
                    if (cockpit.RotationIndicator.Y == 0)
                    {
                        keysPressing &= ~(Key.right);
                        keysPressing &= ~(Key.left);
                    }
                    else if (cockpit.RollIndicator > 0)
                    {
                        keysPressing ^= Key.right;
                        keysPressing &= ~(Key.left);
                    }
                    else
                    {
                        keysPressing &= ~(Key.right);
                        keysPressing ^= Key.left;
                    }
                    keysPressed = (keysPressing ^ keysPressingLast) & keysPressing;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
