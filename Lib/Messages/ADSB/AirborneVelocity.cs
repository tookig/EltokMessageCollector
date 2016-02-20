using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class AirborneVelocity : ADSBBase {
		public enum SpeedFormats { IAS=0x0, TAS=0x1, GND=0x2 }

		public bool DirectionAvailable { get; protected set; }
		public int Direction { get; protected set; }
		public int Speed { get; protected set; }
		public int NSSpeed { get; protected set; }
		public int EWSpeed { get; protected set; }
		public SpeedFormats SpeedType { get; protected set; }
		public int VerticalRate { get; protected set; }

		public AirborneVelocity(byte[] id, byte[] source) : base(id, source) {
			if ((this.SubtypeSource == 0x01) || (this.SubtypeSource == 0x02)) {
				this.EWSpeed = (((source[1] & 0x03) << 8) | source[2]) * ((source[1] & 0x04) == 0 ? 1 : -1);
				this.NSSpeed = (((source[3] & 0x7F) << 3) | ((source[4] & 0xE0) >> 5)) * ((source[3] & 0x80) == 0 ? 1 : -1);
				if (this.SubtypeSource == 0x02) {
					// Supersonic
					this.EWSpeed *= 4;
					this.NSSpeed *= 4;
				}
				this.Speed = (int)Math.Round(Math.Sqrt(this.EWSpeed * this.EWSpeed + this.NSSpeed * this.NSSpeed));
				this.Direction = ((int)(90.0 - 180.0 / Math.PI * Math.Atan2(this.NSSpeed, this.EWSpeed) + 720.0 - 1.0) % 360) + 1;
				this.SpeedType = SpeedFormats.GND;
				this.DirectionAvailable = true;
			}
			else if ((this.SubtypeSource == 0x03) || (this.SubtypeSource == 0x04)) {
				this.DirectionAvailable = ((source[1] & 0x04) != 0);
				if (this.DirectionAvailable) {
					this.Direction = (int)Math.Round((double)(((source[1] & 0x03) << 8) | source[2]) * 0.3515625);
				}
				this.SpeedType = (SpeedFormats)((source[3] & 0x80) >> 7);
				this.Speed = ((source[3] & 0x7F) << 3) | (source[4] >> 5);
				if (this.SubtypeSource == 0x04) {
					this.Speed *= 4;
				}
			}
			this.VerticalRate = ((((source[4] & 0x07) << 6) | (source[5] >> 2)) * 64 - 64) * ((source[4] & 0x08) == 0 ? 1 : -1);
		}

		public override string ToString() {
			return string.Format("{0} Heading ({2}): {1} Speed ({4}): {3} Vert-rate: {5}", base.ToString(), 
				this.Direction, (this.SpeedType == SpeedFormats.GND ? "GND" : "MAG"), 
				this.Speed, this.SpeedType.ToString(),
				this.VerticalRate);
		}
	}
}

