using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
    //TODO: set up run completion triggers.  Might be here.
    [HarmonyPatch(typeof(SaveSlotData), nameof(SaveSlotData.RunCompleted))]
    class SaveSlotData_RunCompleted_Patch
    {
        static void Prefix(SaveSlotData __instance)
		{
            List<GameMode> handledGameModes = new List<GameMode>(){ GameMode.MegaMap, GameMode.MirrorWorld, GameMode.Normal, GameMode.Spooky, GameMode.TrueCoOp };
            if (handledGameModes.Contains(__instance.activeGameData.gameMode))
			{
                ArchipelagoClient.Instance.SendRunCompleted();
			}
		}
    }
}
