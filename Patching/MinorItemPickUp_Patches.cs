using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(MinorItemPickUp), nameof(MinorItemPickUp.OnPickUp))]
	class MinorItemPickUp_OnPickUp_Patch
	{
		static void Prefix(MinorItemPickUp __instance, Player player)
		{
			//-99 is the ID of an item dropped by an enemy
			Log.Debug($"Picking up minor item globalID {__instance.data.globalID}");
			ItemTracker.Instance.LastPickedMinorItemGlobal = __instance.data.globalID;
		}
	}
}
