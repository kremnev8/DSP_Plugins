﻿using System.Collections.Generic;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void SavePreferences()
        {
            Log("Start");
            var preferences = new GSPreferences();
            preferences.MainSettings = GS2.Config.Export();
            // preferences.GeneratorID = generator.GUID;
            // preferences.debug = debugOn;
            // preferences.forceRare = GS2.mainSettings.ForceRare;
            // preferences.skipPrologue = SkipPrologue;
            // preferences.noTutorials = tutorialsOff;
            // preferences.cheatMode = CheatMode;
            Log("Retrieving preferences for plugins");
            foreach (var g in Generators)
                if (g is iConfigurableGenerator)
                {
                    var gen = g as iConfigurableGenerator;
                    Log("Trying to get preferences for " + gen.Name);
                    var prefs = gen.Export();

                    preferences.PluginOptions[gen.GUID] = prefs;
                    Log("Finished adding preferences for " + gen.Name);
                }
            
            var serializer = new fsSerializer();
            Log("Trying to serialize preferences object");
            serializer.TrySerialize(preferences, out var data);
            Log("Serialized");
            var json = fsJsonPrinter.PrettyJson(data);
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);

            File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
            Log("End");
        }

        public static void LoadPreferences(bool debug = false)
        {
            Log("Start");
            var path = Path.Combine(DataDir, "Preferences.json");
            if (!CheckJsonFileExists(path)) return;

            Log("Loading Preferences from " + path);
            var serializer = new fsSerializer();
            var json = File.ReadAllText(path);
            var preferences = new GSPreferences();
            var data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize(data2, ref preferences);
            if (!debug)
                ParsePreferences(preferences);
            else
            {
                Config.Import(preferences.MainSettings);
                // debugOn = preferences.debug;
            }
            Log("Preferences loaded");
            Log("End");
        }

        private static void ParsePreferences(GSPreferences p)
        {
            Log("Start");
            // debugOn = p.debug;
            Config.Import(p.MainSettings);
            // if (DebugLogOption != null) DebugLogOption.Set(debugOn);
            // Force1RareChance = p.forceRare;
            // if (Force1RareChanceOption != null) Force1RareChanceOption.Set(Force1RareChance);
            // SkipPrologue = p.skipPrologue;
            // if (SkipPrologueOption != null) SkipPrologueOption.Set(SkipPrologue);
            // tutorialsOff = p.noTutorials;
            // if (NoTutorialsOption != null) NoTutorialsOption.Set(tutorialsOff);
            // CheatMode = p.cheatMode;
            // if (CheatModeOption != null) CheatModeOption.Set(CheatMode);
            // generator = GetGeneratorByID(p.GeneratorID);
            if (p.PluginOptions != null)
                foreach (var pluginOptions in p.PluginOptions)
                {
                    Log("Plugin Options for " + pluginOptions.Key + "found");
                    var gen = GetGeneratorByID(pluginOptions.Key) as iConfigurableGenerator;
                    if (gen != null)
                    {
                        Log(gen.Name + "'s plugin options exported to generator");
                        gen.Import(pluginOptions.Value);
                    }
                }

            Log("End");
        }

        private class GSPreferences
        {
            // public bool cheatMode;
            // public bool debug;
            // public bool forceRare;
            // public string GeneratorID = "space.customizing.vanilla";
            // public bool noTutorials;
            public GSGenPreferences MainSettings = new GSGenPreferences();
            public readonly Dictionary<string, GSGenPreferences> PluginOptions =
                new Dictionary<string, GSGenPreferences>();

            // public bool skipPrologue;
        }
    }
}