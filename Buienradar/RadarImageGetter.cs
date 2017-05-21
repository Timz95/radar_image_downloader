using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;

namespace Buienradar
{
	public class RadarImageGetter : IEnumerator, IEnumerable
	{
		private DateTime startTime;
		private DateTime currentTime;
		private DateTime endTime;
		private int interval;
		private const string baseUrl = "http://api.buienradar.nl/image/archive/1.0/radarmapnl/";

		public event EventHandler<Exception> Error;

		public RadarImageGetter(DateTime startTime, DateTime endTime, int interval = 5)
		{
			this.startTime = startTime;
			this.currentTime = startTime - new TimeSpan(0, interval, 0);
			this.endTime = endTime;
			this.interval = interval;
		}

		public object Current
		{
			get
			{
				try
				{
					WebResponse response = WebRequest.Create(CreateRadarUri(currentTime)).GetResponse();
					return new RadarImage(response, currentTime);
				}
				catch (WebException e)
				{
					OnError(e);
					return null;
				}
			}
		}

		public bool MoveNext()
		{
			currentTime += new TimeSpan(0, interval, 0);
			return currentTime < endTime;
		}

		public void Reset()
		{
			this.currentTime = this.startTime;
		}

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		public Uri CreateRadarUri(DateTime time)
		{
			string url = baseUrl + time.PasteDateTime();
			return new Uri(url);
		}

		protected virtual void OnError(Exception e)
		{
			var handler = this.Error;
			handler?.Invoke(this, e);
		}
	}
}
