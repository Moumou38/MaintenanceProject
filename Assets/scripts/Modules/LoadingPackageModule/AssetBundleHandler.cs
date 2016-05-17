using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace loadingPackageModule
{
    public class AssetBundleHandler
    {

        public AssetBundleHandler()
        {
            
        }


        public void extractData(AssetBundle iBundle, string iName)
        {
            if (iBundle != null)
            {
                List<Object> objects = new List<Object>();
                foreach (Object o in iBundle.LoadAllAssets<Object>())
                {
                    objects.Add(o);
                }

                if (onExtractionDone != null)
                    onExtractionDone(objects, iName); 
                
            }
        }

       
        public float getProgress()
        {
            return m_progress; 
        }

        ///// PRIVATE //////
        public delegate void OnExtractionDone(List<Object> list, string iName);
        public event OnExtractionDone onExtractionDone;

        TextAsset XMLFile = null; 
        float m_progress = 0f; 
        protected AssetBundleRequest m_Request = null;
    }

}
