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
        /// The location where the objective should be dropped.
        /// </summary>
        private static readonly Vector3 DestinationLocation = new Vector3(1305.3f, -1713, 53.5f);
        /// <summary>
        /// The ped that we need to carry from the airport to Lester's house.
        /// </summary>
        private static Ped Objective { get; set; }
        /// <summary>
        /// If the player has been notified that is out of the vehicle
        /// </summary>
        private static bool OutOfVehicle { get; set; } = false;

        public Mission01()
        {
            // Add our events
            Tick += OnTick;
            Aborted += OnAbort;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // If this mission has been completed
            if (Manager.Completion.Mission01)
            {
                // Remove the event and return
                Tick -= OnTick;
                return;
            }

            // If the mission is not in progress, the player has not been notified, the manager has the basic content loaded, the game is not loading, we are Franklin and we can control him
            if (!IsInProgress && !IsPlayerNotified && Manager.IsContentLoaded && !Game.IsLoading && (uint)Game.Player.Character.Model.Hash == (uint)PedHash.Franklin && Game.Player.CanControlCharacter)
            {
                // Notify the user
                // TODO: Make the message looks like is coming from Lester
                UI.Notify(Manager.Strings["M01_SMS"], true);
                IsPlayerNotified = true;
                return;
            }

            // If the mission is not in progress, there is no blip and we are playing as Franklin
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

            // If the blip exists, has a Lester icon but the player is not Franklin or the mission is in progress
            if (MissionBlip != null && MissionBlip.Exists() && MissionBlip.Sprite == BlipSprite.Lester && ((uint)Game.Player.Character.Model.Hash != (uint)PedHash.Franklin || IsInProgress))
            {
                // Destroy the blip
                MissionBlip.Remove();
                MissionBlip = null;
                return;
            }

            // If the player gets too close from the mission start, there is a blip and is the original Lester blip
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

                // If the player is on a vehicle, tell him to pick the objective
                if (Game.Player.Character.CurrentVehicle != null)
                {
                    UI.ShowSubtitle(Manager.Strings["M01_SUB01"], 5000);
                }
                // Otherwise, tell him to get a vehicle
                else
                {
                    UI.ShowSubtitle(Manager.Strings["M01_SUB07"], 5000);
                }

                // Then, proceed to create the ped
                Objective = World.CreatePed(new Model(PedHash.FreemodeMale01), new Vector3(-1033.15f, -2739.35f, 20.17f), 14.87f);
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

                        // If the player is pressing the horn and is on a vehicle
                        if (Game.IsEnabledControlJustPressed(0, Control.VehicleHorn) && Game.Player.Character.CurrentVehicle != null)
                        {
                            // Add a blip with basic information
                            Objective.AddBlip();
                            Objective.CurrentBlip.Color = BlipColor.Blue;
                            Objective.CurrentBlip.IsShortRange = true;

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
                            // Remove the blip of the objective
                            Objective.CurrentBlip.Remove();

                            // Destroy the existing blip and create a new one
                            MissionBlip.Remove();
                            MissionBlip = World.CreateBlip(DestinationLocation);
                            MissionBlip.Color = BlipColor.Yellow3;
                            MissionBlip.ShowRoute = true;
                            MissionBlip.Name = "Amarillo Vista";

                            // And show some dialog
                            UI.ShowSubtitle(Manager.Strings["M01_SUB02"], 4000);
                            Wait(4000);
                            UI.ShowSubtitle(Manager.Strings["M01_SUB03"], 4000);
                            Wait(4000);
                            UI.ShowSubtitle(Manager.Strings["M01_SUB04"], 4000);
                            Wait(4000);
                            UI.ShowSubtitle(Manager.Strings["M01_SUB05"], 4000);
                        }
                    }
                }
            }

            // If there is no mission blip, there is an objective with a vehicle and that vehicle is the same as the player
            if (MissionBlip == null && Objective != null && Objective.CurrentVehicle != null && Game.Player.Character.CurrentVehicle == Objective.CurrentVehicle)
            {
                // Restore the old blip
                MissionBlip = World.CreateBlip(DestinationLocation);
                MissionBlip.Color = BlipColor.Yellow3;
                MissionBlip.ShowRoute = true;
                MissionBlip.Name = "Amarillo Vista";
            }

            // If the player is en-route to dropping the objective
            if (MissionBlip != null && MissionBlip.Color == BlipColor.Yellow3)
            {
                // If the current player vehicle does not matches the one where the objective is
                if (Game.Player.Character.CurrentVehicle != Objective.CurrentVehicle)
                {
                    // Destroy the objective blip
                    MissionBlip.Remove();
                    MissionBlip = null;
                    // Add a blip onto the vehicle
                    Objective.CurrentVehicle.AddBlip();
                    Objective.CurrentVehicle.CurrentBlip.Color = BlipColor.Blue2;
                    // And tell the user
                    UI.ShowSubtitle(Manager.Strings["M01_SUB08"], 4000);
                }

                // Draw a little marker to show where the player needs to stop
                World.DrawMarker(MarkerType.VerticalCylinder, DestinationLocation, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.Yellow);

                // If the player has arrived
                if (DestinationLocation.DistanceTo(Game.Player.Character.Position) <= 2)
                {
                    // Freeze the player vehicle
                    Game.Player.Character.CurrentVehicle.FreezePosition = true;
                    // Some dialog
                    UI.ShowSubtitle(Manager.Strings["M01_SUB06"], 4000);
                    Wait(4000);
                    // Fade out
                    Game.FadeScreenOut(1000);
                    Wait(1000);

                    // Time for some cleanup!
                    // Let's remove the character
                    Objective.Delete();
                    Objective = null;
                    // Set the mission as completed
                    IsInProgress = false;
                    // And delete the blip
                    MissionBlip.Remove();
                    MissionBlip = null;
                    // Set the mission as passed and save the progress
                    Manager.Completion.Mission01 = true;
                    Manager.SaveProgress();

                    // Fade in
                    Game.FadeScreenIn(1000);
                    Wait(1000);
                    // And finally unfreeze the vehicle
                    Game.Player.Character.CurrentVehicle.FreezePosition = false;
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
