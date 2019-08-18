using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Drawing;

namespace LemonMissionPack.Missions
{
    /// <summary>
    /// Mission 1: The player picks up Lester's friend from LSIA.
    /// </summary>
    public class Mission01 : Script
    {
        /// <summary>
        /// If the player has been notified about the mission.
        /// </summary>
        public static bool IsPlayerNotified { get; private set; } = false;
        /// <summary>
        /// If the mission is currently in progress.
        /// </summary>
        public static bool IsInProgress { get; private set; } = false;
        /// <summary>
        /// The blip used to mark the mission location.
        /// </summary>
        public static Blip MissionBlip { get; private set; }
        /// <summary>
        /// The location of the blip.
        /// </summary>
        private static readonly Vector3 BlipLocation = new Vector3(-1037, -2736, 20);
        /// <summary>
        /// The location for the pick up.
        /// </summary>
        private static readonly Vector3 PickUpLocation = new Vector3(-1032, -2730, 19f);
        /// <summary>
        /// The ped that we need to carry from the airport to Lester's house.
        /// </summary>
        private static Ped Objective { get; set; }

        public Mission01()
        {
            // Add our events
            Tick += OnTickBasics;
            Tick += OnTickMission;
            Aborted += OnAbort;
        }

        private void OnTickBasics(object sender, EventArgs e)
        {
            // If the user has not been notified, the base content is loaded, the game is not loading, the player is Franklin and the player can be controlled
            if (!IsInProgress && !IsPlayerNotified && Manager.IsContentLoaded && !Game.IsLoading && (uint)Game.Player.Character.Model.Hash == (uint)PedHash.Franklin && Game.Player.CanControlCharacter)
            {
                // Notify the user
                // TODO: Make the message looks like is coming from Lester
                UI.Notify(Manager.Strings["M01_SMS"], true);
                IsPlayerNotified = true;
                return;
            }

            // If the blip does not exists and the player is Franklin
            if (!IsInProgress && MissionBlip == null && (uint)Game.Player.Character.Model.Hash == (uint)PedHash.Franklin)
            {
                // Create the mission blip
                MissionBlip = World.CreateBlip(BlipLocation);
                MissionBlip.IsShortRange = false;
                MissionBlip.Sprite = BlipSprite.Lester;
                MissionBlip.Color = BlipColor.Yellow;
                MissionBlip.Name = "Lester (LMP)";
                return;
            }

            // If the blip exists but the character is not Franklin
            if (MissionBlip != null && MissionBlip.Exists() && MissionBlip.Sprite == BlipSprite.Lester && ((uint)Game.Player.Character.Model.Hash != (uint)PedHash.Franklin || IsInProgress))
            {
                // Destroy the blip
                MissionBlip.Remove();
                MissionBlip = null;
                return;
            }
        }

        private void OnTickMission(object sender, EventArgs e)
        {
            // If the player gets too close from the mission start, there is a blip and is the 2nd type of yellow
            if (BlipLocation.DistanceTo(Game.Player.Character.Position) < 500 && MissionBlip != null && MissionBlip.Color == BlipColor.Yellow)
            {
                // Mark the mission as in-progress
                IsInProgress = true;
                // Remove the existing blip
                MissionBlip.Remove();
                // Create a new one
                MissionBlip = World.CreateBlip(PickUpLocation);
                MissionBlip.Color = BlipColor.Yellow2;
                MissionBlip.ShowRoute = true;
                MissionBlip.Name = "Terminal 4";
                // And notify the player
                UI.ShowSubtitle(Manager.Strings["M01_SUB01"], 5000);

                // Then, proceed to create the ped
                Objective = World.CreatePed(new Model(PedHash.FreemodeMale01), new Vector3(-1033.15f, -2739.35f, 20.17f), 14.87f);
                Objective.AddBlip();
                Objective.CurrentBlip.Color = BlipColor.Blue;
                Objective.IsEnemy = false;
                Objective.IsInvincible = true;
            }

            // If there is a blip and is the 2nd type of yellow
            if (MissionBlip != null && MissionBlip.Color == BlipColor.Yellow2)
            {
                // Draw a little marker to show where the player needs to stop
                World.DrawMarker(MarkerType.VerticalCylinder, PickUpLocation, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.Yellow);

                // If the player is on a vehicle and the character is not on said vehicle
                if (Game.Player.Character.CurrentVehicle != null && !Objective.IsInVehicle(Game.Player.Character.CurrentVehicle))
                {
                    // If the player is near the pickup location
                    if (PickUpLocation.DistanceTo(Game.Player.Character.Position) <= 2)
                    {
                        // Freeze the vehicle
                        Game.Player.Character.CurrentVehicle.FreezePosition = true;

                        // If the player is pressing the horn
                        if (Game.IsEnabledControlJustPressed(0, Control.VehicleHorn))
                        {
                            // Tell the ped to enter the vehicle
                            Function.Call(Hash.TASK_ENTER_VEHICLE, Objective, Game.Player.Character.CurrentVehicle, 20000, 0, 2f, 1, 0);

                            // While the objective is entering the vehicle
                            while (!Objective.IsInVehicle(Game.Player.Character.CurrentVehicle))
                            {
                                // Wait
                                Yield();
                            }
                            // Once the objective has entered the vehicle, unfreeze it
                            Game.Player.Character.CurrentVehicle.FreezePosition = false;

                            // And show some dialog
                            UI.ShowSubtitle(Manager.Strings["M01_SUB02"], 4000);
                            Wait(4000);
                            UI.ShowSubtitle(Manager.Strings["M01_SUB03"], 4000);
                            Wait(4000);
                            UI.ShowSubtitle(Manager.Strings["M01_SUB04"], 4000);
                            Wait(4000);
                        }
                    }
                }
            }
        }

        private void OnAbort(object sender, EventArgs e)
        {
            // If there is a blip present
            if (MissionBlip != null && MissionBlip.Exists())
            {
                // Destroy it
                MissionBlip.Remove();
                MissionBlip = null;
            }

            // If there is a ped objective present
            if (Objective != null && Objective.Exists())
            {
                // Also destroy it
                Objective.Kill();
                Objective.MarkAsNoLongerNeeded();
                Objective.Delete();
                Objective = null;
            }
        }
    }
}
