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
// - Fichier créé le 6/5/2015 12:11:13 PM
// - Dernière modification le 6/5/2015 12:11:13 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class ScenarioState
    /// </summary>
    public class ScenarioState
    {
		public ScenarioState(string stepPath, bool isCommentVisible, bool isReferencesVisible, bool isToolsVisible, bool isAnnotationsVisible, int commentIndex, int referenceIndex, int toolIndex, int annotationIndex, bool isLocalizationVisible, bool isGuiVisible)
		{
			StepPath = stepPath;
			IsCommentsVisible = isCommentVisible;
			IsReferencesVisible = isReferencesVisible;
			IsToolsVisible = isToolsVisible;
			IsAnnotationsVisible = isAnnotationsVisible;
			IsLocalizationVisible = isLocalizationVisible;
			CurrentCommentIndex = commentIndex;
			CurrentReferenceIndex = referenceIndex;
			CurrentToolIndex = toolIndex;
			CurrentAnnotationIndex = annotationIndex;
			IsGuiVisible = isGuiVisible;
		}

		public readonly string StepPath;
		public readonly bool IsCommentsVisible;
		public readonly bool IsReferencesVisible;
		public readonly bool IsToolsVisible;
		public readonly bool IsAnnotationsVisible;
		public readonly bool IsLocalizationVisible;
		public readonly int CurrentCommentIndex;
		public readonly int CurrentReferenceIndex;
		public readonly int CurrentToolIndex;
		public readonly int CurrentAnnotationIndex;
		public readonly bool IsGuiVisible;
	}
}
