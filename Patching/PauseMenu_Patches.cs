using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.ExitGame))]
	class PauseMenu_ExitGame_Patch
	{
		static void Prefix()
		{
			References.ExitingGame = true;
		}

		static void Postfix()
		{
			References.ExitingGame = false;
		}
	}
}
