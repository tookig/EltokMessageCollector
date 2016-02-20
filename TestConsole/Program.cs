using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using Eltok.MessageCollector.Lib;

namespace TestConsole {
	class MainClass {
		public static bool m_log = false;

		public static void Main(string[] args) {
			string cmd = "";
			int port = 30002;
			Eltok.MessageCollector.Lib.Dump1090Listener listener = null;


			while (cmd != "exit") {
				cmd = Console.ReadLine();
				if (cmd == "connect") {
					if (listener == null) {
						listener = new Eltok.MessageCollector.Lib.Dump1090Listener(port);
						listener.ItemReceived = Incoming;
						listener.Connect();
					}
				}
				else if (cmd == "disconnect") {
					if (listener != null) {
						listener.Disconnect();
						listener = null;
					}
				}
				else if (cmd.StartsWith("nl ")) {
					double lat;
					if (double.TryParse(cmd.Substring(3), out lat)) {
						Console.WriteLine("{0}: NL={1}", lat, CPR.NL(lat));
					}
					else {
						Console.WriteLine("Not a valid latitude");
					}
				}
				else if (cmd.StartsWith("decode ")) {
					string[] msgs = cmd.Substring("decode ".Length).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string msg in msgs) {
						Incoming(msg.Trim());
					}
				}
				else if (cmd == "logger on") {
					m_log = true;
				}
				else if (cmd == "logger off") {
					m_log = false;
				}
			}
			// decode 8D75804B580FF2CF7E9BA6F701D0 8D75804B580FF6B283EB7A157117
			// 8D4001F1583911CFD4077E89BF53
			// 8D4001F15839153FD0074189BF53
			// decode 8D4001F1583911CFD4077E89BF53 8D4001F15839153FD0074189BF53
		}

		public static void Incoming(string s) {
			byte[] data = ModeS.StringToByteArray(s);

			ModeS msg = ModeS.GetMessage(data);

			DF17 df17 = msg as DF17;
			if ((df17 != null)) {
				Console.WriteLine(df17.ToString());
			}

			DF18 df18 = msg as DF18;
			if (df18 != null) {
				Console.WriteLine(df18.ToString());
			}

			DF20 df20 = msg as DF20;
			if (df20 != null) {
				Console.WriteLine(df20.ToString());
			}

			if (m_log) {
				if (msg == null) {
					Logger.LogUnknown(string.Format("Unknown: {0}", s));
				}
				else if ((df17 != null) && ((df17.ADSB is Eltok.MessageCollector.Lib.ADSB.Unknown) || (df17.ADSB == null))) {
					Logger.LogUnknown(string.Format("DF17 unknown: {0}", s));
				}
			}

			//Console.WriteLine("{0}: {1}", s, msg==null?(data[0] >> 3).ToString():msg.ToString());

			//data[0] = (byte)(data[0] >> 3);
			//Console.WriteLine("{0}: {1}", s, BitConverter.ToString(data));
			//Console.WriteLine("{0}:{1}", s, (data[0] & 0x80) == 0x80 ? "long" : "short");
		}
			
	}

}
