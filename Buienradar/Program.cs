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
using System.Linq.Expressions;

namespace Buienradar
{
    public class Program
    {
	    private const bool Debug = false;
	    private const int DefaultInterval = 5;
        public static void Main(string[] args)
        {
	        if (Debug)
	        {
		        args = new [] {"-i", "5", "-s", "01-05-2017", "-e", "05-05-2017", "-o", @"C:\Users\theus\Desktop\Radarbeelden"};
	        }
	        DateTime? start = null;
	        DateTime? end = null;
	        int interval = DefaultInterval;
	        string location = null;
            for (int i = 0; i < args.Length; i++)
            {
	            try
	            {
		            switch (args[i])
		            {
			            case "-s":
				            start = DateTime.Parse(args[i + 1]);
				            break;

			            case "-e":
				            end = DateTime.Parse(args[i + 1]);
				            break;

			            case "-o":
				            location = args[i + 1];
				            break;

			            case "-i":
				            if (!int.TryParse(args[i + 1], out interval))
					            throw new ArgumentException("Expected value of type int but got: " + args[i + 1]);
				            break;

			            case "-h":
			            case "-help":
				            Console.WriteLine("Parameters: [starttime -s] [endtime -e] [location -o] [interval -i]");
				            Environment.Exit(Environment.ExitCode);
				            break;

			            default:
				            throw new ArgumentException("Unknown parameter: " + args[i]);
		            }

	            }
	            catch (IndexOutOfRangeException e)
	            {
		            Error(e, true, "All parameters must have a value!");
	            }
	            catch (ArgumentException e)
	            {
		            Error(e, true);
	            }

				// Skip the content of te parameter
	            i++;
            }

	        if (start == null || end == null || location == null)
		        Error(new ArgumentNullException("Not all the required parameters are given."), true);

	        if (interval % 5 != 0)
		        Error(new ArgumentException("Interval must be divisible by 5."), true);

	        RadarImageGetter radar = new RadarImageGetter(start.Value, end.Value);
	        radar.Error +=
		        (sender, exception) => Error(exception, false, "Error occured while getting a radar image.");

            foreach (RadarImage image in radar)
            {
	            if (image == null)
		            continue;
	            using (image)
	            {
					image.Save(location);
		            Console.WriteLine("Saved: " + image.RadarTime.ToString());
	            }
            }
        }

	    private static void Error(Exception e, bool exit, string extraMessage = null)
	    {
		    Console.WriteLine($"Error occured in: {e.Source}");
			Console.WriteLine($"Message: {e.Message}");
		    if (extraMessage != null)
			    Console.WriteLine($"Extra message: {extraMessage}");
		    if (exit)
			    Environment.Exit(-1);
	    }
    }

    public static class Extensions
    {
        public static string PasteDateTime(this DateTime time)
        {
            string year, month, day, hour, minutes;
            year = time.Year.ToString();
            month = time.Month.ToString("00");
            day = time.Day.ToString("00");
            hour = time.Hour.ToString("00");
            minutes = time.Minute.ToString("00");
            return string.Format("{0}{1}{2}{3}{4}", year, month, day, hour, minutes);
        }
    }




}
