using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace dassault
{

    public class StreamingServerAdapter
    {

#if UNITY_ANDROID
    [DllImport("StreamServerUnityAdapter", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Initialize")]
    private static extern int Initialize(System.String configFilePath);

    [DllImport("StreamServerUnityAdapter", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StartStreaming")]
    private static extern int StartStreaming();

    [DllImport("StreamServerUnityAdapter", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StopStreaming")]
    private static extern int StopStreaming();

    [DllImport("StreamServerUnityAdapter", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetParameters")]
    private static extern int GetParameters([In, Out]ref System.IntPtr url, ref int urlSize, ref uint httpPort);
#else
        [DllImport("StreamServerUnityAdapter.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Initialize")]
        private static extern int Initialize(System.String configFilePath);

        [DllImport("StreamServerUnityAdapter.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StartStreaming")]
        private static extern int StartStreaming();

        [DllImport("StreamServerUnityAdapter.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "StopStreaming")]
        private static extern int StopStreaming();

        [DllImport("StreamServerUnityAdapter.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetParameters")]
        private static extern int GetParameters([In, Out]ref System.IntPtr url, ref int urlSize, ref uint httpPort);
#endif

        public StreamingServerAdapter()
        {
            // initialise the streaming server library
            string configurationFolder;
#if UNITY_ANDROID
        configurationFolder = "/mnt/sdcard/SupportDistant/";
#else
            configurationFolder = Application.dataPath + "/";
#endif
            m_configFile = configurationFolder + "StreamServerParameters.xml";
            Debug.Log("streaming server config file: " + m_configFile);
            if (System.IO.File.Exists(m_configFile))
            {
                Debug.Log("The streaming configuration file exist.");
                
            }
            else
            {
                Debug.LogError("The streaming configuration file doesn't exist.");
                m_serverUrl = "failed";
                // nothing more to do ! fail....
                return;
            }

            if (Initialize(m_configFile) == 0)
            {
                Debug.Log("streaming server initialized, retrieving connection info");

                System.IntPtr ptr = new System.IntPtr();
                int urlSize = 0;
                uint httpPort = 0;
                if (GetParameters(ref ptr, ref urlSize, ref httpPort) == 0)
                {
                    m_serverUrl = Marshal.PtrToStringAnsi(ptr);
                    m_serverHttpPort = System.Convert.ToInt32(httpPort);

                    if(m_serverHttpPort != 0)
                    {
                        Debug.Log("RTSP over HTTP tunneling enabled on port " + m_serverHttpPort.ToString());
                    }
                    else
                    {
                        Debug.Log("RTSP over HTTP tunneling disabled.");
                    }
                }
                else
                {
                    Debug.LogError("unable to retrieve streaming server connection info");
                }
            }
            else
            {
                m_serverUrl = "failed";
                Debug.LogError("streaming server Initialization failed");
            }
        }

        public bool startServer(out string url, out int port)
        {
            // Port to make rtsp over http. 0 if not used.
            port = m_serverHttpPort;

            url = m_serverUrl;

            // ask the streaming server to start and retrieve the server connection info
            if (StartStreaming() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void stopServer()
        {
            StopStreaming();  
        }

        string m_configFile;
        string m_serverUrl;
        int m_serverHttpPort;
    }

}
