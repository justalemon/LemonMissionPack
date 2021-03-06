﻿using Citron;
using GTA;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LemonMissionPack
{
    /// <summary>
    /// Class that manages the basics of the missions.
    /// </summary>
    public class Manager : Script
    {
        /// <summary>
        /// The location of the progress file.
        /// </summary>
        private static readonly string ProgressPath = Path.Combine(Paths.GetCallingPath(), "LemonMissionPack", "UserProgress.json");
        /// <summary>
        /// The strings to show in the game.
        /// </summary>
        public static Dictionary<string, string> Strings { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// The current progress of the player.
        /// </summary>
        public static UserProgress Completion = JsonConvert.DeserializeObject<UserProgress>(File.ReadAllText(ProgressPath));
        /// <summary>
        /// If the base mission content has been loaded.
        /// </summary>
        public static bool IsContentLoaded { get; private set; } = false;

        public Manager()
        {
            // Reload the list of strings
            ReloadStrings(false);
            // And mark the basics as loaded
            IsContentLoaded = true;
        }

        public static void SaveProgress()
        {
            // Convert the user progress from a C# class to a string
            string JSON = JsonConvert.SerializeObject(Completion);
            // Write that JSON into the file
            File.WriteAllText(ProgressPath, JSON);
        }

        public static void ReloadStrings(bool Notify = true)
        {
            // Get the contents of the English/American language file
            string English = File.ReadAllText(Path.Combine(Paths.GetCallingPath(), "LemonMissionPack", "strings", "American.json"));
            // Parse and load the english strings
            Strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(English);

            // Create the path for the current user language
            string LangPath = Path.Combine(Paths.GetCallingPath(), "LemonMissionPack", "strings", $"{Game.Language}.json");
            // If the current language is not english and the file for that language exists
            if (Game.Language != Language.American && File.Exists(LangPath))
            {
                // Try to load and parse it 
                Dictionary<string, string> LangStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(LangPath));
                // And replace the individual existing values
                LangStrings.ToList().ForEach(x => Strings[x.Key] = x.Value);
            }

            // If we need to notify the user about the language reload
            if (Notify)
            {
                // Show the message
                UI.Notify(Strings["RELOAD"]);
            }
        }
    }
}
