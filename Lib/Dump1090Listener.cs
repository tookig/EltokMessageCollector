using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Eltok.MessageCollector.Lib {
	public class Dump1090Listener : Listener {
		private static Regex incoming_rgx = new Regex("(?<=\\*).*?(?=\\;\n)");
		private static Regex trim_rgx = new Regex("\\*.*\\;\n");

		private string m_incomplete = "";

		public Action<string> ItemReceived = (string s) => {
			Console.WriteLine(s);
		};

		public Dump1090Listener(int port) : base(port) {
		}

		protected override void IncomingData(byte[] data) {
			m_incomplete += Encoding.ASCII.GetString(data, 0, data.Length);
			foreach (Match match in incoming_rgx.Matches(m_incomplete)) {
				this.ItemReceived(match.Value);
			}
			if (!string.IsNullOrEmpty(m_incomplete)) m_incomplete = trim_rgx.Replace(m_incomplete, "");
		}
	}
}

