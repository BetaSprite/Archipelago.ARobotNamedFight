using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
    public sealed class ItemTracker : IDisposable
    {
        private static Lazy<ItemTracker> lazy = new Lazy<ItemTracker>(() => new ItemTracker());

        private ItemTracker() { }

        public static ItemTracker Instance { get { return lazy.Value; } }

        private bool NeedsLoad = true;

        public bool NeedsNewGameItems { get; set; } = false;

        //public long NextCheckNumber = 1;

        private int PickingUpSkipCheck = 0;

        public int LastPickedMinorItemGlobal = -99;

        public bool InShrineOrShopCollection = false;

        //public Dictionary<MinorItemType, int> MinorItemsCollected { get; private set; } = new Dictionary<MinorItemType, int>();

        public Dictionary<MinorItemType, int> CheckIgnores { get; private set; } = new Dictionary<MinorItemType, int>();

        public Dictionary<MinorItemType, int> CheckForces { get; private set; } = new Dictionary<MinorItemType, int>();

        public Dictionary<long, MinorItemType> allAssignedMinorItems { get; private set; } = new Dictionary<long, MinorItemType>();

        public Dictionary<long, MajorItem> allAssignedMajorItems { get; private set; } = new Dictionary<long, MajorItem>();

        public Dictionary<MajorItem, long> allAssignedMajorItemsReverse { get; private set; } = new Dictionary<MajorItem, long>();

        public Queue<KeyValuePair<long, string>> ReceiptQueue { get; private set; } = new Queue<KeyValuePair<long, string>>();


        public void Reset()
        {
            //ItemTracker.Instance.NextCheckNumber = 1;
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

        //public void ItemCollected(MinorItemType minorItemType, bool loading = false)
        //{
        //    Log.Debug($"In ItemCollected for {minorItemType}");
        //    RefreshItemTracker();

        //    lock (CheckIgnores)
        //    {
        //        if (CheckIgnores.ContainsKey(minorItemType) && CheckIgnores[minorItemType] > 0)
        //        {
        //            CheckIgnores[minorItemType]--;
        //            Log.Debug($"Ignored collection of spontaneous item {minorItemType}");
        //        }
        //        else
        //        {
        //            if (!MinorItemsCollected.ContainsKey(minorItemType))
        //            {
        //                MinorItemsCollected.Add(minorItemType, 0);
        //            }

        //            MinorItemsCollected[minorItemType]++;

        //            Log.Debug($"{MinorItemsCollected[minorItemType]} collected towards goal of {ArchipelagoClient.Instance.Configuration.MinorItemSkipCount+1} for item type {minorItemType}");
        //        }
        //    }
        //}

        //public void FinishCheck(MinorItemType minorItemType)
        //{
        //    Log.Debug("In FinishCheck");
        //    RefreshItemTracker();
        //    if (MinorItemsCollected.ContainsKey(minorItemType))
        //    {
        //        ModItem(minorItemType);
        //    }
        //    else
        //    {
        //        Log.Warning($"FinishCheck was called for {minorItemType}, but collection did not contain that type.");
        //    }
        //}

        //public void ModAllItemsCollected()
        //{
        //    Log.Debug("In ModAllItemsCollected");
        //    RefreshItemTracker();
        //    var keys = new List<MinorItemType>(MinorItemsCollected.Keys.ToArray());
        //    foreach (var key in keys)
        //    {
        //        ModItem(key);
        //    }
        //}

   //     public void ModItem(MinorItemType minorItemType)
   //     {
   //         if (!MinorItemsCollected.ContainsKey(minorItemType))
			//{
   //             MinorItemsCollected.Add(minorItemType, 0);
   //         }
   //         Log.Debug($"For {minorItemType}, modding {MinorItemsCollected[minorItemType]} to {MinorItemsCollected[minorItemType] % (ArchipelagoClient.Instance.Configuration.MinorItemSkipCount + 1)}.");
   //         MinorItemsCollected[minorItemType] = MinorItemsCollected[minorItemType] % (ArchipelagoClient.Instance.Configuration.MinorItemSkipCount + 1);
   //         Log.Debug($"ItemsCollected[{minorItemType}] = {MinorItemsCollected[minorItemType]}");
   //     }

        public bool ItemIsAtCheck(MinorItemType minorItemType, bool decrementForce = false)
        {
            Log.Debug($"In ItemIsAtCheck for {minorItemType}");
            RefreshItemTracker();

			//if (CheckIsBeingForced(minorItemType))
			//{
			//    Log.Debug($"Check is being forced for {minorItemType} {CheckForces[minorItemType]} times.  Decrement? {decrementForce}");

			//    if (decrementForce)
			//    {
			//        CheckForces[minorItemType] = CheckForces[minorItemType] - 1;
			//    }

			//    return true;
			//}

            if (ItemTracker.Instance.SkipSendCheck())
			{
                return false;
			}

			if (References.MinorItemBlacklist.Contains(minorItemType))
			{
				return false;
			}

            //if (!MinorItemsCollected.ContainsKey(minorItemType))
            //{
            //	MinorItemsCollected.Add(minorItemType, 0);
            //}

            return true; // (MinorItemsCollected[minorItemType] > ArchipelagoClient.Instance.Configuration.MinorItemSkipCount);
        }

        public bool CheckIsBeingForced(MinorItemType minorItemType)
        {
            Log.Debug($"In CheckIsBeingForced for {minorItemType}");
            RefreshItemTracker();
            return (CheckForces.ContainsKey(minorItemType) && CheckForces[minorItemType] > 0);
        }

        public bool CheckIsBeingIgnored(MinorItemType minorItemType)
        {
            Log.Debug($"In CheckIsBeingIgnored for {minorItemType}");
            RefreshItemTracker();
            return (CheckIgnores.ContainsKey(minorItemType) && CheckIgnores[minorItemType] > 0);
        }

        //public string GetRemainingChecksMessage(MinorItemType minorItemType)
        //{
        //    Log.Debug($"In GetRemainingChecksMessage for {minorItemType}");
        //    RefreshItemTracker();
        //    int remaining = (ArchipelagoClient.Instance.Configuration.MinorItemSkipCount + 1) - MinorItemsCollected[minorItemType];
        //    string s = remaining == 1 ? "" : "s";
        //    return $"Collect {remaining} more {minorItemType}{s} to send an item to another player.";
        //}

        public void AddSkipCheck(int amt = 1)
		{
            PickingUpSkipCheck += amt;
            Log.Debug($"AddSkipCheck. amt = {amt}. Updated PickingUpSkipCheck = {PickingUpSkipCheck}");
        }

        public bool SkipSendCheck(bool consume = false)
        {
            //Log.Debug($"SkipSendCheck. consume = {consume}. PickingUpSkipCheck = {PickingUpSkipCheck}. InShrineOrShopCollection = {InShrineOrShopCollection}.");
            bool bRet = false;
            if (InShrineOrShopCollection) bRet = true;
            if (!bRet)
            {
                bRet = PickingUpSkipCheck > 0;
                if (consume && bRet) PickingUpSkipCheck--;
            }
            return bRet;
		}

        public void AddSingleCheckForce(MinorItemType minorItemType)
        {
            Log.Debug($"In AddSingleCheckForce for {minorItemType}");
            RefreshItemTracker();
            if (CheckForces.ContainsKey(minorItemType))
            {
                CheckForces[minorItemType]++;
            }
            else
            {
                CheckForces.Add(minorItemType, 1);
            }
        }

        public void ReplaceNonProgressionMajorItemInRooms(MajorItem majorItemType)
		{
            Log.Debug($"In ReplaceNonProgressionMajorItemInRooms for {majorItemType}");
            if (!References.MajorItemBlacklist.Contains(majorItemType))
            {
                SaveGameData activeGame = SaveGameManager.activeGame;
                RoomAbstract targetRoom = null;
                List<MajorItem> majorItemsFoundInRooms = new List<MajorItem>();
                foreach (var roomAbstract in activeGame.layout.roomAbstracts)
                {
                    if (!majorItemsFoundInRooms.Contains(roomAbstract.majorItem))
                    {
                        //Gather up the major items that already exist
                        //Log.Debug($"New major item found: {roomAbstract.majorItem}");
                        majorItemsFoundInRooms.Add(roomAbstract.majorItem);

                        if (roomAbstract.majorItem == majorItemType)
						{
                            Log.Debug($"Found room containing {majorItemType}");
                            targetRoom = roomAbstract;
						}
                    }
                }

                if (targetRoom == null)
                {
                    Log.Error($"Couldn't find major item {majorItemType} in rooms!  Must be bonus.");
                }
                else
                {
                    //Find a new, unused major item that isn't blacklisted
                    Array values = Enum.GetValues(typeof(MajorItem));
                    Random random = new Random();
                    MajorItem newItem = (MajorItem)values.GetValue(random.Next(values.Length));
                    Log.Debug($"Attempt to replace item with {newItem}");
                    while (majorItemsFoundInRooms.Contains(newItem) || References.MajorItemBlacklist.Contains(newItem))
                    {
                        newItem = (MajorItem)values.GetValue(random.Next(values.Length));
                        Log.Debug($"Nope, try {newItem} instead?");
                    }

                    //Make the swap
                    Log.Debug($"Swapping major item {targetRoom.majorItem} in map with dummy item {newItem}");
                    targetRoom.majorItem = newItem;

                    Log.Debug($"Swapping item in itemOrder and bonusItemsAdded");
                    if (activeGame.layout.itemOrder.Contains(majorItemType))
					{
                        activeGame.layout.itemOrder.Remove(majorItemType);
                        activeGame.layout.itemOrder.Add(newItem);
                    }
                    if (activeGame.layout.bonusItemsAdded.Contains(majorItemType))
                    {
                        activeGame.layout.bonusItemsAdded.Remove(majorItemType);
                        activeGame.layout.bonusItemsAdded.Add(newItem);
                    }

                    Reset();
                    NeedsLoad = true;
                    RefreshItemTracker();
                }
            }
            else
			{
                Log.Debug($"{majorItemType} is blacklisted, so we won't replace it in the map.");
			}
        }

        public void RefreshItemTracker()
        {
            if (NeedsLoad)
            {
                NeedsLoad = false;
                try
                {
                    Log.Debug("RefreshItemTracker has been triggered");

                    //NextCheckNumber = ArchipelagoClient.Instance.GetNextCheckNumber();

                    //Log.Debug($"GetNextCheckNumber returned {NextCheckNumber}");

                    //RoomAbstracts have the actual in-game items' objects.
                    //activeGame.minorItemIdsCollected can check if the item has been picked up via global ID.
                    //Loop through and identify what's been picked up so that we can pick up where the player left off.
                    //TODO: Replace this with some kind of history retrieval from Archipelago.

                    SaveGameData activeGame = SaveGameManager.activeGame;
                    
                    //Dictionary<int, MinorItemType> visitedRoomMinorItems = new Dictionary<int, MinorItemType>();

                    //Log.Debug($"Loop through the rooms that have been visited. Count: {activeGame.roomsVisited.Count}");
                    //foreach (var roomID in activeGame.roomsVisited)
                    //{
                    //    List<RoomAbstract> roomAbstractsVisited = new List<RoomAbstract>(activeGame.layout.roomAbstracts.Where(x => x.roomID == roomID));
                    //    Log.Debug($"Locating rooms for roomID {roomID}. Count: {roomAbstractsVisited.Count}");
                    //    foreach (var roomAbstract in roomAbstractsVisited)
                    //    {
                    //        Log.Debug($"Loop through this room's items to catalog their types and IDs. Count: {roomAbstract.minorItems.Count}");
                    //        foreach (var item in roomAbstract.minorItems)
                    //        {
                    //            Log.Debug($"Item {item.type} found with global ID {item.globalID}");
                    //            if (!References.MinorItemBlacklist.Contains(item.type))
                    //            {
                    //                visitedRoomMinorItems.Add(item.globalID, item.type);
                    //            }
                    //            else
                    //            {
                    //                Log.Debug($"Item type {item.type} is banned from checks. Ignoring.");
                    //            }
                    //        }
                    //    }
                    //}

                    Log.Debug($"Loop through all rooms. Count: {activeGame.layout.roomAbstracts.Count}");
                    int iMajorItemCounter = 0;
                    foreach (var roomAbstract in activeGame.layout.roomAbstracts)
                    {
                        //Log.Debug($"Loop through this room's minor items to catalog their types and IDs. Count: {roomAbstract.minorItems.Count}");
                        foreach (var item in roomAbstract.minorItems)
                        {
                            Log.Debug($"Item {item.type} found with global ID {item.globalID}");

                            //Increasing the index to start at 1 for count and send logic
                            allAssignedMinorItems.Add(item.globalID, item.type);
                        }

                        if (!References.MajorItemBlacklist.Contains(roomAbstract.majorItem))
                        {
                            iMajorItemCounter++;
                            Log.Debug($"Adding major item {roomAbstract.majorItem} to list with ID {iMajorItemCounter}.");
                            allAssignedMajorItems.Add(iMajorItemCounter, roomAbstract.majorItem);
                            allAssignedMajorItemsReverse.Add(roomAbstract.majorItem, iMajorItemCounter);
                        }
                        else if (roomAbstract.majorItem != MajorItem.None)
                        {
                            Log.Debug($"Ignoring progression major item {roomAbstract.majorItem} due to blacklist.");
                        }
                    }

                    Log.Debug($"Total minor items: {allAssignedMinorItems.Count}");
                    Log.Debug($"Total non-progress major items: {allAssignedMajorItems.Count}");

                    //Log.Debug($"Total visited minor items found: {visitedRoomMinorItems.Count}");

                    //int minorCollectedCount = 0;
                    ////For every item that has been collected, add up their totals in the tracker.
                    //foreach (var collectedId in activeGame.minorItemIdsCollected)
                    //{
                    //    if (visitedRoomMinorItems.ContainsKey(collectedId))
                    //    {
                    //        ItemTracker.Instance.ItemCollected(visitedRoomMinorItems[collectedId], true);
                    //        minorCollectedCount++;
                    //    }
                    //}

                    //Log.Debug($"Total minor items collected count: {minorCollectedCount}");

                    //int majorCollectedCount = 0;
                    //foreach (var collectedId in activeGame.itemsCollected)
                    //{
                    //    majorCollectedCount++;
                    //}

                    //Log.Debug($"Total major items collected count: {majorCollectedCount}");

                    //Log.Debug($"Total collected count: {minorCollectedCount + majorCollectedCount}");

                    //Mod all item counts to get it back to how it was before.
                    //ItemTracker.Instance.ModAllItemsCollected();
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
                    if (ArchipelagoClient.Instance.Configuration.StartWithExplorb)
                    {
                        Log.Debug("Enqueue bonus Explorb collection");
                        ItemTracker.Instance.ReceiptQueue.Enqueue(new KeyValuePair<long, string>(-9999, "BonusExplorb"));
                    }

                    Log.Debug("Needs new game items");
                    ArchipelagoClient.Instance.EnqueueUncollectedReceivedItems();

                    Log.Debug("Enqueued");

     //               Log.Debug("Saving game");
     //               if (SaveGameManager.instance)
					//{
					//	SaveGameManager.instance.Save();
     //               }
     //               Log.Debug("Done");
                }
                catch (NullReferenceException ex)
                {
                    Log.Debug($"NullReferenceException hit in GetNewGameItems...: {ex}");
                    NeedsNewGameItems = true;
                }
            }
        }

        public void ConsumeReceiptQueue()
		{
            if (!Player.instance.paused && Player.instance.enabled&& ReceiptQueue.Any())
            {
                KeyValuePair<long, string> keyValuePair = ReceiptQueue.Dequeue();
                long key = keyValuePair.Key;
                string value = keyValuePair.Value;
                Log.Debug($"ConsumeReceiptQueue: location {key}. name {value}");

                CollectReceivedItem(key);
            }
        }

        public void CollectReceivedItem(long itemLocation)
		{
            Log.Debug($"In CollectReceivedItem for itemId {itemLocation}");
            if (itemLocation == -9999)
			{
                MajorItem itemType = MajorItem.Explorb;
                Log.Debug($"Major item {itemLocation} found to be {itemType}");

                //Prevent the bonus Explorb from messing up the collection percentage
                //SaveGameData activeGame = SaveGameManager.activeGame;
                //Log.Debug($"Increasing total starting major items from {activeGame.layout.totalStartingMajorItems} to {activeGame.layout.totalStartingMajorItems + 1}");
                //activeGame.layout.totalStartingMajorItems++;
                //activeGame.layout.allNonTraversalItemsAdded.Add(itemType);

                ItemTracker.Instance.AddSkipCheck();
                Player.instance.CollectMajorItem(itemType);
                Log.Debug($"Major item {itemType} collected from queue");
                NotificationManager.Instance.NotificationQueue.Enqueue($"R:{itemType}");
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
                    MajorItem itemType = ItemTracker.Instance.allAssignedMajorItems[majorItemId];
                    Log.Debug($"Major item {itemLocation} (modified ID {majorItemId}) found to be {itemType}");

                    //Prevent this major item from messing up the collection percentage
                    //SaveGameData activeGame = SaveGameManager.activeGame;
                    //Log.Debug($"Increasing total starting major items from {activeGame.layout.totalStartingMajorItems} to {activeGame.layout.totalStartingMajorItems + 1}");
                    //activeGame.layout.totalStartingMajorItems++;
                    //activeGame.layout.allNonTraversalItemsAdded.Add(itemType);

                    ItemTracker.Instance.AddSkipCheck();
                    Player.instance.CollectMajorItem(itemType);
                    Log.Debug($"Major item {itemType} collected from queue");
                    NotificationManager.Instance.NotificationQueue.Enqueue($"R:{itemType}");
                }
                else
                {
                    Log.Debug($"Received location {itemLocation}, which is not in the items collection: {ItemTracker.Instance.allAssignedMinorItems.Count} minors and {ItemTracker.Instance.allAssignedMajorItems.Count} majors.");
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
