using Archipelago.ARobotNamedFight.Handlers;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Archipelago.ARobotNamedFight
{
	class ArchipelagoClient : IDisposable
	{
		public const long LocationsStartID = 73310000;
		public Dictionary<GameMode, int> LocationIDOffsetPerGameMode = new Dictionary<GameMode, int>()
		{
			{ GameMode.Normal, 0 },
			{ GameMode.ClassicBossRush, 50 },
		};
		public string ConfigurationFileName = null;
		private ArchipelagoConfiguration _configuration = null;
		private static object _confLock = new object();
		public ArchipelagoConfiguration Configuration
		{
			get
			{
				lock (_confLock)
				{
					if (_configuration == null)
					{
						Log.Debug($"Loading configuration XML");
						if (ConfigurationFileName == null)
						{
							ConfigurationFileName = Path.Combine(Environment.CurrentDirectory, "apconfig.xml");
						}

						Log.Debug($"Config file name: {ConfigurationFileName}");

						if (File.Exists(ConfigurationFileName))
						{
							Log.Debug("Config file exists");

							try
							{
								var serializer = new XmlSerializer(typeof(ArchipelagoConfiguration));
								using (var reader = XmlReader.Create(ConfigurationFileName))
								{
									_configuration = (ArchipelagoConfiguration)serializer.Deserialize(reader);
								}
							}
							catch (Exception ex)
							{
								Log.Error($"Exception thrown while loading config file. Example XML will be generated to _configuration.xml.  Exception: {ex.ToString()}");
								_configuration = new ArchipelagoConfiguration();
								var serializer = new XmlSerializer(_configuration.GetType());
								using (var writer = XmlWriter.Create("_configuration.xml"))
								{
									serializer.Serialize(writer, _configuration);
								}
								_configuration = new ArchipelagoConfiguration();
							}
						}
						else
						{
							Log.Warning($"Archipelago config file {ConfigurationFileName} not found.");
							_configuration = new ArchipelagoConfiguration();
						}
					}
				}
				return _configuration;
			}
		}

		private static Lazy<ArchipelagoClient> lazy = new Lazy<ArchipelagoClient>(() => new ArchipelagoClient());

		private ArchipelagoClient() { }

		public static ArchipelagoClient Instance { get { return lazy.Value; } }

		public delegate void ClientDisconnected(string reason);

		private ArchipelagoSession session;

		public string connectedPlayerName;

		public DeathLinkService deathLinkService;

		//internal DeathLinkHandler DeathLinkHandler { get; private set; }

		public event ClientDisconnected OnClientDisconnect;

		public void Connect(string url, string slot, string pass = null)
		{
			if (session != null && session.Socket.Connected)
			{
				return;
			}

			Log.Debug($"Connect to Archipelago at {url}.");

			ItemTracker.Instance.RefreshItemTracker();

			session = ArchipelagoSessionFactory.CreateSession(url);
			LoginResult loginResult;
			try
			{
				loginResult = session.TryConnectAndLogin("A Robot Named Fight!", slot, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);
			}
			catch (Exception e)
			{
				loginResult = new LoginFailure(e.GetBaseException().Message);
			}

			if (!loginResult.Successful)
			{
				LoginFailure failure = (LoginFailure)loginResult;
				string errorMessage = $"Failed to Connect to {url} as {slot}:";
				foreach (string error in failure.Errors)
				{
					errorMessage += $"\n	{error}";
				}
				foreach (ConnectionRefusedError error in failure.ErrorCodes)
				{
					errorMessage += $"\n	{error}";
				}
				Log.Debug($"Failed to connect: {errorMessage}");
				Dispose();
				return;
			}

			LoginSuccessful loginSuccessful = (LoginSuccessful)loginResult;

			foreach (var slotD in loginSuccessful.SlotData)
			{
				Log.Debug($"Slot data '{slotD.Key}' is '{slotD.Value}'");
			}

			Log.Debug("Starting DeathLink service");
			deathLinkService = session.CreateDeathLinkService();
			deathLinkService.OnDeathLinkReceived += (deathLinkObject) => {
				Player.instance.HandleDamage(9999);
			};

			connectedPlayerName = session.Players.GetPlayerName(session.ConnectionInfo.Slot);
			Log.Debug($"Connected as {connectedPlayerName}");

			Log.Debug("Starting event hookups");
			session.Socket.PacketReceived += Session_PacketReceived;
			session.MessageLog.OnMessageReceived += Session_OnMessageReceived;
			session.Socket.SocketClosed += Session_SocketClosed;
			
			//WIP: Need some way to know which items Fight already has so that when connecting to a restarted server, his stats don't explode.
			//	 Resetting to his base stats is no good, because champ enemies or shrines might have given minor/major items unaccounted for!
			//	 Also can't ignore messages on start, in case player left and returned with checks sent in between.
			session.Items.ItemReceived += (receivedItemsHelper) => {
				try
				{
					var item = receivedItemsHelper.DequeueItem();
					string itemName = session.Items.GetItemName(item.Item);
					Log.Debug($"Item received from AP Server! Name: {itemName}. ID: {item.Item}.  Location: {item.Location}.  Flags: {item.Flags}.  Player: {item.Player}.");

					//CollectReceivedItem(item.Location);
					ItemTracker.Instance.ReceiptQueue.Enqueue(new KeyValuePair<long, string>(item.Item - LocationsStartID, itemName));
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			};

			Log.Debug("Event hookups completed");
		}

		public string ScoutCheckLocationName(long checkNumber)
		{
			try
			{
				//long nextCheck = ItemTracker.Instance.NextCheckNumber;
				//var locationInfo = session?.Locations?.ScoutLocationsAsync(nextCheck).Result;
				string locationName = session.Locations.GetLocationNameFromId(checkNumber + LocationsStartID);

				//Log.Debug($"Next scout location from {nextCheck} count: {locationInfo.Locations.Length}");

				//foreach (var location in locationInfo.Locations)
				//{
				//	Log.Debug($"Scouting found item #{location.Item} for {location.Player} at {location.Location} with flags {location.Flags}.");
				//}

				//return locationInfo;

				return locationName;
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}

			return null;
		}

		public void SendCheck(long localCheckPosition)
		{
			try
			{
				GameMode gameMode = SaveGameManager.activeSlot.activeGameData.gameMode;
				long offset = 0;
				if (LocationIDOffsetPerGameMode.ContainsKey(gameMode)) offset = LocationIDOffsetPerGameMode[gameMode];
				long finalCheckPosition = localCheckPosition + LocationsStartID + offset;
				string locationName = session.Locations.GetLocationNameFromId(finalCheckPosition);
				Log.Debug($"In SendCheck for {localCheckPosition} / {finalCheckPosition}, name {locationName}");
				NotificationManager.Instance.NotificationQueue.Enqueue($"S:{locationName}");
				session.Locations.CompleteLocationChecks(finalCheckPosition);
				//ItemTracker.Instance.NextCheckNumber = ItemTracker.Instance.NextCheckNumber + 1;
				//session.DataStorage["NextCheckNumber"] = ItemTracker.Instance.NextCheckNumber;
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}

		public void RunCompleted(GameMode gameMode)
		{
			Log.Debug($"In RunCompleted for game mode {gameMode}");
			List<GameMode> handledGameModes = new List<GameMode>() { GameMode.Normal, GameMode.ClassicBossRush };
			if (handledGameModes.Contains(gameMode))
			{
				if (ItemTracker.Instance.TotalLocationsInCurrentGame < ItemTracker.Instance.TotalLocationsExpectedForGameMode(gameMode))
				{
					int totalDifference = ItemTracker.Instance.TotalLocationsExpectedForGameMode(gameMode) - ItemTracker.Instance.TotalLocationsInCurrentGame;
					for (int i=0; i < totalDifference; i++)
					{
						ArchipelagoClient.Instance.SendCheck(ItemTracker.Instance.TotalLocationsInCurrentGame + i);
					}
				}

				//string dataStorageKey = $"{gameMode}RunsCompletedCount";
	//			Log.Debug($"dataStorageKey = {dataStorageKey}");
				//string runsCompletedDataStorage = TryGetDataStorage(dataStorageKey);

				//Log.Debug($"runsCompletedDataStorage = {runsCompletedDataStorage}");
				//int runsCompleted = 0;
				//if (!string.IsNullOrEmpty(runsCompletedDataStorage))
				//{
				//	int.TryParse(runsCompletedDataStorage, out runsCompleted);
				//}
				//runsCompleted++;

				//Log.Debug($"New runsCompleted value = {runsCompleted}");
				//SetDataStorage(dataStorageKey, runsCompleted.ToString());
				
	//			Log.Debug("All done");
			}
		}

		public void SetDataStorage(string key, string value)
		{
			Log.Debug($"SetDataStorage is disabled");
			//session.DataStorage[Scope.Slot, key] = value;
			//Log.Debug($"session.DataStorage[Scope.Slot, key] = value has been run");
		}

		public string TryGetDataStorage(string key)
		{
			Log.Debug($"TryGetDataStorage is disabled");
			return null;
			//string value = null;

   //		 try
   //		 {
   //			 value = session.DataStorage[Scope.Slot, key];
   //		 }
   //		 catch (Exception ex)
   //		 {
   //			 Log.Error($"Error in TryGetDataStorage: {ex}");
   //		 }

   //		 return value;
		}

		public void EnqueueUncollectedReceivedItems()
		{
			//TODOs:
			// - When starting a new game, remove items from the map that have already been collected in previous runs (by location number, add minor IDs to activeGame.minorItemIdsCollected?)
			Log.Debug($"In CollectAllReceivedItems");
			Log.Debug($"--- {session.Items.AllItemsReceived.Count} items to receive");
			foreach (var item in session.Items.AllItemsReceived)
			{
				string itemName = session.Items.GetItemName(item.Item);
				ItemTracker.Instance.ReceiptQueue.Enqueue(new KeyValuePair<long, string>((int)item.Item - LocationsStartID, itemName));
			}
		}

		private void Session_PacketReceived(ArchipelagoPacketBase packet)
		{
			ArchipelagoPacketType packetType = packet.PacketType;
			ArchipelagoPacketType archipelagoPacketType = packetType;
			if (archipelagoPacketType != ArchipelagoPacketType.Connected)
			{
				return;
			}

			ConnectedPacket connectedPacket = packet as ConnectedPacket;
			object value;
			bool flag = !connectedPacket.SlotData.TryGetValue("goal", out value) || !Convert.ToBoolean(value);

			var TotalChecks = connectedPacket.LocationsChecked.Count() + connectedPacket.MissingChecks.Count();
			var ChecksTogether = connectedPacket.LocationsChecked.Concat(connectedPacket.MissingChecks).ToArray();
			ChecksTogether = ChecksTogether.OrderBy((long n) => n).ToArray();
			var MissingChecks = connectedPacket.MissingChecks;
			Log.Debug($"Missing Checks {connectedPacket.MissingChecks.Count()} totalChecks {TotalChecks} Locations Checked {connectedPacket.LocationsChecked.Count()} ChecksTogether {ChecksTogether}");
		}

		private void Session_SocketClosed(string reason)
		{
			Dispose();
			//NetMessageExtensions.Send((INetMessage)(object)new ArchipelagoEndMessage(), (NetworkDestination)1);
			if (this.OnClientDisconnect != null)
			{
				this.OnClientDisconnect(reason);
			}
		}

		private void Session_OnMessageReceived(LogMessage message)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				Session_OnMessageReceived_Thread(message);
			});
			thread.Start();
			Thread.Sleep(20);
		}

		private void Session_OnMessageReceived_Thread(LogMessage message)
		{
			Log.Message(String.Join(Environment.NewLine, message.Parts.Select(mp => mp.ToString()).ToList()));
		}

		public void Dispose()
		{
			session = null;
		}
	}
}
