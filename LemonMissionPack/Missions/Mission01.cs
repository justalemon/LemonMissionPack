using GTA;
using GTA.Math;
using GTA.Native;
using System;

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

        public Mission01()
        {
            // Add our events
            Tick += OnTickBasics;
            Tick += OnTickMission;
            Aborted += OnAbort;
        }

        private void OnTickBasics(object sender, EventArgs e)
        {
            // If the mission is in progress, return
            if (IsInProgress)
            {
                return;
            }

            // If the user has not been notified, the base content is loaded, the game is not loading, the player is Franklin and the player can be controlled
            if (!Game.MissionFlag && !IsPlayerNotified && Manager.IsContentLoaded && !Game.IsLoading && (uint)Game.Player.Character.Model.Hash == (uint)PedHash.Franklin && Game.Player.CanControlCharacter)
            {
                // Notify the user
                // TODO: Make the message looks like is coming from Lester
                UI.Notify(Manager.Strings["M01_SMS"], true);
                IsPlayerNotified = true;
                return;
            }

            // If the blip does not exists and the player is Franklin
            if (!Game.MissionFlag && MissionBlip == null && (uint)Game.Player.Character.Model.Hash == (uint)PedHash.Franklin)
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
            if (!Game.MissionFlag && MissionBlip != null && MissionBlip.Exists() && (uint)Game.Player.Character.Model.Hash != (uint)PedHash.Franklin)
            {
                // Destroy the blip
                MissionBlip.Remove();
                MissionBlip = null;
                return;
            }
        }

        private void OnTickMission(object sender, EventArgs e)
        {
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
