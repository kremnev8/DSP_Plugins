﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GalacticScale.GS2;
using static GalacticScale.RomanNumbers;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        
        private void GenerateStars(int starCount)
        {
            Log("Generating Stars");

            for (var i = 0; i < starCount; i++)
            {
                var (type, spectr) = ChooseStarType();
                var star = new GSStar(random.Next(), SystemNames.GetName(i), spectr, type,
                    new GSPlanets());
                if (star.Type != EStarType.BlackHole) star.radius *= preferences.GetFloat("starSizeMulti", 10f);
                if (star.Type == EStarType.BlackHole && preferences.GetFloat("starSizeMulti", 10f) < 2.01f)
                    star.radius *= preferences.GetFloat("starSizeMulti", 2f);
                star.dysonRadius =
                    star.dysonRadius * Mathf.Clamp(preferences.GetFloat("starSizeMulti", 10f), 0.5f, 100f);
                //Warn($"Habitable zone for {star.Name} {Utils.CalculateHabitableZone(star.luminosity)}");
                star.Seed = random.Next();
                GSSettings.Stars.Add(star);
            }
            birthStar = random.Item(GSSettings.Stars);
        }
        private int GetStarPlanetCount(GSStar star)
        {
            var min = GetMinPlanetCountForStar(star);
            var max = GetMaxPlanetCountForStar(star);
            //int result = random.NextInclusive(min, max);
            var result = ClampedNormal(min, max, GetCountBiasForStar(star));
            //Log($"{star.Name} count :{result} min:{min} max:{max}");
            return result;
        }
        private int GetStarMoonSize(GSStar star, int hostRadius, bool hostGas)
        {
            if (hostGas) hostRadius *= 10;
            var min = Utils.ParsePlanetSize(GetMinPlanetSizeForStar(star));
            int max;
            if (preferences.GetBool("moonsAreSmall", true))
            {
                float divider = 2;
                if (hostGas) divider = 4;
                max = Utils.ParsePlanetSize(Mathf.RoundToInt(hostRadius / divider));
            }
            else
            {
                max = Utils.ParsePlanetSize(hostRadius - 10);
            }

            if (max <= min) return min;
            float average = (max - min) / 2 + min;
            var range = max - min;
            var sd = (float) range / 4;
            //int size = Utils.ParsePlanetSize(random.Next(min, max));
            var size = ClampedNormalSize(min, max, GetSizeBiasForStar(star));
            //if (size > hostRadius)
            //{
            //Warn($"MoonSize {size} selected for {star.Name} moon with host size {hostRadius} avg:{average} sd:{sd} max:{max} min:{min} range:{range} hostGas:{hostGas}");
            //    size = Utils.ParsePlanetSize(hostRadius - 10);
            //}
            return size;
        }
        private int GetStarPlanetSize(GSStar star)
        {
            var min = GetMinPlanetSizeForStar(star);
            var max = GetMaxPlanetSizeForStar(star);
            var bias = GetSizeBiasForStar(star);
            return ClampedNormalSize(min, max, bias);
        }
        private (float min, float max) CalculateHabitableZone(GSStar star)
        {
            var lum = star.luminosity;
            var (max, min) = Utils.CalculateHabitableZone(lum);
            star.genData.Set("minHZ", min);
            star.genData.Set("maxHZ", max);
            // GS2.Warn($"HZ of {star.Name} {min}:{max}");
            return (min, max);
        }

        private float CalculateMinimumOrbit(GSStar star)
        {
            var radius = star.RadiusAU;
            var lum = star.luminosity;
            var min = radius +( 0.5f * radius * Mathf.Sqrt(lum));
            var density = 3f/(6f-GetSystemDensityForStar(star));
            // GS2.Warn($"Density:{density} Min:{min} Output:{min*density}");
            min = Mathf.Clamp(min * density, radius * 1.2f, 100f);
            star.genData.Set("minOrbit", min);
            return min;
        }

        private float CalculateMaximumOrbit(GSStar star)
        {
            var minMaxOrbit = 5f;
            var lum = star.luminosity;
            var hzMax = star.genData.Get("maxHZ");
            var maxOrbitByLuminosity = lum * 4f;
            var maxOrbitByRadius = Mathf.Sqrt(star.radius);
            var maxOrbitByHabitableZone = 2f * hzMax;
            var maxByPlanetCount = star.bodyCount * 0.3f;
            var density = (6f-GetSystemDensityForStar(star))/3f;
            var max = Mathf.Clamp(density * Mathf.Max(maxByPlanetCount, minMaxOrbit, maxOrbitByLuminosity, maxOrbitByRadius, maxOrbitByHabitableZone), star.genData.Get("minOrbit"), star.MaxOrbit);
            // Warn($"Getting Max Orbit for Star {star.Name} MaxbyRadius({star.radius}):{maxOrbitByRadius} MaxbyPlanets({star.PlanetCount}):{maxByPlanetCount} MaxbyLum({lum}):{maxOrbitByLuminosity} MaxByHZ({hzMax}):{maxOrbitByHabitableZone} Max({max}):{max} HabitableZone:{star.genData.Get("minHZ")}:{hzMax}");
            star.genData.Set("maxOrbit", max);
            return max;
        }
    }
}