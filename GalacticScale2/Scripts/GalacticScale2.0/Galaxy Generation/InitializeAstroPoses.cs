﻿using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanetsAstroPoses(Random random)
        {
            var highStopwatch = new HighStopwatch();
            highStopwatch.Begin();
            var gSize = galaxy.starCount * 1000;
            galaxy.astroPoses = new AstroPose[gSize];
            Log("Creating Stars");
            for (var i = 0; i < GSSettings.StarCount; i++) galaxy.stars[i] = CreateStar(i, random);
            //for (var i = 0; i < galaxy.stars.Length; i++) GS2.Warn($"Star {galaxy.stars[i].index} id:{galaxy.stars[i].id} name:{galaxy.stars[i].name} GSSettings:{GSSettings.Stars[i].Name}");
            Log($"Stars Created in {highStopwatch.duration:F5}s");highStopwatch.Begin();
            Log("Creating Planets");
            for (var i = 0; i < GSSettings.StarCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc, random);
            Log($"Planets Created in {highStopwatch.duration:F5}s");highStopwatch.Begin();
            Log("Planets have been created");
            var astroPoses = galaxy.astroPoses;
            for (var index = 0; index < galaxy.astroPoses.Length; ++index)
            {
                astroPoses[index].uRot.w = 1f;
                astroPoses[index].uRotNext.w = 1f;
            }
            Log($"Astroposes Reset in {highStopwatch.duration:F5}s");highStopwatch.Begin();
            for (var index = 0; index < GSSettings.StarCount; ++index)
            {
                astroPoses[galaxy.stars[index].id * 100].uPos = astroPoses[galaxy.stars[index].id * 100].uPosNext =
                    galaxy.stars[index].uPosition;
                astroPoses[galaxy.stars[index].id * 100].uRot =
                    astroPoses[galaxy.stars[index].id * 100].uRotNext = Quaternion.identity;
                astroPoses[galaxy.stars[index].id * 100].uRadius = galaxy.stars[index].physicsRadius;
            }
            Log($"Astroposes filled in {highStopwatch.duration:F5}s");highStopwatch.Begin();
            Log("Updating Poses");
            galaxy.UpdatePoses(0.0);
            Log($"Astroposes Initialized in {highStopwatch.duration:F5}s");highStopwatch.Begin();
            Log("End");
        }
    }
}