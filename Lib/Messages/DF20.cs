using System;
namespace Eltok.MessageCollector.Lib {
	public class DF20 : ModeS {
		public const byte FormatCode = 0x14;

		public FlightStatus FlightStatus { get; protected set; }
		public DownlinkRequest DownlinkRequest { get; protected set; }
		public Altitude Altitude { get; protected set; }

		public DF20(byte[] data) : base(data) {
			// Get flight status
			this.FlightStatus = FlightStatus.Unknown;
			if ((this.CapabilitySource == 0) || (this.CapabilitySource == 2) || (this.CapabilitySource == 4) || (this.CapabilitySource == 5)) {
				this.FlightStatus |= FlightStatus.Airborne;
			}
			if ((this.CapabilitySource == 1) || (this.CapabilitySource == 3) || (this.CapabilitySource == 4) || (this.CapabilitySource == 5)) {
				this.FlightStatus |= FlightStatus.OnGround;
			}
			if ((this.CapabilitySource == 2) || (this.CapabilitySource == 3) || (this.CapabilitySource == 4)) {
				this.FlightStatus |= FlightStatus.Alert;
			}

			// Get Downlink request
			this.DownlinkRequest = DownlinkRequest.None;
			int dr = data[1] >> 3;
			switch (dr) {
			case 1:
				this.DownlinkRequest = DownlinkRequest.CommBRequest;
				break;
			case 2:
			case 3:
			case 6:
			case 7:
				this.DownlinkRequest = DownlinkRequest.ACAS;
				break;
			case 4:
				this.DownlinkRequest = DownlinkRequest.CommBMessage1;
				break;
			case 5:
				this.DownlinkRequest = DownlinkRequest.CommBMessage2;
				break;
			}
			if (dr >= 16)
				this.DownlinkRequest = DownlinkRequest.ELM;

			// Get altitude
			int alt = ((data[2] & 0x1F) << 8) | data[3];
			this.Altitude = ModeS.DecodeAltitude(alt, 13);
		}

		public override string ToString() {
			return string.Format("DF20: {0}{1}{2}{3} {4} Altitude: {5}", 
				base.ToString(),
				this.FlightStatus.IsAirborne() ? " Airborne" : "",
				this.FlightStatus.IsOnGround() ? " On ground" : "",
				this.FlightStatus.IsAlert()    ? " Alert!" : "",
				this.DownlinkRequest != DownlinkRequest.None ? this.DownlinkRequest.ToString() : "",
				this.Altitude
			);
		}
	}
}

