#if UNITY_ANDROID

using UnityEngine;

namespace dassault {
    /// <summary>
    /// A core class that wraps Java methods of Android plugin.
    /// </summary>
	public sealed partial class BluetoothDataExchange: IBTServices {

        #region Methods

		public int GetRadioInfos(int radioIdx, byte[] radioName, ref int radioNameLength, byte[] radioAddress, ref int radioAdressLength)
		{
			if (!_isPluginAvailable)
				return 0;

			AndroidJavaObject infos = _plugin.Call<AndroidJavaObject>("getRadioInfos", radioIdx);
			if (infos == null) {
				return(-1);
			}

			byte[] name = infos.Get<byte[]>("mName");
			name.CopyTo(radioName, 0);
			radioNameLength = name.Length;

			byte[] addr = infos.Get<byte[]>("mAddress");
			addr.CopyTo(radioAddress, 0);
			radioAdressLength = addr.Length;

			return(0);
		}

		/// <summary>
		/// Starts a new bluetooth server.
		/// </summary>
		/// <returns>The id of the newly created server if success, an error code if not (TBD).</returns>
		public int StartServer(System.String instanceName, System.String guid, int backlog) {
			if (!_isPluginAvailable)
				return 0;
			
			return _plugin.Call<int>("startServer", instanceName, guid, backlog);
		}

		/// <summary>
		/// Stops a bluetooth server.
		/// </summary>
		/// <param name="serverId">serverId id of the server to stop.</param>
		/// <returns>0 if success, an error code if not (TBD).</returns>
		public int StopServer(int serverId) {
			if (!_isPluginAvailable)
				return 0;

			return _plugin.Call<int>("stopServer", serverId);
		}

		public int StopListeningNewConnections(int serverId)
		{
			if (!_isPluginAvailable)
				return 0;

			return _plugin.Call<int>("stopListeningNewConnections", serverId);
		}

		public int GetNewConnectionsId(int serverId, ref int min, ref int max)
		{
			if (!_isPluginAvailable)
				return 0;

			// get the JNI object pointer
			System.IntPtr obj = _plugin.GetRawObject();
			System.IntPtr cls = _plugin.GetRawClass();
			// get the JNI method id
			System.IntPtr mth = AndroidJNIHelper.GetMethodID(cls, "getNewConnectionsId");
			System.IntPtr results = AndroidJNI.NewIntArray(2);
			// build the JNI args array
			jvalue[] args = new jvalue[2];
			args[0].i = serverId;
			args[1].l = results;
			// call the JNI method
			int res = AndroidJNI.CallIntMethod(obj, mth, args);
			if (res == 2) {
				min = AndroidJNI.GetIntArrayElement(results, 0);
				max = AndroidJNI.GetIntArrayElement(results, 1);
			}
			return(res);
		}

		public int CloseServerConnection(int serverId, int connectionId) {
			if (!_isPluginAvailable)
				return 0;

			return _plugin.Call<int>("closeServerConnection", serverId, connectionId);
		}

		/// <summary>
		/// Sends data from a bluetooth server to a bluetooth client.
		/// </summary>
		/// <param name="configFile">serverId id of the source server.</param>
		/// <param name="configFile">clientId id of the destination client.</param>
		/// <param name="configFile">buffer data to send.</param>
		/// <param name="configFile">bufferLength size of the data to send.</param>
		/// <returns>0 if success,
		///            an error code if not (TBD).</returns>
		public int SendFromServer(int serverId, int clientId, byte[] buffer, int bufferLength) {
			if (!_isPluginAvailable)
				return 0;
			
			return _plugin.Call<int>("sendFromServer", serverId, clientId, buffer, bufferLength);
		}

		/// <summary>
		/// Receives data from a bluetooth client to a bluetooth server.
		/// </summary>
		/// <param name="configFile">serverId id of the destination server.</param>
		/// <param name="configFile">clientId id of the source client.</param>
		/// <param name="configFile">buffer data to receive.</param>
		/// <param name="configFile">bufferLength size of the data to receive.</param>
		/// <returns>0 if success,
		///            an error code if not (TBD).</returns>
		public int RecvFromServer(int serverId, int clientId, byte[] buffer, int bufferLength) {
			if (!_isPluginAvailable)
				return 0;

			AndroidJNI.PushLocalFrame(0);

			// get the JNI object pointer
			System.IntPtr obj = _plugin.GetRawObject();

			System.IntPtr cls = _plugin.GetRawClass();
			// get the JNI method id
			System.IntPtr mth = AndroidJNIHelper.GetMethodID(cls, "recvFromServer");
			// build the JNI args array
			jvalue[] args = new jvalue[4];
			args[0].i = serverId;
			args[1].i = clientId;
			args[2].l = AndroidJNI.ToByteArray(buffer);
			args[3].i = bufferLength;
			// call the JNI method
			int res = AndroidJNI.CallIntMethod(obj, mth, args);
			if (res == 0) {
				// copy from the JNI buffer back to the C# buffer
				byte[] ret = AndroidJNI.FromByteArray(args[2].l);
				ret.CopyTo(buffer, 0);
			}

			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
			return(res);
		}

		/// <summary>
		/// Starts a new bluetooth client.
		/// </summary>
		/// <returns>The id of the newly created client if success,
		///            an error code if not (TBD).</returns>
		public int StartClient(int maxCxnCycles, System.String remoteAddr, System.String remoteName, System.String guid, int channel) {
			if (!_isPluginAvailable)
				return 0;
			
			return _plugin.Call<int>("startClient", maxCxnCycles, remoteAddr, remoteName, guid, channel);
		}

		/// <summary>
		/// Stops a bluetooth client.
		/// </summary>
		/// <param name="clientId">clientId id of the client to stop.</param>
		/// <returns>0 if success,
		///            an error code if not (TBD).</returns>
		public int StopClient(int clientId) {
			if (!_isPluginAvailable)
				return 0;
			
			return _plugin.Call<int>("stopClient", clientId);
		}

		/// <summary>
		/// Sends data from a bluetooth client to its connected bluetooth server.
		/// </summary>
		/// <param name="clientId">clientId id of the source client.</param>
		/// <param name="buffer">buffer data to send.</param>
		/// <param name="bufferLength">bufferLength size of the data to send.</param>
		/// <returns>0 if success,
		///            an error code if not (TBD).</returns>
		public int SendFromClient(int clientId, byte[] buffer, int bufferLength) {
			if (!_isPluginAvailable)
				return 0;
			
			return _plugin.Call<int>("sendFromClient", clientId, buffer, bufferLength);
		}

		/// <summary>
		/// Receives data to a bluetooth client from its connected bluetooth server.
		/// </summary>
		/// <param name="clientId">clientId id of the destination client.</param>
		/// <param name="buffer">buffer data to receive.</param>
		/// <param name="bufferLength">bufferLength size of the data to receive.</param>
		/// <returns>0 if success,
		///            an error code if not (TBD).</returns>
		public int RecvFromClient(int clientId, byte[] buffer, int bufferLength) {
			if (!_isPluginAvailable)
				return 0;

			AndroidJNI.PushLocalFrame(0);

			// get the JNI object pointer
			System.IntPtr obj = _plugin.GetRawObject();
			System.IntPtr cls = _plugin.GetRawClass();
			// get the JNI method id
			System.IntPtr mth = AndroidJNIHelper.GetMethodID(cls, "recvFromClient");
			// build the JNI args array
			jvalue[] args = new jvalue[3];
			args[1].i = clientId;
			args[2].l = AndroidJNI.ToByteArray(buffer);
			args[3].i = bufferLength;
			// call the JNI method
			int res = AndroidJNI.CallIntMethod(obj, mth, args);
			if (res == 0) {
				// copy from the JNI buffer back to the C# buffer
				byte[] ret = AndroidJNI.FromByteArray(args[2].l);
				ret.CopyTo(buffer, 0);
			}

			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
			return(res);
		}


		/*

        /// <summary>
        /// Initializes the plugin and sets the Bluetooth service UUID.
        /// </summary>
        /// <param name="uuid">Bluetooth service UUID. Must be different for each game.</param>
        /// <returns>true on success, false if UUID format is incorrect.</returns>
        public static bool Initialize(string uuid) {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("initUuid", uuid);
        }

        /// <summary>
        /// Starts the server that listens for incoming Bluetooth connections. Must be called before <see cref="Network.InitializeServer(int,bool)"/>.
        /// </summary>
        /// <param name="port">Server port number. Must be the same as passed to <see cref="Network.InitializeServer(int,bool)"/>.</param>
        /// <returns>true on success, false on error.</returns>
        /// <exception cref="BluetoothNotEnabledException">Thrown if called when Bluetooth was not enabled.</exception>
        public static bool StartServer() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("startServer", "127.0.0.1");
        }

        /// <summary>
        /// Connects to a Bluetooth device. Must be called before <see cref="UnityEngine.Network.Connect(string,int)"/>.
        /// </summary>
        /// <param name="hostDeviceAddress">Address of host Bluetooth device to connect to.</param>
        /// <param name="port">Server port number. Must be the same as passed to  <see cref="UnityEngine.Network.Connect(string,int)"/>.</param>
        /// <returns>true on success, false on error/</returns>
        /// <exception cref="BluetoothNotEnabledException"> Thrown if called when Bluetooth was not enabled.</exception>
        public static bool Connect() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("startClient", "127.0.0.1");
        }

        /// <summary>
        /// Stops all Bluetooth connections. 
        /// Client will disconnect from the server.
        /// Server will break connection with all the clients and then halt. 
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool Stop() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("stop");
        }

        /// <summary>
        /// Starts listening for new incoming connections when
        /// in server mode. 
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool StartListening() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("startListening");
        }

        /// <summary>
        /// Stops listening for new incoming connections when
        /// in server mode. 
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool StopListening() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("stopListening");
        }

        /// <summary>
        /// Returns the current plugin <see cref="BluetoothMultiplayerMode"/>.
        /// </summary>
        /// <returns>The current plugin <see cref="BluetoothMultiplayerMode"/>.</returns>
        public static BluetoothMultiplayerMode GetCurrentMode() {
            if (!_isPluginAvailable)
                return BluetoothMultiplayerMode.None;

            return (BluetoothMultiplayerMode) _plugin.Call<byte>("getCurrentMode");
        }

        /// <summary>
        /// Opens a dialog asking user to make device discoverable 
        /// on Bluetooth for <paramref name="discoverabilityDuration"/> seconds. 
        /// This will also request the user to turn on Bluetooth if it was not enabled.
        /// </summary>
        /// <param name="discoverabilityDuration">
        /// The desired duration of discoverability (in seconds). Default value 120 seconds.
        /// On Android 4.0 and higher, value of 0 allows making device discoverable 
        /// "forever" (until discoverability is disabled manually or Bluetooth is disabled).
        /// </param>
        /// <returns>true on success, false on error.</returns>
        public static bool RequestEnableDiscoverability(int discoverabilityDuration = 120) {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("requestEnableDiscoverability", discoverabilityDuration);
        }

        /// <summary>
        /// Opens a dialog asking the user to enable Bluetooth. 
        /// It is recommended to use this method instead 
        /// of <see cref="EnableBluetooth"/> for more native experience.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool RequestEnableBluetooth() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("requestEnableBluetooth");
        }

        /// <summary>
        /// <para>Enables the Bluetooth adapter, if possible.</para>
        /// <para>Do not use this method unless you have provided a 
        /// custom GUI acknowledging user about the action. 
        /// Otherwise use <see cref="RequestEnableBluetooth"/>.</para>
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool EnableBluetooth() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("enableBluetooth");
        }

        /// <summary>
        /// Disables the Bluetooth adapter, if possible.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool DisableBluetooth() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("disableBluetooth");
        }

        /// <summary>
        /// <para>Returns whether the Bluetooth is available.</para>
        /// <para>Bluetooth can be unavailable if no Bluetooth adapter 
        /// is present, or if some error occurred.</para>
        /// </summary>
        /// <returns>true if Bluetooth connectivity is available, false otherwise.</returns>
        public static bool GetIsBluetoothAvailable() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("isBluetoothAvailable");
        }

        /// <summary>
        /// Returns whether if Bluetooth is currently enabled and ready for use.
        /// </summary>
        /// <returns>true if Bluetooth connectivity is available and enabled, false otherwise.</returns>
        public static bool GetIsBluetoothEnabled() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("isBluetoothEnabled");
        }

        /// <summary>
        /// <para>Shows the Bluetooth device picker dialog.</para>para>
        /// <para>Note: this method may fail some on exotic Android modifications like Amazon Fire OS.</para>
        /// </summary>
        /// <param name="showAllDeviceTypes">Whether to show all types or devices (including headsets, keyboards etc.) or only data-capable.</param>
        /// <returns>true on success, false on error.</returns>
        /// <exception cref="BluetoothNotEnabledException">Thrown if Bluetooth was not enabled.</exception>
        public static bool ShowDeviceList(bool showAllDeviceTypes = false) {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("showDeviceList", showAllDeviceTypes);
        }*/

        #endregion

        #region Helper methods

        #endregion
    }
}

#endif