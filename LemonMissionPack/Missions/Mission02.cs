using GTA;
using GTA.Math;
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
            if (IsPlayerNotified && !IsInProgress)
            {
                // Draw a little marker where the mission should start
                World.DrawMarker(MarkerType.VerticalCylinder, Start, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.Yellow);

                // If there is no blip
                if (MissionBlip == null)
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
        }
    }
}
