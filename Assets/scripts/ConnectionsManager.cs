using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace dassault
{
    public enum ConnectionType
    {
        UNKNOWN,
        BLUETOOTH,
        TCPIP
    }

    public abstract class BaseParameters
    {
        public readonly ConnectionType cxnType;

        public BaseParameters(ConnectionType cxnType)
        {
            this.cxnType = cxnType;
        }
    }

    public class DeviceParameters : BaseParameters
    {
        public readonly int deviceIdx;
        public byte[] deviceName;
        public int deviceNameLength;
        public byte[] deviceAddress;
        public int deviceAdressLength;

        public DeviceParameters(ConnectionType cxnType, int deviceIdx) :
            base(cxnType)
        {
            this.deviceIdx = deviceIdx;
        }
    }

    public abstract class ServerParameters : BaseParameters
    {
        public ServerParameters(ConnectionType cxnType) :
            base(cxnType)
        {
        }
    }

    public abstract class ClientParameters : BaseParameters
    {
        public readonly string remoteAddr;
        public readonly string remoteName;

        public ClientParameters(ConnectionType cxnType, System.String remoteAddr, System.String remoteName) :
            base(cxnType)
        {
            this.remoteAddr = remoteAddr;
            this.remoteName = remoteName;
        }
    }

    public class ConnectionsManager
    {
        #region Attributs
        private IBTServices m_btServices;
        private TCPServices m_tcpServices;
        #endregion

        #region Constructor
        public ConnectionsManager()
        {
#if UNITY_ANDROID
			m_btServices = BluetoothDataExchange.GetInstance();
#else
            m_btServices = new BTServicesWindows();
#endif
            m_tcpServices = new TCPServices();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Retrieves information of a connection device.
        /// </summary>
        /// <param name="parameters">Information to provide.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int GetDeviceInfo(ref DeviceParameters parameters)
        {
            switch (parameters.cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.GetRadioInfos(parameters.deviceIdx, parameters.deviceName, ref parameters.deviceNameLength, parameters.deviceAddress, ref parameters.deviceAdressLength);

                case ConnectionType.TCPIP:
                    return m_tcpServices.GetDeviceInfo(ref parameters);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return -1;
            }
        }

        /// <summary>
        /// Starts a new server.
        /// </summary>
        /// <param name="parameters">Parameters used to initialize the server</param>
        /// <returns>The id of the newly created server if success, an error code if not (TBD).</returns>
        public int StartServer(ServerParameters parameters)
        {
            switch(parameters.cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    Debug.Log("starting bluetooth server");
                    BTServerParameters btParams = (BTServerParameters)parameters;
                    return m_btServices.StartServer(btParams.instanceName, btParams.guid, btParams.backlog);

                case ConnectionType.TCPIP:
                    Debug.Log("starting TCP server");
                    TCPServerParameters tcpParams = (TCPServerParameters)parameters;
                    return m_tcpServices.StartServer(tcpParams);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return -1;
            }
        }

        /// <summary>
        /// Stops a server.
        /// </summary>
        /// <param name="serverId">ServerId id of the server to stop.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int StopServer(int serverId, ConnectionType cxnType)
        {
            switch(cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.StopServer(serverId);

                case ConnectionType.TCPIP:
                    return m_tcpServices.StopServer(serverId);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Make a server to stop listening new connections without closing existing connections.
        /// </summary>
        /// <param name="serverId">id of the server that has to stop listening new connections.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int StopListeningNewConnections(int serverId, ConnectionType cxnType)
        {
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.StopListeningNewConnections(serverId);

                case ConnectionType.TCPIP:
                    return m_tcpServices.StopListeningNewConnections(serverId);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

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
        public int GetNewConnectionsId(int serverId, ref int idMin, ref int idMax, ConnectionType cxnType)
        {
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.GetNewConnectionsId(serverId, ref idMin, ref idMax);

                case ConnectionType.TCPIP:
                    return m_tcpServices.GetNewConnectionsId(serverId, ref idMin, ref idMax);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Close the connection established from a distant client to a local server.
        /// When a connection is closed, the ids of the others don't change.
        /// </summary>
        /// <param name="serverId">The local server id</param>
        /// <param name="clientId">id of the connection to close</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int CloseServerConnection(int serverId, int clientId, ConnectionType cxnType)
        {
            Debug.Log("closing server");
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.CloseServerConnection(serverId, clientId);

                case ConnectionType.TCPIP:
                    return m_tcpServices.CloseServerConnection(serverId, clientId);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Sends data from a server to a client.
        /// </summary>
        /// <param name="serverId">id of the source server.</param>
        /// <param name="clientId">id of the destination client.</param>
        /// <param name="buffer">data to send.</param>
        /// <param name="bufferLength">size of the data to send.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int SendFromServer(int serverId, int clientId, byte[] buffer, int bufferLength, ConnectionType cxnType)
        {
            Debug.Log("sending data from server");
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.SendFromServer(serverId, clientId, buffer, bufferLength);

                case ConnectionType.TCPIP:
                    return m_tcpServices.SendFromServer(serverId, clientId, buffer, bufferLength);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Receives data from a client to a server.
        /// </summary>
        /// <param name="serverId">id of the destination server.</param>
        /// <param name="clientId">id of the source client.</param>
        /// <param name="buffer">buffer to store the received data</param>
        /// <param name="bufferLength">size of the data to receive.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int RecvFromServer(int serverId, int clientId, byte[] buffer, int bufferLength, ConnectionType cxnType)
        {
            Debug.Log("receiving data from server");
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.RecvFromServer(serverId, clientId, buffer, bufferLength);

                case ConnectionType.TCPIP:
                    return m_tcpServices.RecvFromServer(serverId, clientId, buffer, bufferLength);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Starts a new client.
        /// </summary>
        /// <param name="parameters">Parameters used to initialize the client.</param>
        /// <returns>The id of the newly created client if success, an error code if not (TBD).</returns>
        public int StartClient(ClientParameters parameters)
        {
            switch (parameters.cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    Debug.Log("starting bluetooth client");
                    BTClientParameters btParams = (BTClientParameters)parameters;
                    return  m_btServices.StartClient(btParams.maxCxnCycles, btParams.remoteAddr, btParams.remoteName, btParams.guid, btParams.channel);

                case ConnectionType.TCPIP:
                    Debug.Log("starting TCP client");
                    TCPClientParameters tcpParams = (TCPClientParameters)parameters;
                    return m_tcpServices.StartClient(tcpParams);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return -1;
            }
        }

        /// <summary>
        /// Stops a client.
        /// </summary>
        /// <param name="clientId">id of the client to stop.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int StopClient(int clientId, ConnectionType cxnType)
        {
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    Debug.Log("stopping bluetooth client");
                    return m_btServices.StopClient(clientId);

                case ConnectionType.TCPIP:
                    Debug.Log("stopping tcp client");
                    return m_tcpServices.StopClient(clientId);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Sends data from a client to its connected server.
        /// </summary>
        /// <param name="clientId">id of the source client.</param>
        /// <param name="buffer">data to send.</param>
        /// <param name="bufferLength">size of the data to send.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int SendFromClient(int clientId, byte[] buffer, int bufferLength, ConnectionType cxnType)
        {
            Debug.Log("sending data from client");
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.SendFromClient(clientId, buffer, bufferLength);

                case ConnectionType.TCPIP:
                    return m_tcpServices.SendFromClient(clientId, buffer, bufferLength);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }

        /// <summary>
        /// Receives data to a client from its connected server.
        /// </summary>
        /// <param name="clientId">id of the destination client.</param>
        /// <param name="buffer">buffer to store the received data</param>
        /// <param name="bufferLength">size of the data to receive.</param>
        /// <returns>0 if success, an error code if not (TBD).</returns>
        public int RecvFromClient(int clientId, byte[] buffer, int bufferLength, ConnectionType cxnType)
        {
            Debug.Log("receiving data from client");
            switch (cxnType)
            {
                case ConnectionType.BLUETOOTH:
                    return m_btServices.RecvFromClient(clientId, buffer, bufferLength);

                case ConnectionType.TCPIP:
                    return m_tcpServices.RecvFromClient(clientId, buffer, bufferLength);

                default:
                    Debug.LogError("ConnectionsManager: invalid connection type.");
                    return 1;
            }
        }
        #endregion
    }
}
