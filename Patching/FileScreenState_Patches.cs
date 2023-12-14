using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(FileScreenState), nameof(FileScreenState.FileSlotSelected))]
	class FileScreenState_FileSlotSelected_Patch
	{
		static void Postfix()
		{
			try
			{
				ArchipelagoClient.Instance.Connect(ArchipelagoClient.Instance.Configuration.url,
													ArchipelagoClient.Instance.Configuration.slot,
													ArchipelagoClient.Instance.Configuration.pass);
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}
}
