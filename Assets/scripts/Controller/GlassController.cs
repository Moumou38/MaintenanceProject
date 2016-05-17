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
// - Fichier créé le 4/28/2015 2:10:52 PM
// - Dernière modification le 4/28/2015 2:10:52 PM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace dassault
{
	/// <summary>
	/// description for class DebugGlassController
	/// </summary>
	public abstract class GlassController : ApplicationController
	{
		#region Public methods
		public virtual GlassController Init(GlassControllerCallbacks callbacks)
		{
			m_callbacks = callbacks;

			return this;
		}

		public abstract void OnAnnotationRequest(string stepPath, byte[] image);

		public abstract void OnCurrentStepChanged(string stepPath);

		public abstract void OnCommentViewVisibilityChanged(bool visible);
		public abstract void OnToolViewVisibilityChanged(bool visible);
		public abstract void OnReferenceViewVisibilityChanged(bool visible);
		public abstract void OnAnnotationViewVisibilityChanged(bool visible);
		public abstract void OnLocalizationViewVisibilityChanged(bool visible);

        public abstract void OnStreamingServerStart(string url, int port);

        public abstract void OnScenariiStatus(List<string> scenarii);
        public abstract void addScenarioToList(string scenario);

        #endregion Public methods

        #region Attributs
        protected GlassControllerCallbacks m_callbacks;
		#endregion Attributs
	}
}
