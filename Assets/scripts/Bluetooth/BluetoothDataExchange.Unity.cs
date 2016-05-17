#if UNITY_ANDROID
using UnityEngine;

namespace dassault {
    /// <summary>
    /// A core class that wraps Java methods of Android plugin.
    /// </summary>
	public sealed partial class BluetoothDataExchange : MonoBehaviour {
        /// <summary>
        /// Java Name of main plugin facade class.
        /// </summary>
        private const string kPluginClassName = "com.theoris.unity.bluetoothdataexchange.BluetoothDataExchange";

        /// <summary>
        /// The name of the GameObject, used for receiving messages from Java side.
        /// </summary>
		private static readonly string kGameObjectName = typeof(BluetoothDataExchange).Name;

        /// <summary>
        /// A reference to the Java BluetoothMediator object .
        /// </summary>
        private static readonly AndroidJavaObject _plugin;

        /// <summary>
        /// Whether the plugin is available and was loaded successfully.
        /// </summary>
        private static readonly bool _isPluginAvailable;

        /// <summary>
        /// A reference to singleton instance.
        /// </summary>
		private static BluetoothDataExchange _instance;

        /// <summary>
		/// Initializes <see cref="AndroidBluetoothDataExchange"/> class.
        /// Retrieves a pointer to the Java plugin object.
        /// Initalizes the singleton instance on the first usage of the class.
        /// </summary>
		static BluetoothDataExchange() {
            _plugin = null;
            _isPluginAvailable = false;

            try {
                UpdateInstance();
            } catch {
                // Happens when this static constructor is called from a GameObject being created.
                // Just ignoring, as this is intended.
            }

			#if !UNITY_EDITOR && UNITY_ANDROID
			// Retrieve BluetoothDataExchange singleton instance
            try {
				using (AndroidJavaClass BluetoothDataExchange = new AndroidJavaClass("com.theoris.unity.bluetoothdataexchange.BluetoothDataExchange")) {
					if (BluetoothDataExchange!=null) {
						_plugin = BluetoothDataExchange.CallStatic<AndroidJavaObject>("getSingleton");
						if(_plugin!=null)
						   _isPluginAvailable = true;
                    }
                }
            } catch {
                Debug.LogError("AndroidBluetoothMultiplayer initialization failed. Probably .jar not present?");
                throw;
            }
			#endif
        }

		public static BluetoothDataExchange GetInstance() {
			return(_instance);
		}

        /// <summary>
        /// Tries to find an existing instance in the scene, 
        /// and creates one if there were none.
        /// </summary>
        private static void UpdateInstance() {
            if (_instance != null)
                return;

            // Trying to find an existing instance in the scene
			_instance = (BluetoothDataExchange) FindObjectOfType(typeof(BluetoothDataExchange));

            // Creating a new instance in case there are no instances present in the scene
            if (_instance != null)
                return;

            GameObject gameObject = new GameObject(kGameObjectName);
			_instance = gameObject.AddComponent<BluetoothDataExchange>();

            // Make it hidden and indestructible
            gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
        }


        private void Awake() {
            // Kill other instances
			if (FindObjectsOfType(typeof(BluetoothDataExchange)).Length > 1) {
                Debug.LogError("Multiple " + kGameObjectName + " instances found, destroying...");
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;

            // Set the GameObject name to the class name for UnitySendMessage
            gameObject.name = kGameObjectName;

            // We want this object to persist across scenes
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(this);
        }
    }
}

#endif