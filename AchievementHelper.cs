using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
	public static class AchievementHelper
	{
		public static void AwardNecessaryAchievements()
		{
			Log.Debug("Awarding achievements for extra modes (WIP)");

			var activeSlot = SaveGameManager.activeSlot;

			//Infinite loop on layout gen
			if (ArchipelagoClient.Instance.SlotServerSettings.NewBossRushIncluded)
			{
				Log.Debug("Giving achievements for all bosses and Boss Rush unlock");
				var allBossNames = Enum.GetValues(typeof(BossName)).Cast<BossName>().ToList();
				foreach (BossName bossName in allBossNames)
				{
					var achievement = AchievementManager.instance.GetBossAchievement(bossName);
					GiveAchievement(achievement);
				}
				GiveAchievement(AchievementID.BossRush);
			}

			//Seems OK, needs testing
			if (true || ArchipelagoClient.Instance.SlotServerSettings.ExterminatorIncluded)
			{
				GiveAchievement(AchievementID.Exterminator);
			}

			//Error on layout gen
			if (ArchipelagoClient.Instance.SlotServerSettings.MegaMapIncluded)
			{

				GiveAchievement(AchievementID.MegaMap);
			}
		}

		public static void GiveAchievement(AchievementID achievement)
		{
			if (!SaveGameManager.activeSlot.achievements.Contains(achievement))
				SaveGameManager.activeSlot.achievements.Add(achievement);
		}

		public static void AwardAllAchievements()
		{
			Log.Debug("Awarding all achievements");
			var activeSlot = SaveGameManager.activeSlot;
			var allAchievements = Enum.GetValues(typeof(AchievementID)).Cast<AchievementID>().ToList();
			foreach (var a in allAchievements)
			{
				if (!activeSlot.achievements.Contains(a)) { activeSlot.achievements.Add(a); }
			}
			SaveGameManager.instance.Save();
		}
	}
}
