using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(GameStartState), nameof(GameStartState.NewGame))]
	class GameStartState_NewGame_Patch
	{
		static void Postfix()
		{
			try
			{
				ItemTracker.Instance.Reset();
				ItemTracker.Instance.StartingNewGame();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}

	[HarmonyPatch(typeof(GameStartState), nameof(GameStartState.Continue))]
	class GameStartState_Continue_Patch
	{
		static void Postfix()
		{
			try
			{
				Log.Debug("In GameStartState_Continue_Patch.Postfix");
				ItemTracker.Instance.Reset();
				ItemTracker.Instance.RefreshItemTracker();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}
}
