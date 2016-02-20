using System;
namespace Eltok.MessageCollector.Lib.ADSB {
	public class OperationalStatus : ADSBBase {
		public OperationalStatus(byte[] id, byte[] adsbsource) : base(id, adsbsource) {
		}

		public override string ToString() {
			return string.Format("{0} Operational Status (no data)", base.ToString());
		} 
	}
}

