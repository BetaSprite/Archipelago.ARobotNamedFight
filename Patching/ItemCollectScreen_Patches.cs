using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Archipelago.ARobotNamedFight.Patching
{
    [HarmonyPatch(typeof(ItemCollectScreen), nameof(ItemCollectScreen.Show))]
    class ItemCollectScreen_Show_Patch
    {
        static bool Prefix(ItemInfo itemInfo)
        {
            Log.Debug("ItemCollectScreen_Show_Patch Prefix");

            return false;

   //         if (ArchipelagoClient.Instance.Configuration.SkipItemCollectScreenPopups)
   //         {
   //             //GUI.Label(new Rect(10, 10, 100, 20), "Does this work?");
   //             //Need to look into stealing the timer's UI...
   //             return false;
   //         }

   //         bool overrideDescription = !ItemTracker.Instance.SkipSendCheck();
   //         if (overrideDescription && itemInfo is MajorItemInfo)
			//{
   //             var mii = (MajorItemInfo)itemInfo;
   //             if (References.MajorItemBlacklist.Contains(mii.type))
			//	{
   //                 overrideDescription = false;
			//	}
			//}

   //         if (overrideDescription)
   //         {
   //             long itemCheckNumber = -99;

   //             if (itemInfo is MajorItemInfo)
   //             {
   //                 var mii = (MajorItemInfo)itemInfo;
   //                 if (ItemTracker.Instance.allAssignedMajorItemsReverse.ContainsKey(mii.type))
			//		{
   //                     itemCheckNumber = ItemTracker.Instance.allAssignedMajorItemsReverse[mii.type] + ItemTracker.Instance.allAssignedMinorItems.Count;
   //                 }
   //             }
   //             else if (ItemTracker.Instance.LastPickedMinorItemGlobal > -99)
			//	{
   //                 //Increasing the index to start at 1 for count and send logic
   //                 itemCheckNumber = ItemTracker.Instance.LastPickedMinorItemGlobal + 1;
   //             }

   //             if (itemCheckNumber > -99)
   //             {
   //                 var locationName = ArchipelagoClient.Instance.ScoutCheckLocationName(itemCheckNumber);

   //                 if (locationName != null)
   //                 {
   //                     itemInfo.fullName = $"{locationName}";
   //                     itemInfo.description = $"Scouted location name for {itemCheckNumber}";
   //                 }
   //                 else
   //                 {
   //                     itemInfo.fullName = "Item sent to another player!";
   //                     itemInfo.description = $"No location name found for {itemCheckNumber}, though...";
   //                 }
   //             }
   //         }

   //         return true;
        }
    }

    //[HarmonyPatch(typeof(ItemCollectScreen), nameof(ItemCollectScreen.WaitForJingle))]
    //class ItemCollectScreen_WaitForJingle_Patch
    //{
    //    static void Postfix()
    //    {
    //        ItemTracker.Instance.ShowingItemCollectScreen = false;
    //    }
    //}
}
