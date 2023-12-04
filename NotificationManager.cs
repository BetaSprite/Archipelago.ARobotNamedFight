using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
	public class NotificationManager : IDisposable
	{
		private static Lazy<NotificationManager> lazy = new Lazy<NotificationManager>(() => new NotificationManager());

		private NotificationManager() { }

		public static NotificationManager Instance { get { return lazy.Value; } }

		public Queue<string> NotificationQueue { get; private set; } = new Queue<string>();

		public void Dispose()
		{

		}
	}
}
