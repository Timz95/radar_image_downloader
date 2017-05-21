using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Buienradar
{
	public class RadarImage: IDisposable
	{
		private const int bufferSize = 8192;

		private Stream radarStream;
		private WebResponse radarResponse;
		private string imageFormat;

		public DateTime RadarTime { get; }

	    

		public bool Disposed { get; private set; }

		public RadarImage(Stream radarStream, DateTime time, string imageFormat = ".jpg")
		{
			this.radarStream = radarStream;
			this.RadarTime = time;

		    if (imageFormat == null)
		        imageFormat = string.Empty;
            else if (imageFormat.First() != '.')
		        imageFormat = string.Concat('.', imageFormat);

		    this.imageFormat = imageFormat;
		}

		public RadarImage(WebResponse radarResponse, DateTime time, string imageFormat = ".jpg")
			: this(radarResponse.GetResponseStream(), time, imageFormat)
		{
			this.radarResponse = radarResponse;
		}


		~RadarImage()
		{
			Dispose();
		}

		public void Save(string location)
		{
			if (!(Directory.Exists(location)))
			{
				Directory.CreateDirectory(location);
			}
			location = Path.Combine(location, RadarTime.PasteDateTime() + imageFormat);

			if (File.Exists(location))
				return;

			FileStream stream = new FileStream(location, FileMode.Create);
			byte[] buffer = new byte[bufferSize];
			int count;
			do
			{
				count = radarStream.Read(buffer, 0, buffer.Length);
				if (count != 0)
				{
					stream.Write(buffer, 0, count);
				}
			}
			while (count > 0);
		}

		public void Dispose()
		{
			if (!Disposed)
			{
				radarStream?.Dispose();
				radarResponse?.Dispose();
				GC.SuppressFinalize(this);
				Disposed = true;
			}
		}
	}
}
