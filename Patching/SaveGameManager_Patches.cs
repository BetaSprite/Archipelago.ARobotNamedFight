using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(SaveGameManager), nameof(SaveGameManager.Save))]
	class SaveGameManager_Save_Patch
	{
		static void Postfix(bool saveLayout, bool forceSave)
		{
			if (!References.ExitingGame && saveLayout && forceSave)
			{
				Log.Debug("Not sure why we're resetting the Item Tracker in SaveGameManager.Save, but here we are...?");
				ItemTracker.Instance.Reset();
			}
		}
	}
}
