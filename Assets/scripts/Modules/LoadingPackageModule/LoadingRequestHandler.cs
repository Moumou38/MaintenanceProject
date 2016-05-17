using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace loadingPackageModule
{
    public class LoadingRequestHandler : MonoBehaviour
    {

        public enum LoadingMode
        {
            LOCAL = 0,
            REMOTE = 1,
            ERROR = 2, 
            NONE = 3
        }

        public void processRequestBundle(string name, string URL = "")
        {
            if (!m_DownloadingWWWs.ContainsKey(name))
            {
                StartCoroutine(loadAssetBundle(name, URL));
            }
        }

        public int getDownloadingObjectCount()
        {
            if (m_DownloadingWWWs != null)
                return m_DownloadingWWWs.Count; 
            else return 0; 
        }

        public bool isBundleDownloading(string iName)
        {
            if (m_DownloadingWWWs != null)
                return m_DownloadingWWWs.ContainsKey(iName); 
            else
                return false; 
        }

        public void setLocalPath(string iPath)
        {
            m_localFolderPath = iPath;
        }

        IEnumerator loadAssetBundle(string iName, string iURL = "")
        {
            LoadingMode status = LoadingMode.NONE; 
            string url = "";
            string path = ""; 

            if (m_localFolderPath != "")
            {
#if UNITY_ANDROID                
               
                path = "/mnt/sdcard/" + m_localFolderPath; 
#else
                path = System.IO.Directory.GetParent(".")  + m_localFolderPath; 
#endif
            }
            else
            {
#if UNITY_ANDROID
                path = "/mnt/sdcard/AssetBundles/";
#else
                path = System.IO.Directory.GetParent(".") + "AssetBundles/"; 
#endif



            }

            path = path.Replace(@"\", "/");
            if (iName.Contains(".unity3d") == false)
                iName += ".unity3d"; 
            // Debug.Log("//////////////////// PATH : " + path); 
            if (System.IO.Directory.Exists(path) && System.IO.File.Exists(path + iName))
            {
                status = LoadingMode.LOCAL;
                url = "file://" + path + iName;
                
            }
            else if (iURL != "")
            {
                url = iURL;
                yield return StartCoroutine("CheckConnection");  // we don't go further as long as the internet connection is not verified
                status = LoadingMode.REMOTE; 
            }
            else
            {
                // the URL is not correct and the bundle doesn't exist locally
                if (onAssetBundleLoaded != null)
                {
                    onAssetBundleLoaded(null, LoadingMode.ERROR);
                }
                yield return null;  // we stop the coroutine
            }

            // Download the file from the URL. It will not be saved in the Cache
            using (WWW download = new WWW(url))
            {              
                yield return download;

                if (download.error != null)
                {
                    if (onAssetBundleLoaded != null)
                    {
                        Debug.Log("//////// FAILED TO DOWNLOAD BUNDLE");
                        onAssetBundleLoaded(null, LoadingMode.ERROR);
                    }

                }
                else
                {                   
                    if (download.assetBundle != null && status == LoadingMode.REMOTE)
                    {

                        byte[] t = new byte[download.bytes.Length]; 
                        download.bytes.CopyTo(t,0);   
                        // We save locally the bundle
                        m_assetBundleSerializer.saveAssetBundleLocally(t, path, iName);                        
                    }
                    if (onAssetBundleLoaded != null)
                    {
                        onAssetBundleLoaded(download.assetBundle, status, iName);
                    }
                }
            }
                                           
        }

        IEnumerator CheckConnection()
        {
            const float timeout = 10f;
            float startTime = Time.timeSinceLevelLoad;
            Ping ping = new Ping("199.59.148.82");

            while (true)
            {
                if (ping.isDone)
                {
                    //internet available, we can return to the loading process
                    yield break;
                }
                if (Time.timeSinceLevelLoad - startTime > timeout)
                {
                    if (onAssetBundleLoaded != null)
                    {
                        onAssetBundleLoaded(null, LoadingMode.ERROR);
                    }
                    StopCoroutine("loadAssetBundle");
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private new void Awake()
        {
            m_assetBundleSerializer = gameObject.AddComponent<AssetBundleSerializer>();
        }

        

        
        public delegate void AssetBundleLoaded(AssetBundle iBundle, LoadingMode iStatus, string name = "");
        public event AssetBundleLoaded onAssetBundleLoaded;
        Dictionary<string, KeyValuePair<WWW, LoadingMode>> m_DownloadingWWWs = new Dictionary<string, KeyValuePair<WWW, LoadingMode>>(); // used in case we download several bundles at the same time
        private static string m_localFolderPath = "";
        AssetBundleSerializer m_assetBundleSerializer; 
    }
}
