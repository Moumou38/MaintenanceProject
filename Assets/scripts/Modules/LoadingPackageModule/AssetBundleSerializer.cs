using UnityEngine;
using System.Collections;
using System.IO;
using System;


namespace loadingPackageModule
{

    public class AssetBundleSerializer : MonoBehaviour
    {


        public void saveAssetBundleLocally(byte[] iBundleBytes, string iPath, string iName)
        {
            StartCoroutine(writeOnDisk(iBundleBytes, iPath, iName));
        }


        IEnumerator writeOnDisk(byte[] iBundleBytes, string iPath, string iName)
        {
            try
            {
                // create the directory if it doesn't already exist
                if (!System.IO.Directory.Exists(iPath))
                {
                    System.IO.Directory.CreateDirectory(iPath);
                }
                // write out the file
                string filePath = "";
                filePath = Path.Combine(iPath, iName);

                if (!System.IO.File.Exists(filePath))
                    File.WriteAllBytes(filePath, iBundleBytes);    // write the object out to disk
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't save file: " + System.Environment.NewLine + e);
            }

            yield return null;
        }

    }

}