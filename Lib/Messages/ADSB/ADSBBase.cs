using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class ADSBBase {
		public static char[] ADSBCharTable = { 	
			'?', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
			'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
			'X', 'Y', 'Z', '?', '?', '?', '?', '?', ' ', '?', '?', '?', '?',
			'?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '0',
			'1', '2', '3', '4', '5', '6', '7', '8', '9', '?', '?',
			'?', '?', '?', '?'
		};

		public byte[] ID { get; private set; }
		public byte[] Source { get; private set; }
		public byte TypeSource { get; private set; }
		public byte SubtypeSource { get; private set; }

		public ADSBBase(byte[] id, byte[] data) {
			this.ID = id;
			this.Source = data;
			this.TypeSource = (byte)(this.Source[0] >> 3);
			this.SubtypeSource = (byte)(this.Source[0] & 0x07);
		}

		public static ADSBBase GetADSBMessage(byte[] id, byte[] adsbsource) {
			int adsbtype = adsbsource[0] >> 3;
			if (adsbtype <= 0x00) {
			}
			else if (adsbtype <= 0x04) {
				return new Identification(id, adsbsource);
			}
			else if (adsbtype <= 0x08) {
				return new SurfacePosition(id, adsbsource);
			}
			else if (adsbtype <= 0x12) {
				return new AirbornePosition(id, adsbsource);
			}
			else if (adsbtype == 0x13) {
				return new AirborneVelocity(id, adsbsource);
			}
			else if (adsbtype <= 0x16) {
				return new AirbornePosition(id, adsbsource);
			}
			else if (adsbtype == 0x1F) {
				return new OperationalStatus(id, adsbsource);
			}
			return new Unknown(id, adsbsource);
		}

		public override string ToString() {
			return string.Format("ADSB-type: {0}", BitConverter.ToString(new byte[] { this.TypeSource }));
		}
	}
}

