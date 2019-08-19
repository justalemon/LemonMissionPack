using GTA;
using GTA.Math;
using System;
using System.Drawing;

namespace LemonMissionPack.Missions
{
    /// <summary>
    /// Mission 2: Grab a vehicle for the character to use.
    /// </summary>
    public class Mission02 : MissionBase
    {
        public Mission02()
        {
            // Add our events
            Tick += OnTick;
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
        }
    }
}
