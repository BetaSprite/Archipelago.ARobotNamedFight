using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
	public static class SecretSeedHelper
	{
		public static void AddNecessarySecretSeeds()
		{
			Log.Debug($"Adding necessary secret seeds.");
			if (SaveGameManager.activeSlot != null)
			{
				if (ArchipelagoClient.Instance.SlotServerSettings.GameMode == GameMode.MirrorWorld)
					SaveGameManager.activeSlot.AddSeedToSecretSeeds(SeedHelper.MirrorWorld);

				if (ArchipelagoClient.Instance.SlotServerSettings.GameMode == GameMode.Spooky)
					SaveGameManager.activeSlot.AddSeedToSecretSeeds(SeedHelper.Spooky);

				if (ArchipelagoClient.Instance.SlotServerSettings.GameMode == GameMode.ClassicBossRush)
					SaveGameManager.activeSlot.AddSeedToSecretSeeds(SeedHelper.ClassicBossRush);
			}
		}
	}
}
