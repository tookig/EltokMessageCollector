using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class SurfacePosition : ADSBBase {

		public static Position ReceiverLocation = new Position(55.53601,13.3532111);

		/// <summary>
		/// Speed in knots
		/// </summary>
		/// <value>The speed.</value>
		public double Speed { get; protected set; }
		public enum SpeedFlags { Invalid, Precise, Exceeds };
		public SpeedFlags SpeedValid { get; protected set; }

		/// <summary>
		/// Ground track
		/// </summary>
		/// <value>The track.</value>
		public double Track { get; protected set; }
		public bool TrackValid { get; protected set; }

		/// <summary>
		/// Aircraft position
		/// </summary>
		/// <value>The local location.</value>
		public Position LocalLocation { get; protected set; }


		public SurfacePosition(byte[] id, byte[] adsbsource) : base(id, adsbsource)  {
			// Movement speed
			int s = ((adsbsource[0] & 0x07) << 4) | ((adsbsource[1] & 0xF0) >> 4);
			SpeedValid = SpeedFlags.Precise;
			if (s < 1) {
				SpeedValid = SpeedFlags.Invalid;
			}
			else if (s == 1) {
				Speed = 0.0;
			}
			else if (s <= 8) {
				Speed = 0.125 + (double)(s - 2) * 0.125;
			}
			else if (s <= 12) {
				Speed = 1.0 + (double)(s - 9) * 0.25;
			}
			else if (s <= 38) {
				Speed = 2.0 + (double)(s - 13) * 0.5;
			}
			else if (s <= 93) {
				Speed = 15.0 + (double)(s - 39);
			}
			else if (s <= 108) {
				Speed = 70.0 + (double)(s - 94) * 2.0;
			}
			else if (s <= 123) {
				Speed = 100.0 + (double)(s - 109) * 5.0;
			}
			else if (s == 124) {
				Speed = 175;
				SpeedValid = SpeedFlags.Exceeds;
			}
			else {
				SpeedValid = SpeedFlags.Invalid;
			}
			// Ground track
			if ((adsbsource[1] & 0x08) == 0) {
				TrackValid = false;
			}
			else {
				TrackValid = true;
				int t = ((adsbsource[1] & 0x07) << 4) | ((adsbsource[2] & 0xF0) >> 4);
				Track = (double)t * 2.8125;
			}
			// Lat/lon
			int i = (adsbsource[2] & 0x04) >> 2;
			int YZi = ((adsbsource[2] & 0x03) << 15) | (adsbsource[3] << 7) | (adsbsource[4] >> 1);
			int XZi = ((adsbsource[4] & 0x01) << 16) | (adsbsource[5] << 8) | adsbsource[6];
			this.LocalLocation = CPR.LocalSurfaceCPR(ReceiverLocation, i, YZi, XZi);
		}

		public override string ToString() {
			string s = base.ToString();
			if (SpeedValid != SpeedFlags.Invalid) {
				s += string.Format(" Speed: {0}{1}", (SpeedValid == SpeedFlags.Exceeds) ? ">" : "", Speed);
			}
			if (TrackValid) {
				s += string.Format(" Ground Track: {0}", Track);
			}
			s += String.Format(" Local Location: ", this.LocalLocation);
			return s;
		}
	}
}

