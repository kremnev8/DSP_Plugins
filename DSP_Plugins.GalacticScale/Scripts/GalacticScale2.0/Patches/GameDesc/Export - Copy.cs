﻿//using System.IO;
//using HarmonyLib;

//namespace GalacticScale
//{
//	public static class PatchOnGameDescExport
//	{
//		[HarmonyPatch(typeof(GameDesc))]
//		[HarmonyPrefix, HarmonyPatch("Export")]
//		public static bool Export(BinaryWriter w)
//		{
//			if (GS2.IsMenuDemo) return true;
//			GS2.Export(w);
//			return true;
//		}
//	}
//}