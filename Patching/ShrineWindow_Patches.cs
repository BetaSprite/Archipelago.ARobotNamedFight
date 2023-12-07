using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(ShrineWindow), nameof(ShrineWindow.DonateScrap))]
	class ShrineWindow_DonateScrap_Patch
	{
		static void Prefix()
		{
			ItemTracker.Instance.InShrineOrShopCollection = true;
		}
		//TODO: before CollectMajorItem, insert a call to force a check skip... somehow?
		//static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		//{
		//	var codes = new List<CodeInstruction>(instructions);

		//	//Log.LogDebug("Attempting to skip the item pickup message box");
		//	//if (codes.Count > 0)
		//	//{
		//	//	codes[0].opcode = OpCodes.Ret;
		//	//}

		//	return codes.AsEnumerable();
		//}
		static void Postfix()
		{
			ItemTracker.Instance.InShrineOrShopCollection = false;
		}
	}
}
