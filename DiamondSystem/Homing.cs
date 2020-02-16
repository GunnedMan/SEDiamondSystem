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
        public static Vector3D CalculateHoming(MatrixD _origin, Vector3D _target)
        {
            Vector3D result = Vector3D.Zero;

            _target = Vector3D.Normalize(_target);

            double dotUp = _origin.Up.Dot(_target);
            double dotRight = _origin.Right.Dot(_target);
            double dotForward = _origin.Forward.Dot(_target);

            //тангаж
            if (dotForward > 0)
            { result.X = dotUp; }
            else if (dotUp > 0)
            { result.X = 1; }
            else
            { result.X = -1; }
            //рысканье
            if (dotForward > 0)
            { result.Y = dotRight; }
            else if (dotRight > 0)
            { result.Y = 1; }
            else
            { result.Y = -1; }
            /*
            //крен
            if (dotUp > -0.5)
            {
                result.Z = dotLeft;
            }
            else if (dotLeft > 0)
            {
                result.Z = 1;
            }
            else
            {
                result.Z = -1;
            }
            */
            return result;
        }
    }
}
