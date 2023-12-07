using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(AchievementScreen), nameof(AchievementScreen.Show))]
	class AchievementScreen_Show_Patch
	{
		static bool Prefix(AchievementInfo achievementInfo)
		{
			NotificationManager.Instance.NotificationQueue.Enqueue($"A:{achievementInfo.name}");

			return false;
		}
	}
}
