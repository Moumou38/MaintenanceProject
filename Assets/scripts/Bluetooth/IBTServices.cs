using System.Collections;

namespace dassault
{
	public interface IBTServices
	{
		/// <summary>
		/// Retrieves information of a radio device.
		/// </summary>
		/// <param name="radioIdx">Index of the radio device for which information is requested.</param>
		/// <param name="radioName">Name of the radio device.</param>
		/// <param name="radioNameLength">Size of the allocated buffer for the radio name. If the function returns
		/// success this parameters contains the size of the radio name string.</param>
		/// <param name="radioAddress">Address of the radio device.</param>
		/// <param name="radioAdressLength">Size of the allocated buffer for the radio address. If the function returns
		/// success this parameters contains the size of the radio address string</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int GetRadioInfos(int radioIdx, byte[] radioName, ref int radioNameLength, byte[] radioAddress, ref int radioAdressLength);

		/// <summary>
		/// Starts a new bluetooth server.
		/// </summary>
		/// <param name="instanceName">Name of the bluetooth service executed by the server.</param>
		/// <param name="guid">GUID of the bluetooth service executed by the server.</param>
		/// <param name="backlog">Maximum size of the queue of pending connections. If -1 is specified, the default underlying value is used.</param>
		/// <returns>The id of the newly created server if success, an error code if not (TBD).</returns>
		int StartServer(System.String instanceName, System.String guid, int backlog);

		/// <summary>
		/// Stops a bluetooth server.
		/// </summary>
		/// <param name="serverId">ServerId id of the server to stop.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int StopServer(int serverId);

		/// <summary>
		/// Make a server to stop listening new connections without closing existing connections.
		/// </summary>
		/// <param name="serverId">id of the server that has to stop listening new connections.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int StopListeningNewConnections(int serverId);

		/// <summary>
		/// Retrieves a range of values corresponding to the ids of the new connections established 
		/// with a given server since the last "GetNewConnectionsId" call.
		/// The returned range will not be returned anymore.
		/// </summary>
		/// <param name="serverId">id of the local server.</param>
		/// <param name="idMin">The lower id of the new connections.</param>
		/// <param name="idMax">The higher id of the new connections.</param>
		/// <returns>
		/// 0 if there is no new ids,
		/// 1 if the server id is invalid,
		/// 2 if success.
		/// </returns>
		int GetNewConnectionsId(int serverId, ref int idMin, ref int idMax);

		/// <summary>
		/// Close the connection established from a distant client to a local server.
		/// When a connection is closed, the ids of the others don't change.
		/// </summary>
		/// <param name="serverId">The local server id</param>
		/// <param name="clientId">id of the connection to close</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int CloseServerConnection(int serverId, int clientId);

		/// <summary>
		/// Sends data from a bluetooth server to a bluetooth client.
		/// </summary>
		/// <param name="serverId">id of the source server.</param>
		/// <param name="clientId">id of the destination client.</param>
		/// <param name="buffer">data to send.</param>
		/// <param name="bufferLength">size of the data to send.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int SendFromServer(int serverId, int clientId, byte[] buffer, int bufferLength);

		/// <summary>
		/// Receives data from a bluetooth client to a bluetooth server.
		/// </summary>
		/// <param name="serverId">id of the destination server.</param>
		/// <param name="clientId">id of the source client.</param>
		/// <param name="buffer">buffer to store the received data</param>
		/// <param name="bufferLength">size of the data to receive.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int RecvFromServer(int serverId, int clientId, byte[] buffer, int bufferLength);

		/// <summary>
		/// Starts a new bluetooth client.
		/// </summary>
		/// <param name="maxCxnCycles">Number of connection attempts.</param>
		/// <param name="remoteAddr">Address of the server to connect.</param>
		/// <param name="remoteName">Name of the server to connect (not used if the remoteAddr is specified).</param>
		/// <param name="guid">GUID of the bluetooth service to connect on the server.</param>
		/// <param name="channel">The RFCOMM channel (not used if the GUID is specified).</param>
		/// <returns>The id of the newly created client if success, an error code if not (TBD).</returns>
		int StartClient(int maxCxnCycles, System.String remoteAddr, System.String remoteName, System.String guid, int channel);

		/// <summary>
		/// Stops a bluetooth client.
		/// </summary>
		/// <param name="clientId">id of the client to stop.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int StopClient(int clientId);

		/// <summary>
		/// Sends data from a bluetooth client to its connected bluetooth server.
		/// </summary>
		/// <param name="clientId">id of the source client.</param>
		/// <param name="buffer">data to send.</param>
		/// <param name="bufferLength">size of the data to send.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int SendFromClient(int clientId, byte[] buffer, int bufferLength);

		/// <summary>
		/// Receives data to a bluetooth client from its connected bluetooth server.
		/// </summary>
		/// <param name="clientId">id of the destination client.</param>
		/// <param name="buffer">buffer to store the received data</param>
		/// <param name="bufferLength">size of the data to receive.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		int RecvFromClient(int clientId, byte[] buffer, int bufferLength);
	}
}