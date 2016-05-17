using UnityEngine;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace dassault
{
    public class TCPServices
    {
        #region Attributs
        private List<TCPServerManager> m_tcpServers;
        private List<TCPClientManager> m_tcpClients;
        #endregion

        #region Constructor
        public TCPServices()
        {
            m_tcpServers = new List<TCPServerManager>();
            m_tcpClients = new List<TCPClientManager>();
        }
        #endregion

        #region Public methods
        public int GetDeviceInfo(ref DeviceParameters parameters)
        {            
            // get the device name
            string hostname = Dns.GetHostName();
            // record the hostname in the parameters

            byte[] hostnameAsBytes = System.Text.Encoding.ASCII.GetBytes(hostname);
            parameters.deviceName = hostnameAsBytes;
            parameters.deviceNameLength = hostname.Length;

            // get the device address
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
            if (hostAddresses.Length == 0)
            {
                //failed to retrieve the host adress wtf ??? "not 0" means error
                return 1;
            }
            // there must be at least one address, take the first 
            string hostAddressAsString = hostAddresses[0].ToString();
            byte[] hostAddressAsBytes = System.Text.Encoding.ASCII.GetBytes(hostAddressAsString);
            parameters.deviceAddress = hostAddressAsBytes;
            parameters.deviceAdressLength = hostAddressAsString.Length;

            // 0 means success!
            return 0;
        }

        public int StartServer(TCPServerParameters parameters)
        {
            TCPServerManager mgr = new TCPServerManager(parameters);

            Debug.Log("TCPServices: tries to start the server on port: " + parameters.port);

            int ret = mgr.Start();

            if(ret == 1)
            {
                Debug.Log("TCPServices: Failed to start the server.");
                return -1;
            }

            Debug.Log("TCPServices: Server successfully started.");

            int serverId = 0;
            if(m_tcpServers.Count == 0)
            {
                m_tcpServers.Add(mgr);
            }
            else
            {
                int idx = 0;
                for(; idx < m_tcpServers.Count; ++idx)
                {
                    if(m_tcpServers[idx] == null)
                    {
                        m_tcpServers[idx] = mgr;
                        serverId = idx;
                        break;
                    }
                }

                if(idx == m_tcpServers.Count)
                {
                    m_tcpServers.Add(mgr);
                    serverId = m_tcpServers.Count - 1;
                }
            }

            return serverId;
        }

        public int StopServer(int serverId)
        {
            Debug.Log("TCPServices: Stops server " + serverId.ToString());

            if(m_tcpServers.Count == 0)
            {
                Debug.LogError("TCPServices: No server started.");
                return 1;
            }

            if((serverId >= m_tcpServers.Count) || (serverId < 0))
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " is not valid.");
                return 1;
            }

            TCPServerManager mgr = m_tcpServers[serverId];

            if(mgr == null)
            {
                Debug.LogError("TCPServices: The server " + serverId.ToString() + " is already stopped.");
                return 1;
            }

            int ret = mgr.Stop();

            m_tcpServers[serverId] = null;

            return ret;
        }

        public int StopListeningNewConnections(int serverId)
        {
            Debug.Log("TCPServices: Stop server " + serverId.ToString() + " listening new connections.");

            if(m_tcpServers.Count == 0)
            {
                Debug.LogError("TCPServices: No server started!");
                return 1;
            }

            if ((serverId >= m_tcpServers.Count) || (serverId < 0))
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " is not valid.");
                return 1;
            }

            TCPServerManager mgr = m_tcpServers[serverId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The server " + serverId.ToString() + " has been stopped.");
                return 1;
            }

            return mgr.StopListeningNewConnection();
        }

        public int GetNewConnectionsId(int serverId, ref int idMin, ref int idMax)
        {
            if (m_tcpServers.Count == 0)
            {
                Debug.LogError("TCPServices: No server started!");
                return -2;
            }

            if ((serverId >= m_tcpServers.Count) || (serverId < 0))
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " is not valid.");
                return -2;
            }

            TCPServerManager mgr = m_tcpServers[serverId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The server " + serverId.ToString() + " has been stopped.");
                return -2;
            }

            return mgr.GetNewConnectionsId(ref idMin, ref idMax);
        }

        public int CloseServerConnection(int serverId, int clientId)
        {
            Debug.Log("TCPServices: Tries to close the connection from the distant client " + clientId.ToString() + " to the local server " + serverId.ToString());

            if (m_tcpServers.Count == 0)
            {
                Debug.LogError("TCPServices: No server started!");
                return 1;
            }

            if ((serverId >= m_tcpServers.Count) || (serverId < 0))
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " is not valid.");
                return 1;
            }

            TCPServerManager mgr = m_tcpServers[serverId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The server " + serverId.ToString() + " has been stopped.");
                return 1;
            }

            return mgr.CloseConnection(clientId);
        }

        public int SendFromServer(int serverId, int clientId, byte[] buffer, int bufferLength)
        {
            Debug.Log("TCPServices: Sends data from server " + serverId.ToString() + " to client " + clientId.ToString());

            if (m_tcpServers.Count == 0)
            {
                Debug.LogError("TCPServices: No server started!");
                return 1;
            }

            if ((serverId >= m_tcpServers.Count) || (serverId < 0))
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " is not valid.");
                return 1;
            }

            TCPServerManager mgr = m_tcpServers[serverId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The server " + serverId.ToString() + " has been stopped.");
                return 1;
            }

            return mgr.Send(clientId, buffer, bufferLength);
        }

        public int RecvFromServer(int serverId, int clientId, byte[] buffer, int bufferLength)
        {
            Debug.Log("TCPServices: Receives data from server " + serverId.ToString() + " to client " + clientId.ToString());

            if (m_tcpServers.Count == 0)
            {
                Debug.LogError("TCPServices: No server started!");
                return 1;
            }

            if ((serverId >= m_tcpServers.Count) || (serverId < 0))
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " is not valid.");
                return 1;
            }

            TCPServerManager mgr = m_tcpServers[serverId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The server id " + serverId.ToString() + " has been stopped.");
                return 1;
            }

            return mgr.Recv(clientId, buffer, bufferLength);
        }

        public int StartClient(TCPClientParameters parameters)
        {
            TCPClientManager mgr = new TCPClientManager(parameters);

            Debug.Log("TCPServices: tries to connect to server (name: " + parameters.remoteName + ", address: " + parameters.remoteAddr + ") on port: " + parameters.port.ToString() + ".");

            int ret = mgr.Start();

            if (ret == 1)
            {
                Debug.Log("TCPServices: Failed to start the client.");
                return -1;
            }

            Debug.Log("TCPServices: Client successfully started.");

            int clientId = 0;
            if (m_tcpClients.Count == 0)
            {
                m_tcpClients.Add(mgr);
            }
            else
            {
                int idx = 0;
                for (; idx < m_tcpClients.Count; ++idx)
                {
                    if (m_tcpClients[idx] == null)
                    {
                        m_tcpClients[idx] = mgr;
                        clientId = idx;
                        break;
                    }
                }

                if (idx == m_tcpServers.Count)
                {
                    m_tcpClients.Add(mgr);
                    clientId = m_tcpClients.Count - 1;
                }
            }

            return clientId;
        }

        public int StopClient(int clientId)
        {
            Debug.Log("TCPServices: Stops client " + clientId.ToString());

            if (m_tcpClients.Count == 0)
            {
                Debug.LogError("TCPServices: No client started.");
                return 1;
            }

            if ((clientId >= m_tcpClients.Count) || (clientId < 0))
            {
                Debug.LogError("TCPServices: The client id " + clientId.ToString() + " is not valid.");
                return 1;
            }

            TCPClientManager mgr = m_tcpClients[clientId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The client id " + clientId.ToString() + " is already stopped.");
                return 1;
            }

            int ret = mgr.Stop();

            m_tcpClients[clientId] = null;

            return ret;
        }

        public int SendFromClient(int clientId, byte[] buffer, int bufferLength)
        {
            Debug.Log("TCPServices: Sends data from client " + clientId.ToString());

            if (m_tcpClients.Count == 0)
            {
                Debug.LogError("TCPServices: No client started!");
                return 1;
            }

            if ((clientId >= m_tcpClients.Count) || (clientId < 0))
            {
                Debug.LogError("TCPServices: The client id " + clientId.ToString() + " is not valid.");
                return 1;
            }

            TCPClientManager mgr = m_tcpClients[clientId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The client " + clientId.ToString() + " has been stopped.");
                return 1;
            }

            return mgr.Send(buffer, bufferLength);
        }

        public int RecvFromClient(int clientId, byte[] buffer, int bufferLength)
        {
            Debug.Log("TCPServices: Receives data from client " + clientId.ToString());

            if (m_tcpClients.Count == 0)
            {
                Debug.LogError("TCPServices: No client started!");
                return 1;
            }

            if ((clientId >= m_tcpClients.Count) || (clientId < 0))
            {
                Debug.LogError("TCPServices: The client id " + clientId.ToString() + " is not valid.");
                return 1;
            }

            TCPClientManager mgr = m_tcpClients[clientId];

            if (mgr == null)
            {
                Debug.LogError("TCPServices: The client " + clientId.ToString() + " has been stopped.");
                return 1;
            }

            return mgr.Recv(buffer, bufferLength);
        }
        #endregion
    }
}
