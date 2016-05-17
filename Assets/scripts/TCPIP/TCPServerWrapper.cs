using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace dassault
{
    public class TCPServerWrapper : ConnectionObservable
    {
        #region Attributs
        private TCPServerParameters m_parameters;
        private System.Net.Sockets.TcpListener m_listener;
        private volatile bool m_stopLoop;
        private System.Threading.Thread m_cxnLoop;
        private List<Connection> m_connections;
        private static Object m_connectionsLock = new Object();
        #endregion

        #region Constructor
        public TCPServerWrapper(TCPServerParameters parameters)
        {
            m_parameters = parameters;
            m_stopLoop = false;
            m_cxnLoop = null;
            m_connections = new List<Connection>();
        }
        #endregion

        #region Public methods
        public bool Start()
        {
            try
            {
                m_listener = new System.Net.Sockets.TcpListener(m_parameters.port);
                m_listener.Start();
            }
            catch(System.ArgumentOutOfRangeException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            catch(System.Net.Sockets.SocketException se)
            {
                switch(se.ErrorCode)
                {
                    default:
                        Debug.LogError("Socket exception (error code: " + se.ErrorCode.ToString() + ")");
                        return false;
                }
            }

            m_cxnLoop = new System.Threading.Thread(new System.Threading.ThreadStart(ConnectionLoop));
            m_cxnLoop.Start();

            return true;
        }

        public bool Stop()
        {
            StopListening();

            foreach(Connection cxn in m_connections)
            {
                if(cxn != null)
                {
                    cxn.Close();
                }
            }

            m_connections.Clear();

            return true;
        }

        public bool StopListening()
        {
            m_stopLoop = true;

            if (m_listener != null)
            {
                m_listener.Stop();
                m_listener = null;
            }

            if (m_cxnLoop.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                m_cxnLoop.Join();
            }

            return true;
        }

        public bool CloseConnection(int cxnId)
        {
            lock(m_connectionsLock)
            {
                if(cxnId >= m_connections.Count)
                {
                    Debug.Log("TCP/IP Close connection error! Invalid connection id.");
                    return false;
                }

                Connection cxn = m_connections[cxnId];

                if(cxn == null)
                {
                    Debug.Log("TCP/IP Close connection error! The connection has already been closed.");
                    return false;
                }

                cxn.Close();
                m_connections[cxnId] = null;

                return true;
            }
        }
        #endregion

        #region Private methods
        private void ConnectionLoop()
        {
            while(!m_stopLoop)
            {
                System.Net.Sockets.Socket s = m_listener.AcceptSocket();

                Connection cxn = null;
                lock (m_connectionsLock)
                {
                    cxn = new Connection(new System.Net.Sockets.NetworkStream(s, true));
                    m_connections.Add(cxn);
                }

                if(m_observer != null)
                    m_observer.NotifyNewConnection(cxn);
            }
        }
        #endregion
    }
}
