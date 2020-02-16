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
    partial class Program : MyGridProgram
    {
        const string LIDAR_TAG = "<LIDAR>";
        const double LIDAR_MAX_DISTANCE_SQ = 6000 ^ 2;

        //Timing variables
        TimeSpan currentTime;


        //Lidar object
        Lidar mainLidar;

        






        public Program()
        {
            Lidar.Init(this);
            CommandSeatControl.Init(this);
            Torpedo.Init(this);

            mainLidar = new Lidar(LIDAR_TAG);


            Echo("Hello world!");


        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            //TIMINGS
            currentTime += Runtime.TimeSinceLastRun;
            //GETTING CONTROL INPUTS


        }
    }
}
