using System;
namespace Eltok.MessageCollector.Lib {

	public enum FlightStatus {
		Airborne = 0x1,
		OnGround = 0x2,
		Alert = 0x4,
		Unknown = 0x0
	}

	public enum DownlinkRequest {
		None = 0x00,
		CommBRequest = 0x01,
		ACAS = 0x02,
		CommBMessage1 = 0x03,
		CommBMessage2 = 0x04,
		ELM = 0x05
	}

	public static class Extensions {
		public static bool IsAirborne(this FlightStatus fs) {
			return (fs & FlightStatus.Airborne) == FlightStatus.Airborne;
		}

		public static bool IsOnGround(this FlightStatus fs) {
			return (fs & FlightStatus.OnGround) == FlightStatus.OnGround;
		}

		public static bool IsAlert(this FlightStatus fs) {
			return (fs & FlightStatus.Alert) == FlightStatus.Alert;
		}
	}

}


