using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace dassault
{
    public class TCPServerManager : ConnectionObserver
    {
        #region Attributs
        private TCPServerWrapper m_server;
        private List<Connection> m_connections;
        private List<int> m_newCxnIds;
        private static Object m_connectionsLock = new Object();
        #endregion

        #region Constructor
        public TCPServerManager(TCPServerParameters parameters)
        {
            m_server = new TCPServerWrapper(parameters);
            m_connections = new List<Connection>();
            m_newCxnIds = new List<int>();

            m_server.AttachObserver(this);
        }
        #endregion

        #region Public methods
        public int Start()
        {
            if (!m_server.Start()) return 1;

            return 0;
        }

        public int Stop()
        {
            if (!m_server.Stop()) return 1;

            m_connections.Clear();

            return 0;
        }

        public int Send(int clientId, byte[] buffer, int bufferLength)
        {
            Connection cxn = null;

            lock(m_connectionsLock)
            {
                if((clientId >= m_connections.Count) || (clientId < 0))
                {
                    Debug.Log("TCP/IP Error! The client id " + clientId.ToString() + " is not valid.");
                    return 1;
                }

                cxn = m_connections[clientId];
            }

            if(cxn == null)
            {
                Debug.Log("TCP/IP Error! The connection has been previously closed.");
                return 1;
            }

            if (!cxn.Send(buffer, bufferLength)) return 1;

            return 0;
        }

        public int Recv(int clientId, byte[] buffer, int bufferLength)
        {
            Connection cxn = null;

            lock (m_connectionsLock)
            {
                if ((clientId >= m_connections.Count) || (clientId < 0))
                {
                    Debug.Log("TCP/IP Error! The client id " + clientId.ToString() + " is not valid.");
                    return 1;
                }

                cxn = m_connections[clientId];
            }

            if (cxn == null)
            {
                Debug.Log("TCP/IP Error! The connection has been previously closed.");
                return 1;
            }

            if (!cxn.Recv(buffer, bufferLength)) return 1;

            return 0;
        }

        public int StopListeningNewConnection()
        {
            m_server.StopListening();
            return 0;
        }

        public int GetNewConnectionsId(ref int minId, ref int maxId)
        {
            lock(m_connectionsLock)
            {
                int nbNewCxn = m_newCxnIds.Count;

                if (nbNewCxn == 0) return 0;

                minId = m_newCxnIds[0];
                maxId = m_newCxnIds[nbNewCxn - 1];

                m_newCxnIds.Clear();
            }

            return 2;
        }

        public int CloseConnection(int clientId)
        {
            if(m_server.CloseConnection(clientId))
            {
                m_connections[clientId] = null;

                return 0;
            }

            return 1;
        }

        public override void NotifyNewConnection(Connection cxn)
        {
            lock(m_connectionsLock)
            {
                m_connections.Add(cxn);
                m_newCxnIds.Add(m_connections.Count - 1);
            }
        }
        #endregion
    }
}
