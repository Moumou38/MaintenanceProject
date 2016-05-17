using UnityEngine;
using System.Collections;

namespace dassault
{
    public class Connection
    {
        #region Attributs
        private System.Net.Sockets.NetworkStream m_stream;
        #endregion

        #region Constructor
        public Connection(System.Net.Sockets.NetworkStream s)
        {
            m_stream = s;
        }
        #endregion

        #region Public methods
        public void Close()
        {
            m_stream.Close();
        }

        public bool Send(byte[] buffer, int bufferLength)
        {
            Debug.Log("Connection: writing data on socket");
            try
            {
                m_stream.Write(buffer, 0, bufferLength);
            }
            catch(System.Net.Sockets.SocketException e)
            {
                Debug.LogError("Connection: error while sending data: " + e.Message);
                switch(e.ErrorCode)
                {
                    default:
                        return false;
                }
            }
            return true;
       }

        public bool Recv(byte[] buffer, int bufferLength)
        {
            Debug.Log("Connection:receiving data on socket");
            int lengthReceived = 0, totalLengthReceived = 0;
            bool success = true;

            int offset = 0;

            try
            {
                while (success && (totalLengthReceived < bufferLength))
                {
                    lengthReceived = m_stream.Read(buffer, offset, bufferLength - totalLengthReceived);

                    if (lengthReceived == 0)
                    {
                        Debug.Log("TCP/IP connection: The conneciton has been gracefully closed.");
                        success = false;
                    }
                    else
                    {
                        if(lengthReceived > (bufferLength - totalLengthReceived))
                        {
                            Debug.Log("TCP/IP connection: received too much data.");
                            success = false;
                        }
                        else
                        {
                            offset += lengthReceived;
                            totalLengthReceived += lengthReceived;
                        }
                    }
                }
            }
            catch(System.IO.IOException e)
            {
                Debug.LogError("Connection: error while receiving data: " + e.Message);
                success = false;

                if(e.InnerException != null)
                {
                    System.Net.Sockets.SocketException se = e.InnerException as System.Net.Sockets.SocketException;
                    if (se != null)
                    {
                        switch (se.ErrorCode)
                        {
                            default:
                                Debug.Log("TCP/IP connection: Socket exception!");
                                break;
                        }
                    }
                }
            }

            return success;
        }
        #endregion
    }
}

