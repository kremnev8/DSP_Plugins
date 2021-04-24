﻿using System.Collections.Generic;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(ref  StarData star, GameDesc gameDesc)
        {
            GSSettings.Stars[star.index].counter = 0;
            star.planets = new PlanetData[GSSettings.Stars[star.index].bodyCount];
            for (var i = 0; i < GSSettings.Stars[star.index].planetCount; i++) CreatePlanet(ref star, GSSettings.Stars[star.index].Planets[i], null);
        }
    }
}