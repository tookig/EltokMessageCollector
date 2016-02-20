using System;
namespace Eltok.MessageCollector.Lib {
	public class DF11 : ModeS {
		public const byte FormatCode = 0x0B;

		// DF0  public const byte FormatCode = 0x00;
		// DF4  public const byte FormatCode = 0x04;
		// DF5  public const byte FormatCode = 0x05;
		// DF16 public const byte FormatCode = 0x10;
		// DF18 public const byte FormatCode = 0x12;
		// DF19 public const byte FormatCode = 0x13;
		// DF20 public const byte FormatCode = 0x14;
		// DF22 public const byte FormatCode = 0x16;


		public DF11(byte[] data) : base(data) {
		}

		public override string ToString() {
			return string.Format("DF11: {0}", base.ToString());
		}
	}
}

