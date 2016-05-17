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
// - Fichier créé le 4/28/2015 4:11:14 PM
// - Dernière modification le 4/28/2015 4:11:14 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class PadControllerCallbacks
    /// </summary>
    public class PadControllerCallbacks
    {
		public delegate void GlassEventOnCaptureReceived(string stepPath, byte[] image);
		public event GlassEventOnCaptureReceived OnCaptureReceived;
		public void CallOnCaptureReceived(string step, byte[] image)
		{
			if(OnCaptureReceived != null)
				OnCaptureReceived(step, image);
		}

		public delegate void WatchEventScreenChanged(int screenIndex);
		public event WatchEventScreenChanged OnWatchScreenChanged;
		public void CallOnWatchScreenChanged(int screenIndex)
		{
			if(OnWatchScreenChanged != null)
				OnWatchScreenChanged(screenIndex);
		}
	
		public delegate void WatchEventStepPathChanged(string stepPath);
		public event WatchEventStepPathChanged OnWatchStepPathChanged;
		public void CallOnWatchStepPathChanged(string stepPath)
		{
			if(OnWatchStepPathChanged != null)
				OnWatchStepPathChanged(stepPath);
		}

		public delegate void BluetoothEventOnServerCreated(bool created);
		public event BluetoothEventOnServerCreated OnServerCreated;
		public void CallOnOnServerCreated(bool created)
		{
			if(OnServerCreated != null)
				OnServerCreated(created);
		}

		public delegate void BluetoothEventOnConnectionResult(bool connected);
		public event BluetoothEventOnConnectionResult OnConnectionResult;
		public void CallOnOnOnConnectionResult(bool connected)
		{
			if(OnConnectionResult != null)
				OnConnectionResult(connected);
		}
	}
}
