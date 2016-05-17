using System.Runtime.InteropServices;

namespace dassault
{
	public class BTServicesWindows : IBTServices
	{
		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetRadioInfos")]
		private static extern int GetRadioInfosCb(int radioIdx, [In, Out]System.IntPtr radioName, ref int radioNameLength, [In, Out]System.IntPtr radioAddress, ref int radioAdressLength);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StartServer")]
		private static extern int StartServerCb(System.String instanceName, System.String guid, int backlog);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StopServer")]
		private static extern int StopServerCb(int serverId);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StopListeningNewConnections")]
		private static extern int StopListeningNewConnectionsCb(int serverId);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetNewConnectionsId")]
		private static extern int GetNewConnectionsIdCb(int serverId, ref int idMin, ref int idMax);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CloseServerConnection")]
		private static extern int CloseServerConnectionCb(int serverId, int clientId);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SendFromServer")]
		private static extern int SendFromServerCb(int serverId, int clientId, [In]System.IntPtr buffer, int bufferLength);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "RecvFromServer")]
		private static extern int RecvFromServerCb(int serverId, int clientId, [In, Out]System.IntPtr buffer, int bufferLength);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StartClient")]
		private static extern int StartClientCb(int maxCxnCycles, System.String remoteAddr, System.String remoteName, System.String guid, int channel);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StopClient")]
		private static extern int StopClientCb(int clientId);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SendFromClient")]
		private static extern int SendFromClientCb(int clientId, [In]System.IntPtr buffer, int bufferLength);

		[DllImport("BTServices.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "RecvFromClient")]
		private static extern int RecvFromClientCb(int clientId, [In, Out]System.IntPtr buffer, int bufferLength);

		#region IBTServices implementation

		public int GetRadioInfos(int radioIdx, byte[] radioName, ref int radioNameLength, byte[] radioAddress, ref int radioAdressLength)
		{
			GCHandle gcName = GCHandle.Alloc(radioName, GCHandleType.Pinned);
			System.IntPtr namePtr = gcName.AddrOfPinnedObject();

			GCHandle gcAddr = GCHandle.Alloc(radioAddress, GCHandleType.Pinned);
			System.IntPtr addrPtr = gcAddr.AddrOfPinnedObject();

			try
			{
				return GetRadioInfosCb(radioIdx, namePtr, ref radioNameLength, addrPtr, ref radioAdressLength);
			}
			finally
			{
				gcName.Free();
				gcAddr.Free();
			}
		}

		public int StartServer(string instanceName, string guid, int backlog)
		{
			return StartServerCb(instanceName, guid, backlog);
		}

		public int StopServer(int serverId)
		{
			return StopServerCb(serverId);
		}

		public int StopListeningNewConnections(int serverId)
		{
			return StopListeningNewConnectionsCb(serverId);
		}

		public int GetNewConnectionsId(int serverId, ref int idMin, ref int idMax)
		{
			return GetNewConnectionsIdCb(serverId, ref idMin, ref idMax);
		}

		public int CloseServerConnection(int serverId, int clientId)
		{
			return CloseServerConnectionCb(serverId, clientId);
		}

		public int SendFromServer(int serverId, int clientId, byte[] buffer, int bufferLength)
		{
			GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			System.IntPtr bufferPtr = gc.AddrOfPinnedObject();

			try
			{
				return SendFromServerCb(serverId, clientId, bufferPtr, bufferLength);
			}
			finally
			{
				gc.Free();
			}
		}

		public int RecvFromServer(int serverId, int clientId, byte[] buffer, int bufferLength)
		{
			GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			System.IntPtr bufferPtr = gc.AddrOfPinnedObject();
			try
			{
				return RecvFromServerCb(serverId, clientId, bufferPtr, bufferLength);
			}
			finally
			{
				gc.Free();
			}
		}

		public int StartClient(int maxCxnCycles, string remoteAddr, string remoteName, string guid, int channel)
		{
			return StartClientCb(maxCxnCycles, remoteAddr, remoteName, guid, channel);
		}

		public int StopClient(int clientId)
		{
			return StopClientCb(clientId);
		}

		public int SendFromClient(int clientId, byte[] buffer, int bufferLength)
		{
			GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			System.IntPtr bufferPtr = gc.AddrOfPinnedObject();
			try
			{
				return SendFromClientCb(clientId, bufferPtr, bufferLength);
			}
			finally
			{
				gc.Free();
			}
		}

		public int RecvFromClient(int clientId, byte[] buffer, int bufferLength)
		{
			GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			System.IntPtr bufferPtr = gc.AddrOfPinnedObject();
			try
			{
				return RecvFromClientCb(clientId, bufferPtr, bufferLength);
			}
			finally
			{
				gc.Free();
			}
		}

		#endregion
	}
}