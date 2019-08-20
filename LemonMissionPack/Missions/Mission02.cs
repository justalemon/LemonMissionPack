using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Drawing;

namespace LemonMissionPack.Missions
{
    /// <summary>
    /// Mission 2: Steal a vehicle for the character to use.
    /// </summary>
    public class Mission02 : MissionBase
    {
        /// <summary>
        /// The location of the mission start.
        /// </summary>
        private static readonly Vector3 Start = new Vector3(1291.8f, -1717.5f, 54f);
        /// <summary>
        /// The target vehicle that we need to steal.
        /// </summary>
        private static Vehicle Target;

        public Mission02()
        {
            // Add our events
            Tick += OnTick;
            Aborted += OnAbort;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // If this mission has been completed
            if (Manager.Completion.Mission02 != 0)
            {
                // Remove the tick event and return
                Tick -= OnTick;
                return;
            }

            // If the 1st mission has not been completed
            if (Manager.Completion.Mission01 == 0)
            {
                // Return, because the user needs to complete that first
                return;
            }

            // If the player has not been notified and a minute has passed since the last mission
            if (!IsPlayerNotified && ((DateTime.UtcNow - DateTime.MinValue).TotalSeconds - Manager.Completion.Mission01) >= 60)
            {
                // Notify the user
                UI.Notify(Manager.Strings["M02_SMS"]);
                IsPlayerNotified = true;
                return;
            }

            // If the player has been notified and the mission has not been started
            if (IsPlayerNotified && !IsInProgress && Game.Player.CanControlCharacter)
            {
                // Draw a little marker where the mission should start
                World.DrawMarker(MarkerType.VerticalCylinder, Start, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.Yellow);

                // If the player is near mission start witout a vehicle
                if (Start.DistanceTo(Game.Player.Character.Position) < 2 && Game.Player.Character.CurrentVehicle == null)
                {
                    // Fade the screen out
                    Game.FadeScreenOut(1000);
                    Wait(1000);

                    // Mark the mission as started
                    IsInProgress = true;
                    // Change the time of day to morning
                    World.CurrentDayTime = new TimeSpan(7, 0, 0);
                    // Move the player to an appropiate position and lock him in place
                    Game.Player.Character.Position = new Vector3(1290.3f, -1714, 54);
                    Game.Player.Character.Heading = 111;
                    Game.Player.Character.FreezePosition = true;
                    // Request the freemode male model
                    Model FreemodeMale = new Model(PedHash.FreemodeMale01);
                    FreemodeMale.Request();
                    // Wait until the model has been loaded
                    while (!FreemodeMale.IsLoaded)
                    {
                        Yield();
                    }
                    // Then, spawn the objective ped and also freeze it in place
                    Ped Objective = World.CreatePed(FreemodeMale, new Vector3(1288.5f, -1714, 54), 396.4f);
                    Objective.Heading = 293;
                    Objective.FreezePosition = true;

                    // Fade in
                    Game.FadeScreenIn(1000);
                    Wait(1000);

                    // And print the dialog
                    UI.ShowSubtitle(Manager.Strings["M02_SUB01"], 4000);
                    Wait(4000);
                    UI.ShowSubtitle(Manager.Strings["M02_SUB02"], 4000);
                    Wait(4000);
                    UI.ShowSubtitle(Manager.Strings["M02_SUB03"], 4000);
                    Wait(4000);
                    UI.ShowSubtitle(Manager.Strings["M02_SUB04"], 4000);
                    Wait(4000);
                    UI.ShowSubtitle(Manager.Strings["M02_SUB05"], 4000);
                    Wait(4000);

                    // Then, fade out again
                    Game.FadeScreenOut(1000);
                    Wait(1000);
                    // Remove the ped
                    Objective.Delete();
                    // Change the player position
                    Game.Player.Character.Position = new Vector3(1292.2f, -1718.4f, 54);
                    Game.Player.Character.Heading = 206.5f;
                    // Request the Banshee model
                    Model Banshee = new Model(VehicleHash.Banshee2);
                    Banshee.Request();
                    while (!Banshee.IsLoaded)
                    {
                        Yield();
                    }
                    // Create a vehicle and add a blip
                    Target = World.CreateVehicle(Banshee, new Vector3(1054.3f, -3039.7f, 5.7f), 268.2f);
                    Target.AddBlip();
                    Target.CurrentBlip.Sprite = BlipSprite.GetawayCar;
                    Target.CurrentBlip.IsShortRange = false;
                    Target.CurrentBlip.Color = BlipColor.Green;
                    // Change some tunning options
                    Function.Call(Hash.SET_VEHICLE_EXTRA, Target, 2, 1);
                    // Destroy the old blip
                    MissionBlip.Remove();
                    MissionBlip = null;

                    // Fade back in
                    Game.FadeScreenIn(1000);
                    Wait(750);
                    // Unfreeze the player
                    Game.Player.Character.FreezePosition = false;
                    // And show a subtitle with instructions
                    UI.ShowSubtitle(Manager.Strings["M02_SUB07"], 4000);
                }

                // If there is no blip and no vehicle to steal
                if (MissionBlip == null && Target == null)
                {
                    // Create a blip on the start position
                    MissionBlip = World.CreateBlip(Start);
                    MissionBlip.Sprite = BlipSprite.Lester;
                    MissionBlip.Color = BlipColor.Yellow;
                    MissionBlip.Name = "Placeholder (LMP)";
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

            // If there is a vehicle
            if (Target != null && Target.Exists())
            {
                // Destroy the blip if it exists
                if (Target.CurrentBlip != null && Target.CurrentBlip.Exists())
                {
                    Target.CurrentBlip.Remove();
                }

                // Then, destroy the vehicle
                Target.Delete();
            }

            // If the screen is faded, return it into the original pov
            if (Game.IsScreenFadedIn)
            {
                Game.FadeScreenIn(1);
            }
        }
    }
}
