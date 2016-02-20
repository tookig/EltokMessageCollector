using System;
using System.IO;
namespace Eltok.MessageCollector.Lib {
	public class Logger {

		public static Logger UnknownLogger;
		public static void LogUnknown(string text) {
			if (UnknownLogger == null) {
				UnknownLogger = new Logger("/home/tokig/ACARS/unknown.txt");
			}
			UnknownLogger.Log(text);
		}




		public string Filename { get; set; }

		public Logger(string filename) {
			this.Filename = filename;
		}

		public void Log(string text) {
			using (StreamWriter stream = new StreamWriter(this.Filename, true)) {
				stream.WriteLine(text);
			}	
		}
	}
}

