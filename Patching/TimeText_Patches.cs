using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Archipelago.ARobotNamedFight.Patching
{
	[HarmonyPatch(typeof(TimeText))]
	[HarmonyPatch("Update")]
	class TimeText_Update_Patch
	{
		static string _lastTimeText = "00:00:00";
		static DateTime _currentOverrideTimestamp = DateTime.MinValue;
		static Color _startingColor = new Color(1f, 1f, 1f);
		static Color _sentColor = new Color(.7f, .2f, .2f);
		static Color _receivedColor = new Color(.2f, .7f, .2f);
		static bool _resized = false;

		static void Postfix(ref Text ____timeText)
		{
			if (____timeText == null)
			{
				Log.Error("No ____timeText found in TimeText_Update_Patch.Update. =/");
			}

			//TODO:  See if I can make the timer cover more space.  Maybe get the RectTransform from the gameobject?
			if (!_resized)
			{
				_resized = true;

				var rectT = ____timeText.GetComponent<RectTransform>();
				Log.Debug($"rectT.rect.width = {rectT.rect.width}; rectT.rect.height = {rectT.rect.height}; rectT.sizeDelta.x = {rectT.sizeDelta.x}; rectT.sizeDelta.y = {rectT.sizeDelta.y}; x = {rectT.rect.position.x}");
				rectT.sizeDelta = new Vector2(200, rectT.sizeDelta.y);
				Log.Debug($"rectT.rect.width = {rectT.rect.width}; rectT.rect.height = {rectT.rect.height}; rectT.sizeDelta.x = {rectT.sizeDelta.x}; rectT.sizeDelta.y = {rectT.sizeDelta.y}; x = {rectT.rect.position.x}");
			}

			ItemTracker.Instance.ConsumeLocationExpendQueue();
			ItemTracker.Instance.ConsumeReceiptQueue();

			//Each second, as long as we're not picking up an item already, consume items from the receipt queue
			if (_lastTimeText != ____timeText.text && !ItemTracker.Instance.InShrineOrShopCollection && !ItemTracker.Instance.SkipSendCheck())
			{
				_lastTimeText = ____timeText.text;

				if (_currentOverrideTimestamp != DateTime.MinValue && _currentOverrideTimestamp.AddSeconds(4) < DateTime.Now)
				{
					_currentOverrideTimestamp = DateTime.MinValue;
					NotificationManager.Instance.NotificationQueue.Dequeue();
				}
			}

			____timeText.color = _startingColor;

			if (NotificationManager.Instance.NotificationQueue.Any())
			{
				if (_currentOverrideTimestamp == DateTime.MinValue) _currentOverrideTimestamp = DateTime.Now;
				____timeText.text = NotificationManager.Instance.NotificationQueue.Peek();

				Color clr = _startingColor;
				if (____timeText.text[0] == 'R')
				{
					clr = _receivedColor;
				}
				else if (____timeText.text[0] == 'S')
				{
					clr = _sentColor;
				}

				//Find the % from one second to the next that we are
				float clrFlux = (((float)DateTime.Now.Millisecond) / 1000f);
				//On off-seconds, invert the brightness pulse
				if (DateTime.Now.Second % 2 == 0)
				{
					clrFlux = 1 - clrFlux;
				}
				//Halve the strength
				clrFlux /= 2;
				//Add half back in for a 0.5 to 1 fluctuation
				clrFlux += 0.5f;
				//Log.Debug($"flux {clrFlux}");
				____timeText.color = new Color(clr.r * clrFlux, clr.g * clrFlux, clr.b * clrFlux);
			}
		}
	}
}
