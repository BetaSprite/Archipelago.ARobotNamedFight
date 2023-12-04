using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
    internal static class Log
	{
		internal static ManualLogSource _logSource;

		internal static void Init(ManualLogSource logSource)
		{
			_logSource = logSource;
		}

		internal static void Debug(object data)
		{
			_logSource.LogDebug($"{DateTime.Now}: {data}");
		}

		internal static void Error(object data)
		{
			_logSource.LogError($"{DateTime.Now}: {data}");
		}

		internal static void Fatal(object data)
		{
			_logSource.LogFatal($"{DateTime.Now}: {data}");
		}

		internal static void Info(object data)
		{
			_logSource.LogInfo($"{DateTime.Now}: {data}");
		}

		internal static void Message(object data)
		{
			_logSource.LogMessage($"{DateTime.Now}: {data}");
		}

		internal static void Warning(object data)
		{
			_logSource.LogWarning($"{DateTime.Now}: {data}");
		}
	}
}
