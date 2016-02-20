using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class Identification : ADSBBase {
		public char[] Callsign { get; protected set; }

		public Identification(byte[] id, byte[] adsbsource) : base(id, adsbsource) {
			this.Callsign = new char[8];
			this.Callsign[0] = ADSBBase.ADSBCharTable[adsbsource[1] >> 2];
			this.Callsign[1] = ADSBBase.ADSBCharTable[((adsbsource[1] << 4) & 0x30) | (adsbsource[2] >> 4)];
			this.Callsign[2] = ADSBBase.ADSBCharTable[((adsbsource[2] << 2) & 0x3C) | (adsbsource[3] >> 6)];
			this.Callsign[3] = ADSBBase.ADSBCharTable[adsbsource[3] & 0x3F];
			this.Callsign[4] = ADSBBase.ADSBCharTable[adsbsource[4] >> 2];
			this.Callsign[5] = ADSBBase.ADSBCharTable[((adsbsource[4] << 4) & 0x30) | (adsbsource[5] >> 4)];
			this.Callsign[6] = ADSBBase.ADSBCharTable[((adsbsource[5] << 2) & 0x3C) | (adsbsource[6] >> 6)];
			this.Callsign[7] = ADSBBase.ADSBCharTable[adsbsource[6] & 0x3F];
		}

		public override string ToString() {
			return string.Format("{0} Callsign: {1}", base.ToString(), new string(this.Callsign));
		} 
	}
}

