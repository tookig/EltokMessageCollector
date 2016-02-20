using System;
namespace Eltok.MessageCollector.Lib {
	public class DF18 : ModeS {

		public const byte FormatCode = 0x12;

		public enum DF18Types {
			ADSBStandardAddress = 0x0,
			//ADSBNonStandardAddress = 0x1,
			//TISBFine = 0x2,
			//TISBCoarse = 0x3,
			//TISBReserved = 0x4,
			//TISBRelayADSB = 0x5,
			//ADSBRebroadcast = 0x6,
			//Reserved = 0x7
			Other = 0x8
		}

		public DF18Types DF18Type { get; protected set; }
		public ADSB.ADSBBase ADSB { get; protected set; }

		public DF18(byte[] data) : base(data) {
			byte[] payload = new byte[7];
			for (int i = 4; i < 11; i++) {
				payload[i - 4] = data[i];
			}
			if ((int)this.CapabilitySource == (int)DF18Types.ADSBStandardAddress) {
				this.DF18Type = DF18Types.ADSBStandardAddress;
				this.ADSB = Eltok.MessageCollector.Lib.ADSB.ADSBBase.GetADSBMessage(base.AircraftID, payload);
			}
			else {
				this.DF18Type = DF18Types.Other;
			}
		}

		public override string ToString() {
			return string.Format("DF18: {0} {1}", base.ToString(), this.ADSB == null ? "" : this.ADSB.ToString());
		}
	}
}

