// Copyright 2015 dassault
//
// - Description
//
// - Namespace(s)
// - dassault
//
// - Auteurs
// - de VERDELHAN
//
// - Fichier créé le 
//		\date #CREATIONDATE#
// - Dernière modification le 
//		\date #CREATIONDATE#
//@END-HEADER
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace dassault
{
	/// <summary>
	/// classe effectuant le remplacement de keyword lors de la création d'un nouvel asset
	/// </summary>
	public class KeywordReplace : UnityEditor.AssetModificationProcessor {
		
		public static void OnWillCreateAsset ( string path ) {
			path = path.Replace( ".meta", "" );
			int index = path.LastIndexOf( "." );
			string file = path.Substring( index );
			if ( file != ".cs" && file != ".js" && file != ".boo" ) return;
			index = Application.dataPath.LastIndexOf( "Assets" );
			path = Application.dataPath.Substring( 0, index ) + path;
			file = System.IO.File.ReadAllText( path );
			
			file = file.Replace( "#CREATIONDATE#", System.DateTime.Now + "" );
			file = file.Replace( "#COMPANYNAME#", PlayerSettings.companyName );
			file = file.Replace( "#DEFAULTNAMESPACE#", PlayerSettings.companyName.ToLower() );

			System.IO.File.WriteAllText( path, file );
			AssetDatabase.Refresh();
		}
	}
}
