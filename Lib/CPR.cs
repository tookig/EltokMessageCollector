using System;
using System.Collections.Generic;

namespace Eltok.MessageCollector.Lib {
	/// <summary>
	/// Static class used to calculate compressed position reports.
	/// </summary>
	public abstract class CPR {
		/// <summary>
		/// Helper class to store history items
		/// </summary>
		protected class GlobalItem {
			public int CPR { get; set; }
			public int YZi { get; set; }
			public int XZi { get; set;}
			public DateTime Timestamp { get; set; }
			public GlobalItem(int cpr, int yzi, int xzi) {
				this.CPR = cpr;
				this.YZi = yzi;
				this.XZi = xzi;
				this.Timestamp = DateTime.UtcNow;
			}
		}

		/// <summary>
		/// Latitude zone table
		/// </summary>
		protected static double[] s_lzonetable = null;
		/// <summary>
		/// Number of latitude zones
		/// </summary>
		public const int NZ = 15;

		/// <summary>
		/// Previously processed messages
		/// </summary>
		protected static Dictionary<string, GlobalItem> s_history = new Dictionary<string, GlobalItem>();

		/// <summary>
		/// Saves a position report, and returns the previous valid one. Check time difference (must
		/// be less than 10 seconds), and CPR bit (cannot be equal).
		/// </summary>
		/// <returns>The last known position message</returns>
		/// <param name="AID">Aircraft ID</param>
		/// <param name="CPR">CPR bit</param>
		/// <param name="Nb">Number of bits per lat/lon item</param>
		/// <param name="YZi">Latitude bits</param>
		/// <param name="XZi">Longitude bits</param>
		protected static GlobalItem ProcessHistory(byte[] AID, int CPR, int Nb, int YZi, int XZi) {
			// Only process 17-bit positions
			if (Nb != 17) {
				return null;
			}
			// Create ID string
			string search = BitConverter.ToString(AID);
			// Get old if present
			GlobalItem gi = null;
			GlobalItem newgi = new GlobalItem(CPR, YZi, XZi);
			if (s_history.TryGetValue(search, out gi)) {
				// Check so that CPR is different
				if (gi.CPR == CPR) {
					gi = null;
				}
				// Check so that previous message isnt too old
				else if (DateTime.UtcNow.Subtract(gi.Timestamp).TotalSeconds > 10.0) {
					gi = null;
				}
				// Save new message
				s_history[search] = newgi;
			}
			else {
				// Not found, add to list
				s_history.Add(search, newgi);
			}
			return gi;
		}

		/// <summary>
		/// Get the latitude zone for a specific latitude
		/// </summary>
		/// <param name="lat">Latitude</param>
		public static int NL(double lat) {
			// Make sure the table is made
			if (s_lzonetable == null) {
				InitLZoneTable();
			}
			// Get zone index
			for (int i = s_lzonetable.Length - 1; i >= 0; i--) {
				if (Math.Abs(lat) < s_lzonetable[i]) {
					return i + 1;
				}
			}
			return 1;
		}

		/// <summary>
		/// Modulo function for CPR calculations
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static double MOD(double x, double y) {
			if (y == 0.0)
				return x;
			return (x >= 0.0) ? (x % y) : ((x % y) + y);
		}

		/// <summary>
		/// Creates the latitude zone table
		/// </summary>
		protected static void InitLZoneTable() {
			s_lzonetable = new double[NZ*4-1];
			for (int i = 2; i < s_lzonetable.Length; i++) {
				s_lzonetable[i] = 180.0 / Math.PI * Math.Acos(Math.Sqrt((1.0 - Math.Cos(Math.PI/(2.0*(double)NZ)))/(1.0-Math.Cos(2.0*Math.PI/((double)i+1.0)))));
			}
			s_lzonetable[0] = 90.0;
			s_lzonetable[1] = 87.0;
		}

		/// <summary>
		/// Creates a local position based on a nearby location.
		/// </summary>
		/// <returns>The airborne position.</returns>
		/// <param name="reference">Reference.</param>
		/// <param name="i">CPR (odd or even)</param>
		/// <param name="Nb">Bitcount</param>
		/// <param name="YZi">Latitude bit value</param>
		/// <param name="XZi">Longitude bit value</param>
		public static Position LocalAirborneCPR(Position reference, int i, int Nb, int YZi, int XZi) {
			// Calculate latitude
			double Dlati = 360.0 / (4.0 * NZ - i);
			double YZdec = (double)YZi / (double)(1 << Nb);
			int j = (int)Math.Floor(reference.Lat / Dlati) + (int)Math.Floor(0.5 + MOD(reference.Lat, Dlati) / Dlati - YZdec);
			double RLati = Dlati * ((double)j + YZdec);
			// Get latitude zone index
			int NLi = NL(RLati) - i;
			// Calculate longitude
			double Dloni = (NLi == 0 ? 360.0 : 360.0 / NLi);
			double XZdec = (double)XZi / (double)(1 << Nb);
			double m = Math.Floor(reference.Lon / Dloni) + Math.Floor(0.5 + MOD(reference.Lon, Dloni) / Dloni - XZdec);
			double RLoni = Dloni * (m + XZdec);
			// Return
			return new Position(RLati, RLoni);
		}

		/// <summary>
		/// Calculates a global position. Needs two messages, one even and one odd within a 
		/// 10 second timespan.
		/// </summary>
		/// <returns>The global position.</returns>
		/// <param name="AID">Aircraft ID</param>
		/// <param name="i">CPR (odd or even)</param>
		/// <param name="Nb">Bitcount</param>
		/// <param name="YZi">Latitude bit value</param>
		/// <param name="XZi">Longitude bit value</param>
		public static Position GlobalAirborneCPR(byte[] AID, int i, int Nb, int YZi, int XZi) {
			// Add this item to history and retrieve previous one (if any)
			GlobalItem prev = ProcessHistory(AID, i, Nb, YZi, XZi);
			if (prev == null) {
				return null;
			}
			// Save positions
			GlobalItem curr = new GlobalItem(i, YZi, XZi);
			GlobalItem i0 = prev;
			GlobalItem i1 = curr;
			// Make sure i0 has the CPR=0 item and i1 the CPR=1 item
			if (prev.CPR == 1) {
				i0 = curr;
				i1 = prev;
			}
			// Calculate latitude from both messages
			double Dlat0 = 360.0 / (4.0 * NZ);
			double Dlat1 = 360.0 / (4.0 * NZ - 1.0);
			int j = (int)Math.Floor((59.0 * i0.YZi - 60.0*i1.YZi)/(1 << Nb) + 0.5);
			double RLat0 = Dlat0 * (MOD(j, 60.0) + i0.YZi/(double)(1 << Nb));
			double RLat1 = Dlat1 * (MOD(j, 59.0) + i1.YZi/(double)(1 << Nb));
			RLat0 -= RLat0 > 180.0 ? 360.0 : 0.0;
			RLat1 -= RLat1 > 180.0 ? 360.0 : 0.0;
			double RLat = curr.CPR == 0 ? RLat0 : RLat1;
			// Check so that the values are within the same latitude area. Else not able to compute.
			if (NL(RLat0) != NL(RLat1)) {
				return null;
			}
			// Calculate the most recent longitude value
			int nl = NL(RLat);
			int ni = Math.Max(nl - curr.CPR, 1);
			double Dloni = 360.0 / (double)ni;
			int m = (int)Math.Floor(((double)i0.XZi*(double)(nl - 1) - (double)i1.XZi * (double)nl)/(double)(1<<Nb) + 0.5);
			double RLon = Dloni * (MOD(m, ni) + (double)curr.XZi/(double)(1 << Nb));
			// Return
			return new Position(RLat, RLon);
		}

		/// <summary>
		/// Creates a local position based on a nearby location.
		/// </summary>
		/// <returns>The surface position.</returns>
		/// <param name="reference">Reference.</param>
		/// <param name="i">CPR (odd or even)</param>
		/// <param name="YZi">Latitude bit value (17 bits)</param>
		/// <param name="XZi">Longitude bit value (17 bits)</param>
		public static Position LocalSurfaceCPR(Position reference, int i, int YZi, int XZi) {
			// Calculate latitude
			double Dlati = 90.0 / (4.0 * NZ - i);
			double YZdec = (double)YZi / (double)(1 << 17);
			int j = (int)Math.Floor(reference.Lat / Dlati) + (int)Math.Floor(0.5 + MOD(reference.Lat, Dlati) / Dlati - YZdec);
			double RLati = Dlati * ((double)j + YZdec);
			// Get latitude zone index
			int NLi = NL(RLati) - i;
			// Calculate longitude
			double Dloni = (NLi == 0 ? 90.0 : 90.0 / NLi);
			double XZdec = (double)XZi / (double)(1 << 17);
			double m = Math.Floor(reference.Lon / Dloni) + Math.Floor(0.5 + MOD(reference.Lon, Dloni) / Dloni - XZdec);
			double RLoni = Dloni * (m + XZdec);
			// Return
			return new Position(RLati, RLoni);
		}

		public CPR() {
		}
	}
}

