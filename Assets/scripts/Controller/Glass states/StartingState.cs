using UnityEngine;
using System.IO;

namespace dassault
{
    public partial class ConcreteGlassController
    {
        protected class StartingState : GlassControllerState
        {
            public StartingState(ref ConcreteGlassController controller)
                : base(ref controller)
            {
            }

            public override void Update()
            {
                if (!m_btServerInitialized)
                {
                    BTServerParameters parameters = new BTServerParameters("GlassServer", "9C6ABA4A-642D-47BD-BDCA-9E0A4123522A", -1);
                    int ret = m_controller.m_cxnManager.StartServer(parameters);
                    if (ret < 0)
                    {
                        Debug.LogError("Error while starting the BT server");
                        // todo gui callback ???
                    }
                    else
                    {
                        m_controller.m_serverInfo.id = ret;
                        m_btServerInitialized = true;
                    }
                }

                if (!m_tcpServerInitialized)
                {
                    // TODO change well known port management to send it in the connection command to allow the other party to connect back ?
                    TCPServerParameters parameters = new TCPServerParameters(2345);
                    int ret = m_controller.m_cxnManager.StartServer(parameters);
                    if (ret < 0)
                    {
                        Debug.LogError("Error while starting TCP server");
                        // TODO GUI callback ???
                    }
                    else
                    {
                        m_controller.m_tcpServerInfo.id = ret;
                        m_tcpServerInitialized = true;
                    }
                }
                if (m_btServerInitialized && m_tcpServerInitialized)
                {
                    // network layer initialized
                    ControllerState newState = new WaitingWatchConnectionState(ref m_controller);
                    m_controller.ChangeState(ref newState);
                }
            }

            bool m_btServerInitialized = false;
            bool m_tcpServerInitialized = false;
        }
    }
}
