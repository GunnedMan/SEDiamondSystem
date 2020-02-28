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

        const int PROGRAM_MAX_TARGETS = 4;
        const int PROGRAM_MAX_TORPEDOES = 8;

        const string LIDAR_TAG = "<Lidar>";
        const double LIDAR_MAX_DISTANCE = 6000;
        const double LIDAR_MAX_DISTANCE_SQ = LIDAR_MAX_DISTANCE * LIDAR_MAX_DISTANCE;

        const string TORPEDO_TAG = "<Torpedo>";

        const string TORPEDO_BAY_TAG = "<T_Bay>";

        const string FIRE_POST_TAG = "<Fire_Post>"

        //Timing variables
        public TimeSpan currentTime;


        //Lidar object
        Lidar mainLidar;

        List<Target> Targets = new List<Target>(PROGRAM_MAX_TARGETS);

        List<Torpedo> Torpedoes = new List<Torpedo>(PROGRAM_MAX_TORPEDOES);

        List<TorpedoBay> TorpedoBays = new List<TorpedoBay>();

        CommandSeatControl FirePost;





        public Program()
        {

            mainLidar = new Lidar(LIDAR_TAG, this);
            TorpedoBays.Add(new TorpedoBay(TORPEDO_BAY_TAG, this));
            FirePost = new CommandSeatControl(FIRE_POST_TAG, this);


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
            if(FirePost.IsKeyPressed(CommandSeatControl.Key.s))
            {
                TorpedoBays.ForEach(tb => tb.Reload());
            }
            if (FirePost.IsKeyPressed(CommandSeatControl.Key.w))
            {
                TorpedoBays.ForEach(tb => 
                {
                    Torpedo torpedo = tb.Launch(currentTime);
                    if(torpedo != null)
                    {
                        Torpedoes.Add(torpedo);
                    }
                });
            }
        }
    }
}
