using HarmonyLib;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archipelago.ARobotNamedFight
{
	public sealed class ItemTracker : IDisposable
	{
		private static Lazy<ItemTracker> lazy = new Lazy<ItemTracker>(() => new ItemTracker());

		private ItemTracker() { }

		public static ItemTracker Instance { get { return lazy.Value; } }

		private bool NeedsLoad = true;

		public bool NeedsNewGameItems { get; set; } = false;

		public long NextCheckNumber
		{
			get
			{
				return ArchipelagoClient.Instance.GetNextCheckNumberFromSession();
			}
		}

		private int PickingUpSkipCheck = 0;

		public int LastPickedMinorItemGlobal = -99;

		public bool InShrineOrShopCollection = false;

		//public Dictionary<MinorItemType, int> MinorItemsCollected { get; private set; } = new Dictionary<MinorItemType, int>();

		public Dictionary<MinorItemType, int> CheckIgnores { get; private set; } = new Dictionary<MinorItemType, int>();

		public Dictionary<MinorItemType, int> CheckForces { get; private set; } = new Dictionary<MinorItemType, int>();

		public Dictionary<long, MinorItemType> allAssignedMinorItems { get; private set; } = new Dictionary<long, MinorItemType>();

		public Dictionary<long, MajorItem> allAssignedMajorItems { get; private set; } = new Dictionary<long, MajorItem>();

		public Dictionary<MajorItem, long> allAssignedMajorItemsReverse { get; private set; } = new Dictionary<MajorItem, long>();

		public Dictionary<long, MajorItem> traversalItems { get; private set; } = new Dictionary<long, MajorItem>();

		public Dictionary<MajorItem, long> traversalItemsReverse { get; private set; } = new Dictionary<MajorItem, long>();

		public List<MajorItemInfo> availableNonTraversalItems { get; private set; } = new List<MajorItemInfo>();

		public Queue<KeyValuePair<long, string>> ReceiptQueue { get; private set; } = new Queue<KeyValuePair<long, string>>();

		public Queue<long> LocationExpendQueue { get; private set; } = new Queue<long>();

		Dictionary<GameMode, int> ExpectedLocationCountPerGameMode = new Dictionary<GameMode, int>()
		{
			//32 minor, 4 major, 7 traversal, 4 glitch
			{ GameMode.Normal, 36 },
			{ GameMode.ClassicBossRush, 13 },
		};

		public void Reset()
		{
			//ItemTracker.Instance.NextCheckNumber = 0;
			LastPickedMinorItemGlobal = -99;
			//MinorItemsCollected.Clear();
			allAssignedMinorItems.Clear();
			allAssignedMajorItems.Clear();
			allAssignedMajorItemsReverse.Clear();
			CheckIgnores.Clear();
			CheckForces.Clear();

			NeedsLoad = true;
		}

		public void StartingNewGame()
		{
			NeedsNewGameItems = true;
		}

		public int TotalLocationsInCurrentGame
		{
			get
			{
				return allAssignedMinorItems.Count + allAssignedMajorItems.Count + traversalItems.Count;
			}
		}

		public int TotalLocationsExpectedForGameMode(GameMode gameMode)
		{
			if (ExpectedLocationCountPerGameMode.ContainsKey(gameMode)) return ExpectedLocationCountPerGameMode[gameMode];

			return 0;
		}

		public void AddSkipCheck(int amt = 1)
		{
			PickingUpSkipCheck += amt;
			Log.Debug($"AddSkipCheck. amt = {amt}. Updated PickingUpSkipCheck = {PickingUpSkipCheck}");
		}

		public bool SkipSendCheck(bool consume = false)
		{
			Log.Debug($"SkipSendCheck. consume = {consume}. PickingUpSkipCheck = {PickingUpSkipCheck}. InShrineOrShopCollection = {InShrineOrShopCollection}.");
			bool bRet = false;
			if (InShrineOrShopCollection)
			{
				bRet = true;
				if (consume) ItemTracker.Instance.InShrineOrShopCollection = false;
			}
			if (!bRet)
			{
				bRet = PickingUpSkipCheck > 0;
				if (consume && bRet) PickingUpSkipCheck--;
			}
			return bRet;
		}

		//public void AddSingleCheckForce(MinorItemType minorItemType)
		//{
		//	Log.Debug($"In AddSingleCheckForce for {minorItemType}");
		//	RefreshItemTracker();
		//	if (CheckForces.ContainsKey(minorItemType))
		//	{
		//		CheckForces[minorItemType]++;
		//	}
		//	else
		//	{
		//		CheckForces.Add(minorItemType, 1);
		//	}
		//}

		public void ReplaceMajorItemInRooms(MajorItem majorItemType)
		{
			Log.Debug($"In ReplaceMajorItemInRooms for {majorItemType}");
			if (!References.MajorItemNeedsSpecialHandling(majorItemType))
			{
				SaveGameData activeGame = SaveGameManager.activeGame;
				RoomAbstract targetRoom = null;

				var majorItemsFoundInRooms = GetMajorItemsAndRooms();
				if (majorItemsFoundInRooms.ContainsKey(majorItemType)) { targetRoom = majorItemsFoundInRooms[majorItemType]; }

				if (targetRoom == null)
				{
					Log.Error($"Couldn't find major item {majorItemType} in rooms!  Must be bonus.");
				}
				else
				{
					MajorItem newItem = GetUnusedMajorItem(majorItemsFoundInRooms);
					
					//Make the swap
					Log.Debug($"Swapping major item {targetRoom.majorItem} in map with dummy item {newItem}");
					targetRoom.majorItem = newItem;

					Log.Debug($"Attempting replacement in item tracker");
					if (allAssignedMajorItemsReverse.ContainsKey(majorItemType))
					{
						long index = allAssignedMajorItemsReverse[majorItemType];
						Log.Debug($"Original index {index} for {majorItemType}, and {allAssignedMajorItems[index]} is also found");
						allAssignedMajorItems[index] = newItem;
						Log.Debug($"Changed to {allAssignedMajorItems[index]}");
						allAssignedMajorItemsReverse.Remove(majorItemType);
						allAssignedMajorItemsReverse.Add(newItem, index);
						Log.Debug($"And after that, reverse lookup for {newItem} has {allAssignedMajorItemsReverse[newItem]} at index {index}.");
					}

					try
					{
						Log.Debug($"Swapping item in itemOrder");
						if (activeGame.layout.itemOrder != null && activeGame.layout.itemOrder.Contains(majorItemType))
						{
							activeGame.layout.itemOrder.Remove(majorItemType);
							activeGame.layout.itemOrder.Add(newItem);
						}
						Log.Debug($"Swapping item in bonusItemsAdded");
						if (activeGame.layout.bonusItemsAdded != null && activeGame.layout.bonusItemsAdded.Contains(majorItemType))
						{
							activeGame.layout.bonusItemsAdded.Remove(majorItemType);
							activeGame.layout.bonusItemsAdded.Add(newItem);
						}
						Log.Debug($"Swapping item in itemsCollected");
						if (activeGame.itemsCollected != null && activeGame.itemsCollected.Contains(majorItemType))
						{
							activeGame.itemsCollected.Remove(majorItemType);
							activeGame.itemsCollected.Add(newItem);
						}
					}
					catch (Exception ex) 
					{
						Log.Error(ex);
					}
				}
			}
			else
			{
				Log.Debug($"{majorItemType} is blacklisted, so we won't replace it in the map.");
			}
		}

		public Dictionary<MajorItem, RoomAbstract> GetMajorItemsAndRooms()
		{
			SaveGameData activeGame = SaveGameManager.activeGame;
			Dictionary<MajorItem, RoomAbstract> majorItemsFoundInRooms = new Dictionary<MajorItem, RoomAbstract>();
			foreach (var roomAbstract in activeGame.layout.roomAbstracts)
			{
				if (!majorItemsFoundInRooms.ContainsKey(roomAbstract.majorItem))
				{
					//Gather up the major items that already exist
					//Log.Debug($"New major item found: {roomAbstract.majorItem}");
					majorItemsFoundInRooms.Add(roomAbstract.majorItem, roomAbstract);
				}
			}

			return majorItemsFoundInRooms;
		}

		public MajorItem GetUnusedMajorItem(Dictionary<MajorItem, RoomAbstract> majorItemsFoundInRooms = null)
		{
			if (majorItemsFoundInRooms == null) 
			{
				majorItemsFoundInRooms = GetMajorItemsAndRooms();
			}

			//Find a new, unused major item that isn't blacklisted
			System.Random random = new System.Random();
			MajorItem itemType = availableNonTraversalItems[random.Next(availableNonTraversalItems.Count)].type;
			Log.Debug($"Attempt to replace item with {itemType}");
			while (majorItemsFoundInRooms.ContainsKey(itemType) || References.MajorItemNeedsSpecialHandling(itemType) || Player.instance.itemsPossessed.Contains(itemType))
			{
				itemType = availableNonTraversalItems[random.Next(availableNonTraversalItems.Count)].type;
				Log.Debug($"Nope, try {itemType} instead?");
			}

			return itemType;
		}

		public void RefreshItemTracker()
		{
			if (NeedsLoad)
			{
				NeedsLoad = false;
				try
				{
					Log.Debug("RefreshItemTracker has been triggered");

					SaveGameData activeGame = SaveGameManager.activeGame;

					Log.Debug($"Loop through all rooms. Count: {activeGame.layout.roomAbstracts.Count}");
					int iRegularMajorItemCounter = 0;
					int iTraversalMajorItemCount = 0;

					foreach (var majorItem in activeGame.layout.itemOrder)
					{
						if (References.MajorItemIsTraversal(majorItem))
						{
							Log.Debug($"Adding traversal major item {majorItem} to list with ID {iTraversalMajorItemCount}.");
							traversalItems.Add(iTraversalMajorItemCount, majorItem);
							traversalItemsReverse.Add(majorItem, iTraversalMajorItemCount);
							iTraversalMajorItemCount++;
						}
					}

					foreach (var roomAbstract in activeGame.layout.roomAbstracts)
					{
						//Log.Debug($"Loop through this room's minor items to catalog their types and IDs. Count: {roomAbstract.minorItems.Count}");
						foreach (var item in roomAbstract.minorItems)
						{
							Log.Debug($"Item {item.type} found with global ID {item.globalID}");

							//Increasing the index to start at 1 for count and send logic
							allAssignedMinorItems.Add(item.globalID, item.type);
						}

						if (!References.MajorItemNeedsSpecialHandling(roomAbstract.majorItem))
						{
							Log.Debug($"Adding major item {roomAbstract.majorItem} to list with ID {iRegularMajorItemCounter}.");
							allAssignedMajorItems.Add(iRegularMajorItemCounter, roomAbstract.majorItem);
							allAssignedMajorItemsReverse.Add(roomAbstract.majorItem, iRegularMajorItemCounter);
							iRegularMajorItemCounter++;
						}
						else if (roomAbstract.majorItem != MajorItem.None)
						{
							Log.Debug($"Ignoring progression major item {roomAbstract.majorItem} due to blacklist.");
						}
					}

					Log.Debug($"Total minor items: {allAssignedMinorItems.Count}");
					Log.Debug($"Total non-progress major items: {allAssignedMajorItems.Count}");

					List<MajorItemInfo> loadedItemInfos = new List<MajorItemInfo>();
					loadedItemInfos.AddRange(Resources.LoadAll<MajorItemInfo>("MajorItemInfos/OtherItems"));
					var achievements = SaveGameManager.activeSlot.achievements;
					foreach (var item in loadedItemInfos)
					{
						if (item.nontraversalPool && (item.requiredAchievement == AchievementID.None || achievements.Contains(item.requiredAchievement)))
						{
							availableNonTraversalItems.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			}
		}

		public void GetNewGameItems()
		{
			if (NeedsNewGameItems)
			{
				NeedsNewGameItems = false;
				try
				{
					if (ArchipelagoClient.Instance.SlotServerSettings.StartWithExplorb)
					{
						Log.Debug("Enqueue bonus Explorb");
						ItemTracker.Instance.ReceiptQueue.Enqueue(new KeyValuePair<long, string>(-9999, "Explorb"));
					}

					if (ArchipelagoClient.Instance.SlotServerSettings.StartWithMasterMap)
					{
						Log.Debug("Enqueue bonus Master Map");
						ItemTracker.Instance.ReceiptQueue.Enqueue(new KeyValuePair<long, string>(-9999, "MasterMap"));
					}

					//#if DEBUG
					//					if (ArchipelagoClient.Instance.Configuration.GodMode)
					//					{
					//						Log.Debug("Enqueue GodMode item collection");
					//						ItemTracker.Instance.ReceiptQueue.Enqueue(new KeyValuePair<long, string>(-99999, "GodModeInfinijump"));
					//					}
					//#endif

					Log.Debug("Needs new game items");
					ArchipelagoClient.Instance.HandleAllReceivedItems();
					ArchipelagoClient.Instance.HandleAllCheckedLocations();

					Log.Debug("Enqueued");
				}
				catch (NullReferenceException ex)
				{
					Log.Error($"NullReferenceException hit in GetNewGameItems: {ex}");
					NeedsNewGameItems = true;
				}
			}
		}

		public void ConsumeLocationExpendQueue()
		{
			if (!Player.instance.paused && Player.instance.enabled && LocationExpendQueue.Any())
			{
				long itemLocation = LocationExpendQueue.Dequeue();
				Log.Debug($"ConsumeLocationExpendQueue: location {itemLocation}");

				if (ItemTracker.Instance.allAssignedMinorItems.ContainsKey(itemLocation))
				{
					SaveGameData activeGame = SaveGameManager.activeGame;
					activeGame.minorItemIdsCollected.Add((int)itemLocation);
					Automap.instance.RefreshItems();
				}
				else
				{
					long majorItemId = itemLocation - ItemTracker.Instance.allAssignedMinorItems.Count;
					if (ItemTracker.Instance.allAssignedMajorItems.ContainsKey(majorItemId))
					{
						MajorItem itemType = ItemTracker.Instance.allAssignedMajorItems[majorItemId];
						Log.Debug($"Major item {itemLocation} (modified ID {majorItemId}) found to be {itemType}");
						PlayerManager.instance.ItemCollected(itemType);
						Automap.instance.RefreshItems();
					}
					else
					{
						Log.Debug($"Received location {itemLocation} (modified ID {majorItemId}), which is not in the items collection: {ItemTracker.Instance.allAssignedMinorItems.Count} minors and {ItemTracker.Instance.allAssignedMajorItems.Count} majors.");
					}
				}
			}
		}

		public void ConsumeReceiptQueue()
		{
			if (!Player.instance.paused && Player.instance.enabled && ReceiptQueue.Any())
			{
				KeyValuePair<long, string> keyValuePair = ReceiptQueue.Dequeue();
				long key = keyValuePair.Key;
				string value = keyValuePair.Value;
				Log.Debug($"ConsumeReceiptQueue: location {key}. name {value}");

				CollectReceivedItem(key, value);
			}
		}

		public void CollectReceivedItem(long itemLocation, string name)
		{
			Log.Debug($"In CollectReceivedItem for itemId {itemLocation}");
			if (itemLocation == -9999)
			{
				if (name == "Explorb")
				{
					Log.Debug("Receiving configured Explorb");
					ReceiveMajorItem(MajorItem.Explorb);
				}
				if (name == "MasterMap")
				{
					Log.Debug("Receiving configured Master Map");
					ReceiveMajorItem(MajorItem.MasterMap);
				}
			}
			else if (itemLocation == -99999)
			{
				Log.Debug($"Adding GodMode items");
				ReceiveMajorItem(MajorItem.Infinijump);
				ReceiveMajorItem(MajorItem.PhaseShot);
				ReceiveMajorItem(MajorItem.FireBolt);
				ReceiveMajorItem(MajorItem.ElectroCharge);
				ReceiveMajorItem(MajorItem.ExplosiveBolt);
				ReceiveMajorItem(MajorItem.BuzzsawShell);
				ReceiveMajorItem(MajorItem.CognitiveStabilizer);
				ReceiveMajorItem(MajorItem.BuzzsawGun);
				ReceiveMajorItem(MajorItem.CelestialCharge);
			}
			else
			{
				long offset = References.GetGameModeOffset();
				long upperBoundExclusive = References.GetGameModeUpperBoundExclusive();
				itemLocation -= offset;

				if (itemLocation < 0 || itemLocation > upperBoundExclusive)
				{
					Log.Debug($"Game mode {SaveGameManager.activeSlot.activeGameData.gameMode} has offset {offset} and upper bound {upperBoundExclusive}, resulting in an unused item ID: {itemLocation}");
				}
				else if (ItemTracker.Instance.allAssignedMinorItems.ContainsKey(itemLocation))
				{
					var minorItemType = ItemTracker.Instance.allAssignedMinorItems[itemLocation];
					Log.Debug($"Minor item {itemLocation} found to be {minorItemType}");

					ItemTracker.Instance.AddSkipCheck();
					Player.instance.CollectMinorItem(minorItemType);
					Log.Debug($"Minor item {minorItemType} collected from queue");
					NotificationManager.Instance.NotificationQueue.Enqueue($"R:{minorItemType}");
				}
				else
				{
					long majorItemId = itemLocation - ItemTracker.Instance.allAssignedMinorItems.Count;
					if (ItemTracker.Instance.allAssignedMajorItems.ContainsKey(majorItemId))
					{
						//MajorItem itemType = ItemTracker.Instance.allAssignedMajorItems[majorItemId];
						var itemType = GetUnusedMajorItem();
						Log.Debug($"Major item {itemLocation} (modified ID {majorItemId}) randomized to be {itemType}");
						ReceiveMajorItem(itemType);
					}
					else
					{
						Log.Error($"Received location {itemLocation} (modified ID {majorItemId}), which is not in the items collection: {ItemTracker.Instance.allAssignedMinorItems.Count} minors and {ItemTracker.Instance.allAssignedMajorItems.Count} majors.");
						var itemType = GetUnusedMajorItem();
						Log.Debug($"Major item {itemLocation} (modified ID {majorItemId}) randomized to be {itemType}");
						ReceiveMajorItem(itemType);
					}
				}
			}
		}

		private void ReceiveMajorItem(MajorItem itemType)
		{
			//If we're receiving an activated item, drop the current one and give the new one to the player
			if (References.ActivatedItemList.Contains(itemType) && Player.instance.activatedItem)
			{
				var currentRoom = LayoutManager.CurrentRoom;
				Player.instance.DropEquippedActiveItem(Player.instance.transform.position, currentRoom);
			}

			ItemTracker.Instance.AddSkipCheck();
			Player.instance.CollectMajorItem(itemType);
			Log.Debug($"Major item {itemType} collected from queue");
			NotificationManager.Instance.NotificationQueue.Enqueue($"R:{itemType}");
		}

		public void Dispose()
		{

		}
	}
}
