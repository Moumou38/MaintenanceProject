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
// - Fichier créé le 4/21/2015 9:56:14 AM
// - Dernière modification le 4/21/2015 9:56:14 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class AnnotationModule
    /// </summary>
    public class AnnotationModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			Activate(false);
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			return result;
		}

		private new void Awake()
		{
			base.Awake();
		}

		public void SetTexture(byte[] textureAsPngFileContent)
		{
			Texture2D texture = new Texture2D(4,4);
			texture.LoadImage(textureAsPngFileContent); // LoadImage replace the texture content
			m_backgroundMaterial.mainTexture = texture;
			/*Vector2 center = m_camera.rect.center;
			Rect r = new Rect(0, 0, texture.width / (float)Screen.width, texture.height / (float)Screen.height);
			r.center = center;
			m_camera.rect = r;*/
			float aspectRatio = texture.width / (float)texture.height;
			m_quad.transform.localScale = new Vector3(aspectRatio, 1, 1);
			m_drawHandler.Reset();
		}

		public void SetCameraViewport(Rect cameraViewport)
		{
			m_camera.rect = cameraViewport;
		}

		public byte[] TakeScreenshot()
		{
			return m_drawHandler.TakeScreenshot();
		}

		public void SetLineSize(float lineSize)
		{
			m_drawHandler.SetCurrentSize(lineSize);
		}

		public void SetLineColor(Color lineColor)
		{
			m_drawHandler.SetCurrentColor(lineColor);
		}

		[SerializeField] private DrawHandler m_drawHandler;
		[SerializeField] private Camera m_camera;
		[SerializeField] private Material m_backgroundMaterial;
		[SerializeField] private GameObject m_quad;

    }
}
