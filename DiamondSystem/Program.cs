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
        const double WORLD_MAX_SPEED = 1000;
        const double WORLD_MAX_SPEED_SQ = WORLD_MAX_SPEED * WORLD_MAX_SPEED;

        const string LIDAR_TAG = "<LIDAR>";
        const double LIDAR_MAX_DISTANCE_SQ = 6000 * 6000;

        const string TORPEDO_TAG = "<Torpedo>";

        //Timing variables
        public TimeSpan currentTime;


        //Lidar object
        Lidar mainLidar;

        






        public Program()
        {

            mainLidar = new Lidar(LIDAR_TAG, this);


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
