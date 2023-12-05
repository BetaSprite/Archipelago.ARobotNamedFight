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
            if (__instance.data.globalID == -99)
            {
                Log.Debug("Picking up minor item drop from random enemy (globalID -99)");
                //if (!ArchipelagoClient.Instance.Configuration.SendMinorItemDropsAsChecks)
                //{
                //    ItemTracker.Instance.AddSkipCheck();
                //}
            }

            ItemTracker.Instance.LastPickedMinorItemGlobal = __instance.data.globalID;
        }
    }
}
