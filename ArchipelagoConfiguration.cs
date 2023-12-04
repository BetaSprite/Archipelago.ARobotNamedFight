using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace Archipelago.ARobotNamedFight
{
    public class ArchipelagoConfiguration
    {
        public bool SkipAchievementsMessage = true;
        public bool SendMinorItemDropsAsChecks = false;
        public bool SkipItemCollectScreenPopups = false;
        //public bool RespinEnabled = true;
        public bool StartWithExplorb = true;

        //public int MinorItemSkipCount = 0;
        //public int MajorItemSkipCount = 0;

#if DEBUG
        public bool GodMode = false;
#endif

        public string url = "archipelago.gg:38281";
        public string slot = "";
        public string pass = null;
    }
}
