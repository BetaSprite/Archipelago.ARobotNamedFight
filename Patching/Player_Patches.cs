﻿using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(Player), nameof(Player.CollectMinorItem))]
	class Player_CollectMinorItem_Patch
	{
		static bool Prefix(MinorItemType itemType)
		{
			Log.Debug($"CollectMinorItemPatch.Prefix picked up a {itemType} with a last picked item number {ItemTracker.Instance.LastPickedMinorItemGlobal}"); //, and I'm returning a {MinorItemType.GlitchModule}");
			//itemType = MinorItemType.GlitchModule;

			if (!ItemTracker.Instance.SkipSendCheck(true))
			{
				//Skip these steps for items picked up as enemy drops
				if (ItemTracker.Instance.LastPickedMinorItemGlobal > -99)
				{
					//long itemCheckNumber = ItemTracker.Instance.LastPickedMinorItemGlobal;
					long itemCheckNumber = ItemTracker.Instance.NextCheckNumber;
					//ItemTracker.Instance.ModItem(itemType);
					ArchipelagoClient.Instance.SendCheck(itemCheckNumber);
					SaveGameManager.instance.Save();
					Automap.instance.RefreshItems();
					//ItemTracker.Instance.NextCheckNumber++;
					return false;
				}
			}

			return true;
		}

		//static void Postfix(MinorItemType itemType)
		//{
		//	Log.Debug($"CollectMinorItemPatch.Postfix picked up a {itemType}");
		//}
	}

	[HarmonyPatch(typeof(Player), nameof(Player.CollectMajorItem))]
	class Player_CollectMajorItem_Patch
	{
		static bool Prefix(MajorItem itemType)
		{
			Log.Debug($"Player_CollectMajorItem_Patch.Prefix picked up a {itemType} in game mode {SaveGameManager.activeSlot.activeGameData.gameMode}");

			MajorItemInfo itemInfo = new MajorItemInfo() { fullName = "Error", description = "Error" };
			ItemManager.items.TryGetValue(itemType, out itemInfo);
			if (!ItemTracker.Instance.SkipSendCheck(true))
			{
				bool sendCheck = !References.MajorItemNeedsSpecialHandling(itemType);

				if (References.MajorItemIsTraversal(itemType))
				{
					long index = ItemTracker.Instance.traversalItemsReverse[itemType];

					if (index == 4 || index == 7)
					{
						sendCheck = true;
					}
				}

				if (sendCheck)
				{
					long itemCheckNumber = ItemTracker.Instance.NextCheckNumber;

					Log.Debug($"Item check number: {itemCheckNumber}");

					if (itemCheckNumber > -99)
					{
						ArchipelagoClient.Instance.SendCheck(itemCheckNumber);
						PlayerManager.instance.ItemCollected(itemType);
						Automap.instance.RefreshItems();
						//ItemTracker.Instance.NextCheckNumber++;
						return false;
					}
				}
			}

			//SaveGameData activeGame = SaveGameManager.activeGame;
			//Log.Debug($"layout.traversalItemCount: {activeGame.layout.traversalItemCount} + layout.minorItemCount {activeGame.layout.minorItemCount} + layout.bonusItemsAdded.Count {activeGame.layout.bonusItemsAdded.Count}");
			//Log.Debug($"Total items collected so far: {activeGame.itemsCollected.Count}");
			//foreach (MajorItem beh in activeGame.itemsCollected)
			//{
			//	Log.Debug($"Item that has previously been collected: {beh}");
			//}

			//if (activeGame.layout.itemOrder != null)
			//{
			//	foreach (MajorItem beh in activeGame.layout.itemOrder)
			//	{
			//		Log.Debug($"Item in order: {beh}");
			//	}
			//}

			//if (activeGame.layout.bonusItemsAdded != null)
			//{
			//	foreach (MajorItem beh in activeGame.layout.bonusItemsAdded)
			//	{
			//		Log.Debug($"Bonus items added: {beh}");
			//	}
			//}

			//var majorItemsCollected = activeGame.itemsCollected.FindAll((i) => activeGame.layout.itemOrder.Contains(i) || activeGame.layout.bonusItemsAdded.Contains(i)).Count;
			//Log.Debug($"majorItemsCollected: {majorItemsCollected}");
			//Log.Debug($"minorItemIdsCollected.Count: {activeGame.minorItemIdsCollected.Count}");

			//If we're actually receiving a non-progression MajorItem, make sure that we swap out the designation in the maps, so that we can still pick up that location
			ItemTracker.Instance.ReplaceMajorItemInRooms(itemType);

			return true;
		}
	}

	//  [HarmonyPatch(typeof(Player), nameof(Player.HandleJumping))]
	//  class Player_HandleJumping_Patch
	//  {
	//	  static void Postfix(Player __instance)
	//	  {
	//	if (ArchipelagoClient.Configuration.RespinEnabled && !Player.instance.spiderForm)
	//	{
	//		string jumpString = __instance.confused ? "Attack" : "Jump";
	//		bool jumpHeld = __instance.controller.GetButton(jumpString);
	//		if (__instance.jumping && !__instance.spinJumping && jumpHeld)
	//		{
	//			Log.Debug($"Initiating spin jump");

	//			__instance.spinJumping = true;
	//		}
	//	}
	//}
	//  }

	[HarmonyPatch(typeof(Player), nameof(Player.EndDeath))]
	class Player_EndDeath_Patch
	{
		static void Postfix()
		{
			Log.Debug($"EndDeath Postfix");
			ArchipelagoClient.Instance.deathLinkService.SendDeathLink(new DeathLink(ArchipelagoClient.Instance.connectedPlayerName, "Renaming robot designated 'Fight!'.  New name is 'Scrap'."));
		}
	}

#if DEBUG
	[HarmonyPatch(typeof(Player), nameof(Player.HandleDamage))]
	class Player_HandleDamage_Patch
	{
		static void Postfix(float damageAmount)
		{
			Player.instance.health = Player.instance.maxHealth;
			Player.instance.energy = Player.instance.maxEnergy;
			Player.instance.grayScrap += 20;
			Player.instance.redScrap += 1;
			Player.instance.greenScrap += 1;
			Player.instance.blueScrap += 1;

			ItemTracker.Instance.AddSkipCheck();
			Player.instance.CollectMinorItem(MinorItemType.DamageModule);
			ItemTracker.Instance.AddSkipCheck();
			Player.instance.CollectMinorItem(MinorItemType.ShotSpeedModule);

			var statMod = Player.instance.gameObject.AddComponent<TemporaryCelestialCharge>();
			statMod.Equip(Player.instance, 60f);

			//string dataStorageKey = $"Test";
			//Log.Debug($"dataStorageKey = {dataStorageKey}");
			//string runsCompletedDataStorage = ArchipelagoClient.Instance.TryGetDataStorage(dataStorageKey);

			//Log.Debug($"runsCompletedDataStorage = {runsCompletedDataStorage}");
			//int runsCompleted = 0;
			//if (!string.IsNullOrEmpty(runsCompletedDataStorage))
			//{
			//	int.TryParse(runsCompletedDataStorage, out runsCompleted);
			//}
			//runsCompleted++;

			//Log.Debug($"New runsCompleted value = {runsCompleted}");
			//ArchipelagoClient.Instance.SetDataStorage(dataStorageKey, runsCompleted.ToString());
			//Log.Debug("After SetDataStorage");
		}
	}
#endif
}
