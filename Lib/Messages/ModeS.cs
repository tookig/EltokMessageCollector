using System;
using System.Linq;

namespace Eltok.MessageCollector.Lib {
	/// <summary>
	/// A Mode S message
	/// </summary>
	public abstract class ModeS {
		/// <summary>
		/// Capability flags.
		/// </summary>
		public enum CapabilityFlags { SurveillanceOnly=0, GICBOnly, Full }

		/// <summary>
		/// Gets the source data.
		/// </summary>
		/// <value>The source data.</value>
		public byte[] SourceData { get; private set; }

		/// <summary>
		/// Gets the aircraft ID.
		/// </summary>
		/// <value>The aircraft ID.</value>
		public byte[] AircraftID { get; private set; }

		/// <summary>
		/// Gets the capability source.
		/// </summary>
		/// <value>The capability source.</value>
		public byte CapabilitySource { 
			get { return (byte)(SourceData[0] & 0x07);	}
		}

		/// <summary>
		/// Gets the capability flag.
		/// </summary>
		/// <value>The capability.</value>
		public CapabilityFlags Capability {
			get {
				int source = SourceData[0] & 0x07;
				if (source >= 4)
					return CapabilityFlags.Full;
				if (source >= 1) {
					return CapabilityFlags.GICBOnly;
				}
				return CapabilityFlags.SurveillanceOnly;
			}
		}
				

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Source data.</param>
		public ModeS(byte[] data) {
			SourceData = data;
			AircraftID = new byte[3];
			AircraftID[0] = SourceData[1];
			AircraftID[1] = SourceData[2];
			AircraftID[2] = SourceData[3];

		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Eltok.MessageCollector.Lib.ModeS"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Eltok.MessageCollector.Lib.ModeS"/>.</returns>
		public override string ToString() {
			return BitConverter.ToString(this.AircraftID);
		}

		/// <summary>
		/// Check if a message is of a specific type
		/// </summary>
		/// <returns></returns>
		/// <param name="source">Source.</param>
		/// <param name="compare">Message format identifier.</param>
		public static bool IsFormat(byte source, byte compare) {
			return (source >> 3) == compare;
		}

		/// <summary>
		/// Creates a new message object
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="source">Source data.</param>
		public static ModeS GetMessage(byte[] source) {
			byte first5 = (byte)(source[0] >> 3);
			switch (first5) {
				case DF11.FormatCode:
					return new DF11(source);
				case DF17.FormatCode:
					return new DF17(source);
				//case DF18.FormatCode:
				//	return new DF18(source);
			case DF20.FormatCode:
				return new DF20(source);
			}

			return null;
		}

		/// <summary>
		/// Creates a byte array from a hex string
		/// </summary>
		/// <returns>Byte array with data.</returns>
		/// <param name="hex">Hex source string.</param>
		public static byte[] StringToByteArray(string hex) {
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}

		/// <summary>
		/// Decode an altitude
		/// </summary>
		/// <returns>The altitude.</returns>
		/// <param name="source">Source bits as int</param>
		/// <param name="nb">Number of bits in source</param>
		public static Altitude DecodeAltitude(int source, int nb) {
			// Get M & Q bits
			int m = 0;
			int q = ((source & 0x0010) >> 4);
			if (nb == 13) {
				m = ((source & 0x0040) >> 6);
				// Remove M-bit
				source = ((source & 0x1F80) >> 1) | (source & 0x003F);
			}
			// Remove Q-bit
			source = ((source & 0xFE0) >> 1) | (source & 0x00F);
			// Check values
			if (m != 0) {
				// Metric; cant compute
				return null;
			}
			if (q == 1) {
				return new Altitude(source * 25 - 1000, Altitude.Units.Feet);
			}
			return null;
		}
	}
}

