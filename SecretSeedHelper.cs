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
			Log.Debug($"Adding necessary secret seeds: {ArchipelagoClient.Instance.SlotServerSettings.MirrorWorldIncluded} {ArchipelagoClient.Instance.SlotServerSettings.SpookyIncluded} {ArchipelagoClient.Instance.SlotServerSettings.ClassicBossRushIncluded}");
			if (SaveGameManager.activeSlot != null)
			{
				if (ArchipelagoClient.Instance.SlotServerSettings.MirrorWorldIncluded)
					SaveGameManager.activeSlot.AddSeedToSecretSeeds(SeedHelper.MirrorWorld);

				if (ArchipelagoClient.Instance.SlotServerSettings.SpookyIncluded)
					SaveGameManager.activeSlot.AddSeedToSecretSeeds(SeedHelper.Spooky);

				if (ArchipelagoClient.Instance.SlotServerSettings.ClassicBossRushIncluded)
					SaveGameManager.activeSlot.AddSeedToSecretSeeds(SeedHelper.ClassicBossRush);
			}
		}
	}
}
