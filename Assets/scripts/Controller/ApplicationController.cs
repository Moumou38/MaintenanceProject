using UnityEngine;
using System;
using System.IO;

namespace dassault
{

    // utility class to help with network buffer formatting.
    class ByteAppender
    {
        public static byte[] append(byte[] source, byte[] toAppend)
        {
            // allocate new array that contains both of them
            byte[] fusion = new byte[source.Length + toAppend.Length];

            // copy the first byte array in the destination byte array
            Buffer.BlockCopy(source, 0, fusion, 0, source.Length);

            // copy the second byt array in the destination byte array
            Buffer.BlockCopy(toAppend, 0, fusion, source.Length, toAppend.Length);

            return fusion;
        }

        public static byte[] append(byte[] source, byte toAppend)
        {
            byte[] fusion = new byte[source.Length + 1];

            // copy the first byte array in the destination byte array
            Buffer.BlockCopy(source, 0, fusion, 0, source.Length);

            // append the byte at the end of the buffer
            fusion[source.Length] = toAppend;

            return fusion;
        }
    }

    public partial class ApplicationController : MonoBehaviour
	{
		protected enum DeviceType
		{
			Watch,
			Glasses,
			Pad,
			Unknown
		}

        protected class ConnectionInfo
        {
            public readonly ConnectionType type;
            public int localToRemoteId;
            public int remoteToLocalId;

            public ConnectionInfo(ConnectionType cxnType)
            { 
                this.type = cxnType;
                localToRemoteId = -1;
                remoteToLocalId = -1;
            }
        }

        protected class ServerInfo
        {
            public readonly ConnectionType cxnType;
            public int id;

            public ServerInfo(ConnectionType cxnType)
            {
                this.cxnType = cxnType;
            }
        }

		public ApplicationController()
		{
			m_cxnManager = new ConnectionsManager();
			m_state = new ControllerState();

			m_pendingMessages = new System.Collections.Generic.List<Message>();
			m_nonValidConnections = new System.Collections.Generic.Dictionary<ServerInfo, System.Collections.Generic.List<int>>();
			m_pendingMessagesLock = new UnityEngine.Object();
			m_activeConnectionsIdsLock = new UnityEngine.Object();

            DeviceParameters deviceParams = new DeviceParameters(ConnectionType.BLUETOOTH, 0);
            deviceParams.deviceName = new byte[100];
            deviceParams.deviceAddress = new byte[100];
            deviceParams.deviceNameLength = deviceParams.deviceName.Length;
            deviceParams.deviceAdressLength = deviceParams.deviceAddress.Length;

            // get bluetooth device params
            if (m_cxnManager.GetDeviceInfo(ref deviceParams) == 0)
            {
                m_btRadioDeviceName = System.Text.Encoding.ASCII.GetString(deviceParams.deviceName, 0, deviceParams.deviceNameLength);
                m_btRadioDeviceAddr = System.Text.Encoding.ASCII.GetString(deviceParams.deviceAddress, 0, deviceParams.deviceAdressLength);

                Debug.Log("Bluetooth device name: " + m_btRadioDeviceName + "\n");
                Debug.Log("Bluetooth device address: " + m_btRadioDeviceAddr + "\n");
            }
            else
            {
                Debug.LogError("Impossible to retrieve the radio device information");
            }

            // get tcp device params
            DeviceParameters TCPDeviceParameters =new DeviceParameters(ConnectionType.TCPIP,0);
            TCPDeviceParameters.deviceName = new byte[100];
            TCPDeviceParameters.deviceAddress = new byte[100];
            TCPDeviceParameters.deviceNameLength = TCPDeviceParameters.deviceName.Length;
            TCPDeviceParameters.deviceAdressLength = TCPDeviceParameters.deviceAddress.Length;

            if (m_cxnManager.GetDeviceInfo(ref TCPDeviceParameters) == 0)
            {
                // 0 means success...
                m_tcpDeviceName = System.Text.Encoding.ASCII.GetString(TCPDeviceParameters.deviceName, 0, TCPDeviceParameters.deviceNameLength);
                m_tcpDeviceAddr = System.Text.Encoding.ASCII.GetString(TCPDeviceParameters.deviceAddress, 0, TCPDeviceParameters.deviceAdressLength);

                Debug.Log("TCP device name: " + m_tcpDeviceName + "\n");
                Debug.Log("TCP device address: " + m_tcpDeviceName + "\n");
            }
            else 
            {
                Debug.LogError("impossible to retrieve tcp device information");
            }
		}

		#region protected methods
		protected void StartDataReception(ServerInfo serverInfo, int clientId)
		{
            AddNonValidConnection(serverInfo, clientId);

            System.Threading.ThreadStart starter = () => ReceptionCallback(serverInfo, clientId);
			System.Threading.Thread t = new System.Threading.Thread(starter);
			t.Start();
		}

		protected void CloseAllNonValidConnections(ServerInfo serverInfo)
		{
			lock (m_activeConnectionsIdsLock)
			{
                foreach (int id in m_nonValidConnections[serverInfo])
				{
                    if (m_cxnManager.CloseServerConnection(serverInfo.id, id, serverInfo.cxnType) != 0)
					{
                        Debug.LogError("Error while closing the undesired connection between server " + serverInfo.id + " and client " + id);
					}
				}
			}
		}

		protected void ChangeState(ref ControllerState state)
		{
			m_state.OnExit();
			m_state = state;
			m_state.OnEnter();
		}

		protected virtual void PreQuit() {}

		protected virtual void PreUpdate() 
		{
			m_state.Update();
		}

        protected void sendBuffer(ConnectionInfo cxnInfo, byte[] buffer)
        {
            // set the payload size in the buffer = buffer size - header size 
            // header  = size + command type
            byte[] payloadSizeBytes = new byte[sizeof(int)];
            payloadSizeBytes = BitConverter.GetBytes(buffer.Length - (sizeof(int)+ sizeof(byte)));
            Buffer.BlockCopy(payloadSizeBytes, 0, buffer, 0, sizeof(int));

            int ret = m_cxnManager.SendFromClient(cxnInfo.localToRemoteId, buffer, buffer.Length, cxnInfo.type);

            if (ret != 0)
            {
                Debug.LogError("Error while sending the buffer.");
                m_state.OnEmissionError(cxnInfo.localToRemoteId, ret);
                return;
            }
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ScenarioCmd cmd)
		{
            // reserve 4 bytes for the size of the payload of the message
			byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Scenario);

            // Sends the size of the project name
            byte[] nameBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.ProjectName);
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(nameBuffer.Length));

            // Sends the project name
            buffer = ByteAppender.append(buffer, nameBuffer);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo,buffer);
		}

        protected void SendCommand(ConnectionInfo cxnInfo, NextStepCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.NextStep);
            
            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, PreviousStepCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.PreviousStep);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, BookmarksCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Bookmarks);

            // Sends the index of the bookmark
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.BookmarkIdx));

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, CurrentViewNextCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.CurrentViewNext);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, CurrentViewPreviousCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.CurrentViewPrevious);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SwitchLocalizationCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SwitchLocalization);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SwitchReferencesCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SwitchReferences);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SwitchToolsCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SwitchTools);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SwitchCommentsCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SwitchComments);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SwitchAnnotationsCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SwitchAnnotations);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SwitchGUICmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SwitchGUI);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, WatchScreenChangedCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.WatchScreenChanged);

            // Sends the screen index
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.ScreenIdx));

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, WatchStepPathChangedCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.WatchStepPathChanged);

			// Sends the path length
			byte[] pathBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.StepPath);
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(pathBuffer.Length));

            // Sends the path
            buffer = ByteAppender.append(buffer, pathBuffer);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, TakeScreenshotCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.TakeScreenshot);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, DiagnosticCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Diagnostic);

            // Sends the flags that indicates if the diagnostic command has been accepted or rejected.
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.Accept));

			// Sends the path length
			byte[] pathBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.StepPath);
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(pathBuffer.Length));

            // Sends the path
            buffer = ByteAppender.append(buffer, pathBuffer);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ConnectionCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Connection);

			byte[] nameBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.DeviceName);
			byte[] addrBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.DeviceAddr);

            // Sends the size of the name (1 byte)
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(nameBuffer.Length));

            // Sends the the name
            buffer = ByteAppender.append(buffer, nameBuffer);

            // Sends the size of the address (1 byte)
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(addrBuffer.Length));

            // Sends the the address
            buffer = ByteAppender.append(buffer, addrBuffer);

            // Sends the flag that indicates if a connection with the pad is required
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.ConnectWithTab));

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, DisconnectionCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Disconnection);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ReSynchronizationCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.ReSynchronization);

			// Sends the size of the current step path (1 byte)
			byte[] pathBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.State.StepPath);
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(pathBuffer.Length));

            // Sends the current step path
            buffer = ByteAppender.append(buffer, pathBuffer);

            // Sends the highlight state of the switch annotation button
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.IsAnnotationsVisible));

            // Sends the annotation index
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.CurrentAnnotationIndex));

            // Sends the highlight state of the switch reference button
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.IsReferencesVisible));

            // Sends the reference index
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.CurrentReferenceIndex));

            // Sends the highlight state of the switch comment button
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.IsCommentsVisible));

            // Sends the comment index
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.CurrentCommentIndex));

            // Sends the highlight state of the switch localization button
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.IsLocalizationVisible));

            // Sends the highlight state of the switch GUI button
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.State.IsGuiVisible));

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ConnectedStatus sts)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Connected);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ConnectionRefusedStatus sts)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.ConnectionRefused);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, AnnotationCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Annotation);

			byte[] pathBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.StepPath);

            // Sends the size of the path (1 byte)
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(pathBuffer.Length));

            // Sends the path
            buffer = ByteAppender.append(buffer, pathBuffer);

            // Sends the size of the image (4 bytes)
            buffer = ByteAppender.append(buffer, System.BitConverter.GetBytes(cmd.ImageContent.Length));

            // Sends the image
            buffer = ByteAppender.append(buffer, cmd.ImageContent);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, CaptureCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.Capture);

			// Sends the size of the path (1 byte)
			byte[] pathBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.StepPath);

            buffer = ByteAppender.append(buffer, System.Convert.ToByte(pathBuffer.Length));

            // Sends the path
            buffer = ByteAppender.append(buffer, pathBuffer);

            // Sends the size of the image (4 bytes)
            buffer = ByteAppender.append(buffer, System.BitConverter.GetBytes(cmd.Image.Length));

            // Sends the image
            buffer = ByteAppender.append(buffer, cmd.Image);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ScenariiStatusCmd cmd)
        {            
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.ScenariiStatus);

            // the payload of the message is composed of [length - string]* followed by a 0 

            foreach (string scenario in cmd.m_scenariiList)
            {
                // Sends the size of the path (1 byte)
                byte[] scenarioBuffer = System.Text.Encoding.ASCII.GetBytes(scenario);

                buffer = ByteAppender.append(buffer, System.Convert.ToByte(scenarioBuffer.Length));

                // Sends the path
                buffer = ByteAppender.append(buffer, scenarioBuffer);
            }

            // append a trailing 0 in order for the receiver to detect the end of the message
            buffer = ByteAppender.append(buffer, 0);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo,ShowNewProcedureCmd cmd)
        {
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.ScenariiStatus);

            // the payload of the message is composed of boolean followed by a string
            buffer = ByteAppender.append(buffer, Convert.ToByte(cmd.m_status));

            // Sends the size of the scenario name (1 byte)
            byte[] scenarioBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.m_procedureName);

            buffer = ByteAppender.append(buffer, System.Convert.ToByte(scenarioBuffer.Length));

            // Sends the scenario name
            buffer = ByteAppender.append(buffer, scenarioBuffer);

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo,ConnectionToSupportCmd cmd)
        {
            /// --->>> nothing to do sent by the watch to the glass application
        }

        protected void SendCommand(ConnectionInfo cxnInfo,DisconnectionToSupportCmd cmd)
        {
            /// --->>> nothing to do, sent by the watch to the glass application
        }

        protected void SendCommand(ConnectionInfo cxnInfo, SupportStatusCmd cmd)
        {
            Debug.Log("sending supportStatusCmd");
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.SupportStatus);

            // add a boolean for the support status
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.m_accepted));

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, StreamParametersCmd cmd)
        {
            Debug.Log("sending StreamParameters");
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.StreamParameters);

            // format the url string
            byte[] urlBuffer = System.Text.Encoding.ASCII.GetBytes(cmd.m_url);

            // Sends the size of the url (1 byte)
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(urlBuffer.Length));

            // Sends the urlBuffer
            buffer = ByteAppender.append(buffer, urlBuffer);

            // add an int for the port(4 bytes)
            buffer = ByteAppender.append(buffer, System.BitConverter.GetBytes(cmd.m_port));

            // set the payload size in the buffer and send it
            sendBuffer(cxnInfo, buffer);
        }

        protected void SendCommand(ConnectionInfo cxnInfo, ConnectedToStreamCmd cmd)
        {
            Debug.Log("sending ConnectedToStream");
            // reserve 4 bytes for the size of the payload of the message
            byte[] buffer = new byte[sizeof(int)];

            // Sends the type of the command
            buffer = ByteAppender.append(buffer, (byte)CommandType.ConnectedToStream);

            // add a boolean for the stream connection status
            buffer = ByteAppender.append(buffer, System.Convert.ToByte(cmd.m_connected));

            //  set the payload size in the buffer and send it.
            sendBuffer(cxnInfo, buffer);
        }

		protected DeviceType GetDeviceType(string deviceName)
		{
			//DeviceType type = DeviceType.Unknown;
			DeviceType type = DeviceType.Glasses;

			if (deviceName.ToLower().Contains("watch"))
			{
				type = DeviceType.Watch;
			}
			else if (deviceName.ToLower().Contains("glass"))
			{
				type = DeviceType.Glasses;
			}
			else if (deviceName.ToLower().Contains("pad"))
			{
				type = DeviceType.Pad;
			}
			else if (deviceName.ToLower().Contains("tython"))
			{
				type = DeviceType.Pad;
			}

			return type;
		}

		protected void PushMessage(Message msg)
		{
			lock (m_pendingMessagesLock)
			{
				m_pendingMessages.Add(msg);
			}
		}

		#endregion protected methods

		#region private methods
		private void OnApplicationQuit()
		{
			PreQuit();
		}

		private void Update()
		{
			PreUpdate();

			Message[] messages = null;

			lock(m_pendingMessagesLock)
			{
				// We make a copy of the messages list because some messages
				// can generate new messages and the messages list can't be modified
				// while it is enumarated.
				// So we enumarate the copy of the messages list and the new messages
				// will be added to the original messages list.
				messages = m_pendingMessages.ToArray();
				m_pendingMessages.Clear();
			}

			foreach (Message msg in messages)
			{
				msg.AcceptVisitor(m_state);
			}
		}

		private void AddNonValidConnection(ServerInfo serverInfo, int clientId)
		{
			lock (m_activeConnectionsIdsLock)
			{
				System.Collections.Generic.List<int> connections = null;

				// Adds the active client id to the list of non valid connections for this server.
				// It will be removed on a Connection message reception.

                if (m_nonValidConnections.TryGetValue(serverInfo, out connections))
				{
					connections.Add(clientId);
				}
				else
				{
					connections = new System.Collections.Generic.List<int>();
					connections.Add(clientId);
                    m_nonValidConnections.Add(serverInfo, connections);
				}
			}
		}

        private void RemoveNonValidConnection(ServerInfo serverInfo, int clientId)
		{
			lock (m_activeConnectionsIdsLock)
			{
                m_nonValidConnections[serverInfo].Remove(clientId);
			}
		}

        private void ReceptionCallback(ServerInfo serverInfo, int clientId)
		{
#if UNITY_ANDROID
			AndroidJNI.AttachCurrentThread();
#endif
			bool error = false, disconnect = false;
			int ret = -1;
			while (!error && !disconnect)
            {
                // the first four bytes are for the message size, we don't need it in this code...
                byte[] buffer = new byte[4];
                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 4, serverInfo.cxnType);
                if (ret != 0)
                {
                    Debug.LogError("Error during the data reception");
                    error = true;
                    // jump to the enclosing while statement, and get out of the loop, as there is a reception error
                    continue; 
                }

                // the next byte is the cmdType enum value
                buffer = new byte[1];
                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

                if (ret != 0)
                {
                    Debug.LogError("Error during the data reception");
					error = true;
				}
				else
				{
					// translate received byte to a command type
					CommandType cmdType = (CommandType)buffer[0];
					switch (cmdType)
					{
						case CommandType.Scenario:
							{
                                Debug.Log("received CommandType.Scenario");
								// retrieves the size of the project name (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int nameLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the project name
								buffer = new byte[nameLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, nameLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string projectName = System.Text.Encoding.Default.GetString(buffer);

								Message cmd = new ScenarioCmd(projectName);
								PushMessage(cmd);

								break;
							}

						case CommandType.NextStep:
                            {
                                Debug.Log("received CommandType.NextStep");
								Message cmd = new NextStepCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.PreviousStep:
                            {
                                Debug.Log("received CommandType.PreviousStep");
								Message cmd = new PreviousStepCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.Bookmarks:
                            {
                                Debug.Log("received CommandType.Bookmarks");
								// retrieves the index of the bookmark (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int bookmarkIdx = System.Convert.ToInt32(buffer[0]);

								Message cmd = new BookmarksCmd(bookmarkIdx);
								PushMessage(cmd);

								break;
							}

						case CommandType.CurrentViewNext:
                            {
                                Debug.Log("received CommandType.CurrentViewNext");
								Message cmd = new CurrentViewNextCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.CurrentViewPrevious:
							{
                                Debug.Log("received CommandType.CurrentViewPrevious");
								Message cmd = new CurrentViewPreviousCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.SwitchLocalization:
                            {
                                Debug.Log("received CommandType.SwitchLocalization");
								Message cmd = new SwitchLocalizationCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.SwitchReferences:
                            {
                                Debug.Log("received CommandType.SwitchReferences");
								Message cmd = new SwitchReferencesCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.SwitchTools:
                            {
                                Debug.Log("received CommandType.SwitchTools");
								Message cmd = new SwitchToolsCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.SwitchComments:
                            {
                                Debug.Log("received CommandType.SwitchComments");
								Message cmd = new SwitchCommentsCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.SwitchAnnotations:
                            {
                                Debug.Log("received CommandType.SwitchAnnotations");
								Message cmd = new SwitchAnnotationsCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.SwitchGUI:
                            {
                                Debug.Log("received CommandType.SwitchGU");
								Message cmd = new SwitchGUICmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.WatchScreenChanged:
                            {
                                Debug.Log("received CommandType.WatchScreenChanged");
								// retrieves the index of the screen (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int screenIdx = System.Convert.ToInt32(buffer[0]);

								Message cmd = new WatchScreenChangedCmd(screenIdx);
								PushMessage(cmd);

								break;
							}

						case CommandType.WatchStepPathChanged:
                            {
                                Debug.Log("received CommandType.WatchStepPathChange");
								// retrieves the size of the path (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int pathLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the path
								buffer = new byte[pathLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, pathLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string stepPath = System.Text.Encoding.Default.GetString(buffer);

								Message cmd = new WatchStepPathChangedCmd(stepPath);
								PushMessage(cmd);

								break;
							}

						case CommandType.TakeScreenshot:
                            {
                                Debug.Log("received CommandType.TakeScreenshot");
								Message cmd = new TakeScreenshotCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.Diagnostic:
                            {
                                Debug.Log("received CommandType.Diagnostic");
								// retrieves the flag that indicates if the diagnostic command has been accepted or rejected.
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool accepted = System.Convert.ToBoolean(buffer[0]);

								// retrieves the size of the path (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int pathLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the path
								buffer = new byte[pathLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, pathLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string stepPath = System.Text.Encoding.Default.GetString(buffer);

								Message cmd = new DiagnosticCmd(stepPath, accepted);
								PushMessage(cmd);

								break;
							}

						case CommandType.Connection:
                            {
                                Debug.Log("received CommandType.Connection");
								// retrieves the size of the name (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int nameLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the name
								buffer = new byte[nameLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, nameLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string name = System.Text.Encoding.Default.GetString(buffer);

								// retrieves the size of the address (1 byte)
								buffer = new byte[1];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int addrLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the address
								buffer = new byte[addrLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, addrLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string addr = System.Text.Encoding.Default.GetString(buffer);

								// retrieves the flag that indicates if a connection with the pad is required
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool connectPad = System.Convert.ToBoolean(buffer[0]);

								Message cmd = new ConnectionCmd(name, addr, connectPad, clientId);
								PushMessage(cmd);

								// The connection is now valid. We can remove its id from the non valid connections list.
								RemoveNonValidConnection(serverInfo, clientId);
								break;
							}

						case CommandType.Disconnection:
                            {
                                Debug.Log("received CommandType.Disconnection");
								Message cmd = new DisconnectionCmd(clientId);
								PushMessage(cmd);

								// stops the reception callback.
								disconnect = true;

								Debug.Log("The client " + clientId + " is offline. Stops the reception callback.");

								break;
							}

						case CommandType.Connected:
                            {
                                Debug.Log("received ConnectedStatus");
								Message sts = new ConnectedStatus(clientId);
								PushMessage(sts);

								break;
							}

						case CommandType.AskReSynchronization:
                            {
                                Debug.Log("received CommandType.AskReSynchronization");
								Message cmd = new AskReSynchronizationCmd();
								PushMessage(cmd);

								break;
							}

						case CommandType.ReSynchronization:
                            {
                                Debug.Log("received CommandType.ReSynchronization");
								// retrieves the size of the current step path (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int stepPathLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the current step path
								buffer = new byte[stepPathLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, stepPathLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string currentStepPath = System.Text.Encoding.Default.GetString(buffer);

								// retrieves the highlight state of the switch annotation button
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool isAnnotationHighlighted = System.Convert.ToBoolean(buffer[0]);

								// retrieves the current annotation index
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int annotationIdx = System.Convert.ToInt32(buffer[0]);

								// retrieves the highlight state of the switch reference button
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool isReferenceHighlighted = System.Convert.ToBoolean(buffer[0]);

								// retrieves the current reference index
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int referenceIdx = System.Convert.ToInt32(buffer[0]);

								// retrieves the highlight state of the switch comment button
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool isCommentHighlighted = System.Convert.ToBoolean(buffer[0]);

								// retrieves the current comment index
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int commentIdx = System.Convert.ToInt32(buffer[0]);

								// retrieves the highlight state of the switch localization button
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool isLocalizationHighlighted = System.Convert.ToBoolean(buffer[0]);

								// retrieves the highlight state of the switch GUI button
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								bool isGUIDisplayed = System.Convert.ToBoolean(buffer[0]);

								ScenarioState state = new ScenarioState(currentStepPath, isCommentHighlighted, isReferenceHighlighted, false, isAnnotationHighlighted, commentIdx, referenceIdx, 0, annotationIdx, isLocalizationHighlighted, isGUIDisplayed);

								Message cmd = new ReSynchronizationCmd(state);
								PushMessage(cmd);

								break;
							}

						case CommandType.ConnectionRefused:
                            {
                                Debug.Log("received CommandType.ConnectionRefused");
								Message sts = new ConnectionRefusedStatus(clientId);
								PushMessage(sts);

								// stops the reception callback.
								disconnect = true;

								Debug.Log("The client has requested to terminate the connection.");

								break;
							}

						case CommandType.Annotation:
                            {
                                Debug.Log("received CommandType.Annotation");
								// retrieves the size of the path (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int pathLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the path
								buffer = new byte[pathLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, pathLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string stepPath = System.Text.Encoding.Default.GetString(buffer);

								// retrieves the size of the image
								buffer = new byte[4];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 4, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int imageSize = System.BitConverter.ToInt32(buffer, 0);

								// retrieves the image
								buffer = new byte[imageSize];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, imageSize, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}


                                Debug.Log("///////////// Application Controller : " + stepPath); 

								Message cmd = new AnnotationCmd(stepPath, buffer);
								PushMessage(cmd);

								break;
							}

						case CommandType.Capture:
                            {
                                Debug.Log("received CommandType.Capture");
								// retrieves the size of the path (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int pathLength = System.Convert.ToInt32(buffer[0]);

								// retrieves the path
								buffer = new byte[pathLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, pathLength, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								string stepPath = System.Text.Encoding.Default.GetString(buffer);

								// retrieves the size of the image
								buffer = new byte[4];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 4, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								int imageSize = System.BitConverter.ToInt32(buffer, 0);

								// retrieves the image
								buffer = new byte[imageSize];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, imageSize, serverInfo.cxnType);

								if (ret != 0)
								{
									Debug.LogError("Error during the data reception");
									error = true;
									break;
								}

								Message cmd = new CaptureCmd(stepPath, buffer);
								PushMessage(cmd);

								break;
							}
                        case CommandType.ScenariiStatus:
                            {
                                Debug.Log("received CommandType.ScenariiStatus");
                                ///// --->nothing to do , sent by the glass application to the watch
                                break;
                            }
                        case CommandType.ShowNewProcedure:
                            {
                                Debug.Log("received CommandType.ShowNewProcedure ");
                                ///// --->>> nothing to do, sent by the glass application to the watch
                                break;
                            }
                        case CommandType.ConnectionToSupport:
                            {
                                Debug.Log("received CommandType.ConnectionToSupport");
                                /// address type 1 byte
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }
                                ConnectionType expertSupportConnectionType = (ConnectionType) System.Convert.ToInt32(buffer[0]);

                                /// address length 1 byte
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }
                                int addressLength = System.Convert.ToInt32(buffer[0]);

                                /// address string
                                buffer = new byte[addressLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, addressLength, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                string address = System.Text.Encoding.Default.GetString(buffer);

                                /// expert nickname length 1 byte
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }
                                int expertNicknameLength = System.Convert.ToInt32(buffer[0]);

                                /// expert nickname string
                                buffer = new byte[expertNicknameLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, expertNicknameLength, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                string experNickname = System.Text.Encoding.Default.GetString(buffer);
                                

                                Message cmd = new ConnectionToSupportCmd(expertSupportConnectionType,address,experNickname);
                                PushMessage(cmd);
                                break;
                            }
                        case CommandType.DisconnectionToSupport:
                            {
                                Debug.Log("received CommandType.DisconnectionToSupport");
                                // no additionnal data ...
                                Message cmd = new DisconnectionToSupportCmd();
                                PushMessage(cmd);
                                break;
                            }
                        case CommandType.SupportStatus:
                            {
                                Debug.Log("received CommandType.SupportStatus");
                                 /// --->>> TODO
                                break;
                            }
                        case CommandType.StreamParameters:
                            {
                                Debug.Log("received CommandType.StreamParameters");
                                buffer = new byte[1];

                                // retrieves the size of the streaming server url (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);
                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                // retrieve the url as a string
                                int urlLength = System.Convert.ToInt32(buffer[0]);

                                // retrieve the url
                                buffer = new byte[urlLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, urlLength, serverInfo.cxnType);
                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }
                                string url = System.Text.Encoding.Default.GetString(buffer);

                                // retrieve the port number
                                buffer = new byte[4];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 4, serverInfo.cxnType);
                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                int port = System.Convert.ToInt32(buffer);

                                Message cmd = new StreamParametersCmd(url, port);
                                PushMessage(cmd);

                                break;
                            }
                        case CommandType.ConnectedToStream:
                            {
                                Debug.Log("received CommandType.ConnectedToStream");
                                buffer = new byte[1];

                                // retrieves the flag that indicates if the diagnostic command has been accepted or rejected.
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);
                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                bool connected = System.Convert.ToBoolean(buffer[0]);

                                Message cmd = new ConnectedToStreamCmd(connected);
                                PushMessage(cmd);
                                break;
                            }
                        case CommandType.DownloadProcedureFromURL:
                            {
                                Debug.Log("received CommandType.DownloadProcedureFromURL");

                                // retrieves the size of the name (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                int nameLength = System.Convert.ToInt32(buffer[0]);

                                Debug.Log("///// Name length : " + nameLength.ToString()); 

                                // retrieves the url
                                buffer = new byte[nameLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, nameLength, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                string name = System.Text.Encoding.Default.GetString(buffer);

                                // retrieves the size of the url (1 byte)
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, 1, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                int urlLength = System.Convert.ToInt32(buffer[0]);

                                // retrieves the url
                                buffer = new byte[urlLength];
                                ret = m_cxnManager.RecvFromServer(serverInfo.id, clientId, buffer, urlLength, serverInfo.cxnType);

                                if (ret != 0)
                                {
                                    Debug.LogError("Error during the data reception");
                                    error = true;
                                    break;
                                }

                                string url = System.Text.Encoding.Default.GetString(buffer);

                                Message cmd = new DownloadProcedureFromURLCmd(name, url);
                                PushMessage(cmd);
                                
                                break;
                            }
						default:
							{
								Debug.LogError("Unknown received command: " + System.Convert.ToInt32(buffer[0]).ToString());
								break;
							}
					}
				}
			}

			if (error)
			{
				ReceptionError err = new ReceptionError(serverInfo.id, clientId, -1);
				PushMessage(err);
			}

			Debug.Log("Termination of the reception callback between server " + serverInfo.id + " and client " + clientId);

#if UNITY_ANDROID
			AndroidJNI.DetachCurrentThread();
#endif
		}
		#endregion private methods

		#region attributs
        /// <summary>
        /// The module used to handle bluetooth and tcp connections.
        /// </summary>
        protected ConnectionsManager m_cxnManager;

        /// <summary>
        /// Name of the bluetooth radio device.
        /// </summary>
        protected string m_btRadioDeviceName;

        /// <summary>
        /// Address of the bluetooth radio device.
        /// </summary>
        protected string m_btRadioDeviceAddr;		
        
        /// <summary>
        /// Name of the tcp device.
        /// </summary>
        protected string m_tcpDeviceName;

        /// <summary>
        /// Address of the tcp device.
        /// </summary>
        protected string m_tcpDeviceAddr;

		/// <summary>
		/// The state of the controller.
		/// </summary>
		protected ControllerState m_state;

		/// <summary>
		/// Queue of received commands that have not been handled.
		/// </summary>
		private System.Collections.Generic.List<Message> m_pendingMessages;

		/// <summary>
		/// Lock to prevent concurrency on the commands queue.
		/// </summary>
		private UnityEngine.Object m_pendingMessagesLock;

		/// <summary>
		/// List of non valid connections for a given server, ie connections for
		/// which a Connection message has not been received yet.
		/// </summary>
		private System.Collections.Generic.Dictionary<ServerInfo, System.Collections.Generic.List<int>> m_nonValidConnections;

		/// <summary>
		/// Lock to prevent concurrency on the connection ids map.
		/// </summary>
		private UnityEngine.Object m_activeConnectionsIdsLock;
		#endregion attributs
	}
}
