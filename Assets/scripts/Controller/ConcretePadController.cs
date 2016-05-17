using System;
using UnityEngine;

namespace dassault
{
	public partial class ConcretePadController : PadController
	{
		#region Constructor
		public ConcretePadController()
		{
            m_serverInfo = new ServerInfo(ConnectionType.BLUETOOTH);

            m_glassConnectionInfo = new ConnectionInfo(ConnectionType.BLUETOOTH);

			ConcretePadController controller = this;
			m_state = new StartingState(ref controller);
		}
		#endregion

		#region Public methods
		public ConcretePadController Init(PadControllerCallbacks padCallback, GlassControllerCallbacks glassCallback, IBTServices btServices)
		{
			base.Init(padCallback, glassCallback);

			return this;
		}

		public override void SendAnnotationBackToGlasses(string stepPath, byte[] image)
		{
			PushMessage(new SendAnnotationAck(new AnnotationCmd(stepPath, image)));
		}

		public override void ConnectToDevice(string deviceName, string deviceAddress)
		{
			PushMessage(new ConnectTo(deviceName, deviceAddress));
		}

		#endregion Public methods

		#region Protected methods
		protected override void PreQuit()
		{
			Debug.Log("Stops the connections");

			CloseServer();

			CloseGlassesConnection();
		}
		#endregion Protected methods

		#region Privates methods
		private void CloseGlassesConnection()
		{
            if (m_glassConnectionInfo.localToRemoteId != -1 && m_cxnManager.StopClient(m_glassConnectionInfo.localToRemoteId, m_glassConnectionInfo.type) != 0)
			{
				Debug.LogError("Error while closing connection with glasses");
			}

            if (m_glassConnectionInfo.remoteToLocalId != -1 &&
                m_serverInfo.id != -1 &&
                m_cxnManager.CloseServerConnection(m_serverInfo.id, m_glassConnectionInfo.remoteToLocalId, m_glassConnectionInfo.type) != 0)
			{
				Debug.LogError("Error while closing connection with glasses");
			}

            m_glassConnectionInfo.localToRemoteId = -1;
            m_glassConnectionInfo.remoteToLocalId = -1;
		}

		private void CloseServer()
		{
            if (m_serverInfo.id != -1 && m_cxnManager.StopServer(m_serverInfo.id, m_serverInfo.cxnType) != 0)
			{
				Debug.LogError("Error while stopping the server");
			}

            m_serverInfo.id = -1;
		}
		#endregion

		#region Attributs
        /// <summary>
        /// Information about the server used to receive commands from glasses.
        /// </summary>
        private ServerInfo m_serverInfo;

        /// <summary>
        /// Information about the connections established with the glasses.
        /// </summary>
        private ConnectionInfo m_glassConnectionInfo;

		/// <summary>
		/// Information about the client we try to connect to.
		/// </summary>
		private ConnectTo m_connectToRqt;
		#endregion Attributs
	}
}
