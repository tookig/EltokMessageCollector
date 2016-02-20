using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class Unknown : ADSBBase {
		public Unknown(byte[] id, byte[] adsbsource) : base(id, adsbsource) {
		}
	}
}

