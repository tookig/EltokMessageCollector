using System;
namespace Eltok.MessageCollector.Lib {
	public class Altitude {
		public enum Units { Feet, Meters }

		public Units Unit { get; protected set; }
		public int Value { get; protected set; }


		public Altitude(int altitude, Units unit = Units.Feet) {
			this.Value = altitude;
			this.Unit = unit;
		}

		public override string ToString() {
			return string.Format("{0} {1}", this.Value, this.Unit == Units.Feet ? "ft" : "m");
		}
	}
}

