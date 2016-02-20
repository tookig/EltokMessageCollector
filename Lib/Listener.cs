using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using System.Text;
//using System.Text.RegularExpressions;


namespace Eltok.MessageCollector.Lib {
	public abstract class Listener {
		/*public class StateObject {
			public Socket Socket = null;
			public const int BufferSize = 256;
			public byte[] Buffer = new byte[BufferSize];
		}*/

		private Socket m_socket = null;
		private const int c_buffersize = 256;
		private byte[] m_buffer;
		private string m_description;

		public int Port { get; set; }

		public Listener(int port) {
			Port = port;
			this.m_buffer = new byte[c_buffersize];
		}
			
		public void Connect() {
			if (this.m_socket != null) {
				throw (new Exception("Listener already active; cannot connect"));
			}
			try {
				IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEP = new IPEndPoint(ipAddress,this.Port);
				this.m_description = remoteEP.ToString();
				Console.WriteLine("Connecting to {0}...", this.m_description);

				this.m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				this.m_socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), this.m_socket);
			}
			catch (Exception err) {
				System.Console.WriteLine(err.ToString());
			}

		}

		private void ConnectCallback(IAsyncResult ar) {
			try {
				this.m_socket.EndConnect(ar);
				Console.WriteLine("Socket connected to {0}", this.m_socket.RemoteEndPoint.ToString());
				Receive();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private void Receive() {
			try {
				this.m_socket.BeginReceive(this.m_buffer, 0, c_buffersize, 0, new AsyncCallback(ReceiveCallback), null);
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private void ReceiveCallback( IAsyncResult ar ) {
			try {
				int bytesRead = this.m_socket.EndReceive(ar);
				if (bytesRead > 0) {
					byte[] data = new byte[bytesRead];
					for (int i=0;i<bytesRead;i++) { data[i] = this.m_buffer[i]; }
					IncomingData(data);															
					this.m_socket.BeginReceive(this.m_buffer, 0, c_buffersize, 0, new AsyncCallback(ReceiveCallback), null);
				} else {
					Receive();
				}
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		public void Disconnect() {
			if (this.m_socket == null) {
				throw (new Exception("Listener not active; cannot disconnect"));
			}
			try {
				this.m_socket.BeginDisconnect(false, DisconnectCallback, null);
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private void DisconnectCallback(IAsyncResult ar) {
			Console.WriteLine("Disconnected from {0}", this.m_description);
			this.m_socket = null;
		}

		protected abstract void IncomingData(byte[] data);

	}
}

