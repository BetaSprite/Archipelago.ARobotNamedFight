using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
	public class ServerSettings
	{
		public bool NormalIncluded { get; set; }
		public bool ClassicBossRushIncluded { get; set; }
		public bool SpookyIncluded { get; set; }
		public bool MirrorWorldIncluded { get; set; }
		public bool MegaMapIncluded { get; set; }
		public bool ExterminatorIncluded { get; set; }
		public GrantAchievementsMode GrantAchievements { get; set; }
		public bool StartWithExplorb { get; set; }
		public bool DeathLink {  get; set; }
	}

	public enum GrantAchievementsMode
	{
		None = 0,
		Necessary = 1,
		All = 2,
	}
}
