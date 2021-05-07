﻿using HarmonyLib;

namespace GalacticScale
{
    
    public static class PatchOnGameOption
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOption), "LoadGlobal")]
        public static void LoadGlobal()
        {
            GS2.LoadPlugins();
            GS2.LoadPreferences();
        }
    }
}