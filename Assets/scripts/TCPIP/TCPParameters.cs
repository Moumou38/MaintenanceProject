using UnityEngine;
using System.Collections;

namespace dassault
{
    public class TCPServerParameters : ServerParameters
    {
        public readonly int port;

        public TCPServerParameters(int port) :
            base(ConnectionType.TCPIP)
        {
            this.port = port;
        }
    }

    public class TCPClientParameters : ClientParameters
    {
        public readonly int port;

        public TCPClientParameters(string remoteAddr, string remoteName, int port) :
            base(ConnectionType.TCPIP, remoteAddr, remoteName)
        {
            this.port = port;
        }
    }
}
