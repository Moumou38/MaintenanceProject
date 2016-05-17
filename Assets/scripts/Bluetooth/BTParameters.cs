using UnityEngine;
using System.Collections;

namespace dassault
{
    public class BTServerParameters : ServerParameters
    {
        public readonly string instanceName;
        public readonly string guid;
        public readonly int backlog;

        public BTServerParameters(string instanceName, string guid, int backlog) :
            base(ConnectionType.BLUETOOTH)
        {
            this.instanceName = instanceName;
            this.guid = guid;
            this.backlog = backlog;
        }
    }

    public class BTClientParameters : ClientParameters
    {
        public readonly int maxCxnCycles;
        public readonly string guid;
        public readonly int channel;

        public BTClientParameters(int maxCxnCycles, System.String remoteAddr, System.String remoteName, System.String guid, int channel) :
            base(ConnectionType.BLUETOOTH, remoteAddr, remoteName)
        {
            this.maxCxnCycles = maxCxnCycles;
            this.guid = guid;
            this.channel = channel;
        }
    }
}
