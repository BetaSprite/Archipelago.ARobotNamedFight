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

			//Seems OK, needs testing
			if (ArchipelagoClient.Instance.SlotServerSettings.GameMode == GameMode.Exterminator)
			{
				GiveAchievement(AchievementID.Exterminator);
			}

			//Error on layout gen
			if (ArchipelagoClient.Instance.SlotServerSettings.GameMode == GameMode.MegaMap)
			{
				GiveAchievement(AchievementID.MegaMap);
				//Add CoolantSeweres, CrystalMines
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
