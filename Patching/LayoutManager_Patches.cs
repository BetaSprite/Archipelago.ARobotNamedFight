using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(LayoutManager), nameof(LayoutManager.Start))]
	class LayoutManager_Start_Patch
	{
		static void Postfix()
		{
			ItemTracker.Instance.RefreshItemTracker();
			ItemTracker.Instance.GetNewGameItems();
		}
	}
}
