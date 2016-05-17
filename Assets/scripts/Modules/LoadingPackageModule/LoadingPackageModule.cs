using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using dassault;
using System.Xml;
using System.IO; 

namespace loadingPackageModule
{  

    public class LoadingPackageModule : ModuleInstance
    {
      
        public enum LoadingState
        {
            NONE = 0,
            START = 1,
            STOP = 2,
            LOADING = 3
        }

        public override void OnAllModuleLoaded(ModuleRepository repository)
        {
            Activate(true);
            m_loadingRequestHandler.onAssetBundleLoaded += handleResponse;
            m_bundleHandler.onExtractionDone += loadingCallback; 

            m_currentLoadingState = LoadingState.NONE;
            m_bundles = new Dictionary<string, AssetBundle>(); 
            m_loadingRequestHandler.setLocalPath(getAssetBundleFolderPath());
            //getDefaultBundle(); 
        }

        public List<string> initializeLocalAssetBundleList()
        {
            List<string> BundleList = new List<string>();
            string path = "/mnt/sdcard/" + getAssetBundleFolderPath(); 
            if (System.IO.Directory.Exists(path))
            {

                string[] bundles = System.IO.Directory.GetFiles(path);

                foreach (string s in bundles)
                {
                    if (s.Contains(".unity3d"))
                    {
                        string scenario = s.Replace(path, "");
                        BundleList.Add(scenario);
                    }
                }
            } 
            return BundleList; 
        }

        public static string getAssetBundleFolderPath()
        {
            XmlDocument document = new XmlDocument();
            string path = ""; 

#if UNITY_ANDROID
            path = "/mnt/sdcard/GlassApplication/";
#endif
#if UNITY_EDITOR
            path = ""; 
#endif

            document.Load(path + "config.xml"); 

            if (document != null)
            {
                XmlNode root = document.FirstChild;
                if (root == null)
                {
                    Debug.Log("Error while reading file");
                    return "";
                }
                else
                {
                    foreach (XmlNode son in root)
                    {
                        if (son.Name == "AssetBundleFolder")
                        {
                            return son.InnerText; 
                        }
                    }
                    return "";
                }

            }
            else
                return ""; 
        }

        void getDefaultBundle()
        {

            XmlDocument document = new XmlDocument();
            string path = "";

#if UNITY_ANDROID
            path = "/mnt/sdcard/GlassApplication/";
#endif
#if UNITY_EDITOR
            path = "";
#endif

            document.Load(path + "config.xml"); 

            if (document != null)
            {
                XmlNode root = document.FirstChild;
                if (root == null)
                {
                    Debug.Log("Error while reading file");
                }
                else
                {
                    foreach (XmlNode son in root)
                    {
                        if (son.Name == "DefaultStartUpBundle")
                        {
                            m_loadingRequestHandler.processRequestBundle(son.Attributes["name"].Value, son.Attributes["URL"].Value);
                            return; 
                        }
                    }

                }

            }
        }

        public override HashSet<string> GetModuleDependencies()
        {
            HashSet<string> result = new HashSet<string>();
            return result;
        }

        public void onStartInspection(string iName)
        {
            if (m_bundles.Count == 0)
            {
                m_currentLoadingState = LoadingState.NONE;
                Debug.LogError("No asset bundle ready : sending request");
                if (iName != "")
                {
                    pending_opening = true;
                    FileToOpen = iName;

                    if (onLoadingDataState != null)
                        onLoadingDataState(LoadingState.START);

                    m_loadingRequestHandler.processRequestBundle(iName);
                }
            }
            else
            {
                if (m_bundles.ContainsKey(iName))
                {
                    
                    m_bundleHandler.extractData(m_bundles[iName], iName);
                    m_currentLoadingState = LoadingState.LOADING;
                }
                else
                {
                    Debug.LogError("AssetBundle not found"); 
                }
            }
            
        }

        public void onStopInspection()
        {
            m_currentLoadingState = LoadingState.NONE; 

        }

        public LoadingState getCurrentLoadingState()
        {
            return m_currentLoadingState; 
        }

        public void loadingCallback(List<UnityEngine.Object> oExtractedObjects, string iName)
        {

            if(onLoadingDataState != null)
                onLoadingDataState(LoadingState.STOP);

            if (onAssetDeployment != null && oExtractedObjects != null)
            {
                onAssetDeployment(oExtractedObjects);
            }
            // UNLOAD BUNDLE HERE

            // Unload the AssetBundle to conserve memory ; the extracted Object are still here
            m_bundles[iName].Unload(false);
            m_bundles.Remove(iName); 
        }
        ////////// PRIVATE ////////////
        

        void handleResponse(AssetBundle iBundle, LoadingRequestHandler.LoadingMode iStatus, string name ="" )
        {
            if (iStatus != LoadingRequestHandler.LoadingMode.ERROR)
            {
                m_bundles.Add(name, iBundle);
                Debug.Log("Bundle Loaded : waiting for extraction");

                if (iStatus == LoadingRequestHandler.LoadingMode.REMOTE && onAssetBundleDownloaded != null)
                {
                    onAssetBundleDownloaded(name); 
                }

                if (pending_opening == true)
                {
                    onStartInspection(FileToOpen); 
                    FileToOpen = ""; 
                    pending_opening = false; 
                }
            }
            else
            {
                Debug.LogError("Failed to download bundle"); 
            }
        }

        public void requestAssetBundle(string iName, string iURL = "")
        {
            m_loadingRequestHandler.processRequestBundle(iName, iURL); 
        }

        private new void Awake()
        {
            base.Awake();
            
            m_bundleHandler =  new AssetBundleHandler();
            m_loadingRequestHandler = gameObject.AddComponent<LoadingRequestHandler>(); 
        }

        // this event will be used to to send events to the other modules during the data extraction
        // we want to freeze the user interactions during loading. Therefore we need to be notified
        public delegate void LoadingDataState(LoadingState iStatus);
        public event LoadingDataState onLoadingDataState;

        public delegate void OnAssetDeployment(List<Object> list);
        public event OnAssetDeployment onAssetDeployment;

        public delegate void OnAssetBundleDownloaded(string LoadedBundle);
        public event OnAssetBundleDownloaded onAssetBundleDownloaded; 
        
        bool pending_opening = false;
        string FileToOpen = ""; 
        LoadingState m_currentLoadingState; 
        Dictionary<string, AssetBundle> m_bundles;
        AssetBundleHandler m_bundleHandler;
        LoadingRequestHandler m_loadingRequestHandler;
    }
}
