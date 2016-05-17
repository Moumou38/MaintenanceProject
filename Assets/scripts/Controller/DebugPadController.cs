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
// - Fichier créé le 4/28/2015 4:23:19 PM
// - Dernière modification le 4/28/2015 4:23:19 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using System.IO;

namespace dassault
{
    /// <summary>
    /// description for class DebugPadController
    /// </summary>
    public class DebugPadController : PadController
    {
        // Update is called once per frame
		protected override void PreUpdate()
        {
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				m_glassCallbacks.CallOnPreviousStep();
			}
			else if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				m_glassCallbacks.CallOnNextStep();
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				m_glassCallbacks.CallOnSwitchLocalization();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad4))
			{
				m_glassCallbacks.CallOnCurrentViewPrevious();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad6))
			{
				m_glassCallbacks.CallOnCurrentViewNext();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad8))
			{
				m_glassCallbacks.CallOnSwitchReferences();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad5))
			{
				m_glassCallbacks.CallOnSwitchTools();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad2))
			{
				m_glassCallbacks.CallOnSwitchComments();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad7))
			{
				m_glassCallbacks.CallOnTakeScreenshot();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad9))
			{
				m_glassCallbacks.CallOnSwitchAnnotation();
			}
			else if(Input.GetKeyDown(KeyCode.G))
			{
				m_glassCallbacks.CallOnSwitchGUI();
			}
			else if(Input.GetKeyDown(KeyCode.L))
			{
				m_glassCallbacks.CallOnLoadProject("scenario1");
			}
			else if(Input.GetKeyDown(KeyCode.P))
			{
				m_padCallbacks.CallOnWatchStepPathChanged("1/1>4/5>1/2>6/11");
			}
			else if(Input.GetKeyDown(KeyCode.R))
			{
				byte[] image = SimulateImageReceptionFromServer();
				if(image != null)
					m_glassCallbacks.CallOnAnnotationReceived("1/1>4/5>1/2>6/11", image);
			}
			else if(Input.GetKeyDown(KeyCode.C))
			{
				string file = "c:\\temp\\cameraCapture.png";
				if(File.Exists(file))
				{
					byte[] imageContent = File.ReadAllBytes(file);
					m_padCallbacks.CallOnCaptureReceived("1/1>4/5>1/2>6/11", imageContent);
				}
				else
				{
					Debug.LogWarning("le fichier de capture " + file + " n'existe pas");
				}
			}
			else if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				m_padCallbacks.CallOnWatchScreenChanged(0);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				m_padCallbacks.CallOnWatchScreenChanged(1);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				m_padCallbacks.CallOnWatchScreenChanged(2);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha8))
			{
				m_glassCallbacks.CallOnLoadBookmark(0);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha9))
			{
				m_glassCallbacks.CallOnLoadBookmark(1);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha0))
			{
				m_glassCallbacks.CallOnLoadBookmark(2);
			}
			else if(Input.GetKeyDown(KeyCode.F9))
			{
				m_padCallbacks.CallOnOnServerCreated(false);
			}
			else if(Input.GetKeyDown(KeyCode.F10))
			{
				m_padCallbacks.CallOnOnServerCreated(true);
			}
			else if(Input.GetKeyDown(KeyCode.F11))
			{
				m_padCallbacks.CallOnOnOnConnectionResult(false);
			}
			else if(Input.GetKeyDown(KeyCode.F12))
			{
				m_padCallbacks.CallOnOnOnConnectionResult(true);
			}
		}

		private byte[] SimulateImageReceptionFromServer()
		{
			string file = "c:\\temp\\annotation.png";
			if(File.Exists(file))
			{
				byte[] fileContent = File.ReadAllBytes(file);
				return fileContent;
			}
			return null;
		}

		public override void SendAnnotationBackToGlasses(string stepPath, byte[] image)
		{
			File.WriteAllBytes("c:\\temp\\annotation.png", image);
		}

		public override void ConnectToDevice(string deviceName, string deviceAddress)
		{
			Debug.Log ("fake connection to device " + deviceName + " with address " + deviceAddress + "press F11 to accept connection, F12 to fail");
		}
    }
}
