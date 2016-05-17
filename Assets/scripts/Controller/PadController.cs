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
// - Fichier créé le 4/28/2015 2:10:11 PM
// - Dernière modification le 4/28/2015 2:10:11 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class PadController
    /// </summary>
	public abstract class PadController : ApplicationController
    {
        #region Public methods
        public virtual PadController Init(PadControllerCallbacks padCallback, GlassControllerCallbacks glassCallback)
        {
            m_padCallbacks = padCallback;
            m_glassCallbacks = glassCallback;

            return this;
        }

		public abstract void SendAnnotationBackToGlasses(string stepPath, byte[] image);
		public abstract void ConnectToDevice(string deviceName, string deviceAddress);
		#endregion Public methods

        #region Attributs
        protected GlassControllerCallbacks m_glassCallbacks;
        protected PadControllerCallbacks m_padCallbacks;
        #endregion Attributs
    }
}
