using System;
using Eltok.MessageCollector.Lib.ADSB;

namespace Eltok.MessageCollector.Lib {
	/// <summary>
	/// A DF17 message
	/// </summary>
	public class DF17 : ModeS {
		/// <summary>
		/// DF17 format code
		/// </summary>
		public const byte FormatCode = 0x11;

		/// <summary>
		/// ADSB payload
		/// </summary>
		/// <value></value>
		public ADSBBase ADSB { get; protected set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data.</param>
		public DF17(byte[] data) : base(data) {
			byte[] adsb = new byte[7];
			for (int i = 4; i < 11; i++) {
				adsb[i - 4] = data[i];
			}
			this.ADSB = ADSBBase.GetADSBMessage(base.AircraftID, adsb);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Eltok.MessageCollector.Lib.DF17"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Eltok.MessageCollector.Lib.DF17"/>.</returns>
		public override string ToString() {
			return string.Format("DF17: {0} {1}", base.ToString(), this.ADSB.ToString());
		}
	}
}

