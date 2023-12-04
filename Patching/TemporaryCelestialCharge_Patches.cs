using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(TemporaryCelestialCharge))]
	[HarmonyPatch("Update")]
	class TemporaryCelestialCharge_Update_Patch
	{
		static void Postfix()
		{
#if DEBUG
			if (ArchipelagoClient.Instance.Configuration.GodMode)
			{
				MusicController.instance.SetPitch(1);
			}
#endif
		}
	}
}
