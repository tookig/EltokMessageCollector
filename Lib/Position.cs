using System;
namespace Eltok.MessageCollector.Lib {
	public class Position {

		public double Lat { get; set; }
		public double Lon { get; set; }

		public Position() {
		}

		public Position(double lat, double lon) {
			Lat = lat;
			Lon = lon;
		}

		public override string ToString() {
			return string.Format("{0:0.0000} {1:0.0000}", this.Lat, this.Lon);
		}
	}
}

