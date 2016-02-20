using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class AirbornePosition : ADSBBase {

		public static Position ReceiverLocation = new Position(55.53601,13.3532111);

		public Altitude Altitude { get; protected set; }
		public Position LocalLocation { get; protected set; }
		public Position GlobalLocation { get; protected set; }

		public AirbornePosition(byte[] id, byte[] adsbsource) : base(id, adsbsource)  {
			// Altitude (not converting 100ft Mod-C values atm)
			int alt = (adsbsource[1] << 4) | (adsbsource[2] >> 4);
			this.Altitude = ModeS.DecodeAltitude(alt, 12);
			//int q = adsbsource[1] & 0x01;
			//if (q == 1) {
			//	this.Altitude = (((adsbsource[1] & 0xFE) << 3) | (adsbsource[2] >> 4)) * 25 - 1000;
			//}
			// Lat/lon
			int i = (adsbsource[2] & 0x04) >> 2;
			int YZi = ((adsbsource[2] & 0x03) << 15) | (adsbsource[3] << 7) | (adsbsource[4] >> 1);
			int XZi = ((adsbsource[4] & 0x01) << 16) | (adsbsource[5] << 8) | adsbsource[6];
			this.LocalLocation = CPR.LocalAirborneCPR(ReceiverLocation, i, 17, YZi, XZi);
			this.GlobalLocation = CPR.GlobalAirborneCPR(base.ID, i, 17, YZi, XZi);
		}

		public override string ToString() {
			return string.Format("{0} Altitude: {1} Local Location: {2} Global location: {3}", base.ToString(), this.Altitude, this.LocalLocation, this.GlobalLocation);
		}
	}
}

