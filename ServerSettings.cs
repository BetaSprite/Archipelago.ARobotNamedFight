using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
	public class ServerSettings
	{
		public GameMode GameMode { get; set; }
		public GrantAchievementsMode GrantAchievements { get; set; }
		public bool StartWithExplorb { get; set; }
		public bool StartWithWallJump { get; set; }
		public bool DeathLink {  get; set; }
	}

	public enum GrantAchievementsMode
	{
		None = 0,
		Necessary = 1,
		All = 2,
	}
}
