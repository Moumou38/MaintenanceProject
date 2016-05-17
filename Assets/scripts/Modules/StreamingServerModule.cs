using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace dassault
{
    public class StreamingServerModule : ModuleInstance
    {
        public override void OnAllModuleLoaded(ModuleRepository repository)
        {
            Activate(false);
        }

        public override HashSet<string> GetModuleDependencies()
        {
            HashSet<string> result = new HashSet<string>();
            return result;
        }

        private new void Awake()
        {
            base.Awake();
            m_streamingServerAdapter = new StreamingServerAdapter();
        }

        public bool startStreaming(out string url, out int port)
        {
            return m_streamingServerAdapter.startServer(out url, out port);
        }

        public void stopStreaming()
        {
            m_streamingServerAdapter.stopServer();
        }

        StreamingServerAdapter m_streamingServerAdapter;
    }
}
