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
        //public bool SendMinorItemDropsAsChecks = false;
        public bool StartWithExplorb = true;

#if DEBUG
        public bool GodMode = true;
#endif

        public string url = "archipelago.gg:38281";
        public string slot = "";
        public string pass = null;
    }
}
