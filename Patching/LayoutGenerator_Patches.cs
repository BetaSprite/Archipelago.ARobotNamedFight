using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(LayoutGenerator), nameof(LayoutGenerator.PopulateLayout))]
	class LayoutGenerator_PopulateLayout_Patch
	{
		static void Postfix(ref RoomLayout ____layout)
		{
			if (____layout == null)
			{
				Log.Error("No ____layout found in LayoutGenerator_PopulateLayout_Patch.Postfix");
			}


		}
	}
}
