using UnityEngine;
using System.Collections;

namespace dassault
{
    public class TCPClientManager
    {
        #region Attributes
        System.Net.Sockets.TcpClient m_client;
        Connection m_connection;
        TCPClientParameters m_parameters;
        #endregion

        #region Constructor
        public TCPClientManager(TCPClientParameters parameters)
        {
            m_parameters = parameters;
            m_client = null;
            m_connection = null;
        }
        #endregion

        #region Public methods
        public int Start()
        {
            try
            {
                if(m_parameters.remoteAddr != string.Empty)
                {
                    Debug.Log("starting client connection with adress: " + m_parameters.remoteAddr);
                    m_client = new System.Net.Sockets.TcpClient(m_parameters.remoteAddr, m_parameters.port);
                }
                else
                {
                    Debug.Log("starting client connection with name: " + m_parameters.remoteName);
                    m_client = new System.Net.Sockets.TcpClient(m_parameters.remoteName, m_parameters.port);
                }

                m_connection = new Connection(m_client.GetStream());
            }
            catch(System.Net.Sockets.SocketException e)
            {

                Debug.LogError("failed to establish client connection: " + e.Message);
                m_connection = null;

                switch(e.ErrorCode)
                {
                    default:
                        return 1;
                }
            }

            return 0;
        }

        public int Stop()
        {
            if(m_client == null) return 1;

            m_connection.Close();
            m_client.Close();

            m_client = null;
            m_connection = null;

            return 0;
        }

        public int Send(byte[] buffer, int bufferLength)
        {
            if (m_client == null) return 1;

            if (!m_connection.Send(buffer, bufferLength)) return 1;

            return 0;
        }

        public int Recv(byte[] buffer, int bufferLength)
        {
            if (m_client == null) return 1;

            if (!m_connection.Recv(buffer, bufferLength)) return 1;

            return 0;
        }
        #endregion
    }
}
