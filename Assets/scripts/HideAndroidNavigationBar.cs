// Copyright 2015 Dassault
//
// - Description
//     
//
// - Namespace(s)
//    dassault
//
// - Auteurs
//    \author Michel de Verdelhan <mdeverdelhan@theoris.fr>
//
// - Fichier créé le 5/20/2015 10:47:41 AM
// - Dernière modification le 5/20/2015 10:47:41 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class HideAndroidNavigationBar
    /// </summary>
    public class HideAndroidNavigationBar : MonoBehaviour
    {
#if UNITY_ANDROID
		private void Start()
		{
			HideNavigationBar();
		}

		private void OnApplicationPause(bool isPaused)
		{
			if( !isPaused )
			{
				HideNavigationBar();
			}
		}

		private void HideNavigationBar()
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(SetWindowFlag));
		}
		
		private void SetWindowFlag()
		{
			AndroidJNI.AttachCurrentThread();
			AndroidJavaClass cView = new AndroidJavaClass("android.view.View");
			AndroidJavaObject oAct = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject oWindow = oAct.Call<AndroidJavaObject>("getWindow");
			AndroidJavaObject oLayoutParams = oWindow.Call<AndroidJavaObject>("getAttributes");
			int flags = oLayoutParams.Get<int>("flags");
			int mask = System.Convert.ToInt32("0x80000000", 16);
			flags |= mask;
			oLayoutParams.Set<int>("flags", flags);
			oWindow.Call ("setAttributes", oLayoutParams);
		}
#endif
	}
}
