using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(ShopWindow), nameof(ShopWindow.PurchaseItem))]
	class ShopWindow_PurchaseItem_Patch
	{
		static void Prefix()
		{
			Log.Debug("ShopWindow_PurchaseItem_Patch Prefix");
			ItemTracker.Instance.InShrineOrShopCollection = true;
		}

		//static void Postfix()
		//{
		//	ItemTracker.Instance.InShrineOrShopCollection = false;
		//}
	}
}
