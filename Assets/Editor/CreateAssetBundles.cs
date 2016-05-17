// Copyright 2015 Dassault
//
// - Description
//    This file adds an "AssetBundles" toolbar button where we can choose how to build our own assetbundles  
//    
//
// - Namespace(s)
//   
//
// - Auteurs
//    \author Monia Arrada <marrada@theoris.fr>
//
//@END-HEADER

using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Xml;
using System.Collections.Generic;

public class CreateAssetBundles : EditorWindow
{

    string m_XMLFilePath = "";
    string m_outputPath = "";
    string m_bundleName = "";
    List<string> m_nameList;
    List<string> m_pathList;
    bool m_read = false;
    public Vector2 m_scrollPosition;
    bool bsmoothDependencies = false;
    bool old_bsmoothDependencies = false; 
    
    [MenuItem("AssetBundles/Open AssetBundle Build Window")]
    public static void ShowWindow()
    {
        // opens a window to build an AssetBundle using an XML file to collect the data
        EditorWindow e = EditorWindow.GetWindow(typeof(CreateAssetBundles), false, "AssetBundleCreation Window");
    }

    void parseDocumentName(string iPath)
    {
        string s = iPath;
        int cpt = 0; 
        foreach (char c in iPath)
        {
            cpt++; 
            if (c == '/')
            {
                cpt = 0; 
            }
            if (c == '.')
            {
                s = s.Substring(iPath.Length - cpt - 3);
                break; 
            }

        }

        m_nameList.Add(s); 
    }

    void readXMLFile()
    {
        m_nameList = new List<string>();
        m_pathList = new List<string>();

        try
        {       
            string relativepath = m_XMLFilePath.Substring(Application.dataPath.Length - "Assets".Length);
            XmlDocument document = new XmlDocument();
            document.Load(relativepath);
            parseDocumentName(relativepath); 
            
            XmlNode root = document.FirstChild;
            if (root == null)
                Debug.Log("Error while reading file");
            else
                parseXML(root as XmlElement);

            m_read = true;

        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
    }

    void parseXML(XmlElement element)
    {


        // read file and stock paths
        //<LINK>picture....</LINK>
        //<REFERENCE> animation
        //<TARGET> 3D

        foreach (XmlNode son in element.ChildNodes)
        {
            if (son.Name == "TARGET")
            {
                if (!m_nameList.Contains(son.InnerText))
                {
                    string[] stringSeparators = new string[] { ";" };
                    string[] tab;
                    tab = son.InnerText.Split(new char[] { ';' });

                    foreach (string s in tab)
                        m_nameList.Add(s);
                }
            }
            else if (son.Name == "REFERENCES")
            {
                ReadReferences(son as XmlElement);
            }
            else if (son.Name == "STEPS")
            {
                ReadSubSteps(son as XmlElement);
            }
        }
    }

    void ReadSubSteps(XmlElement element)
    {
        foreach (XmlNode son in element.ChildNodes)
        {
            if (son.Name == "STEP")
            {
                parseXML(son as XmlElement);
            }
        }
    }

    void ReadReferences(XmlElement element)
    {
        foreach (XmlNode reference in element.ChildNodes)
        {
            if (reference.Name == "REFERENCE")
            {
                string type = (reference as XmlElement).GetAttribute("type");
                if (type == "figure")
                {
                    foreach (XmlNode son in reference.ChildNodes)
                    {
                        if (son.Name == "LINK")
                        {
                            if (!m_nameList.Contains(son.InnerText))
                            {
                                string[] stringSeparators = new string[] { ";" };
                                string[] tab;
                                tab = son.InnerText.Split(new char[] { ';' });

                                foreach (string s in tab)
                                    m_nameList.Add(s);
                            }
                        }
                    }
                }
                else if (type == "animation")
                {
                    foreach (XmlNode son in reference.ChildNodes)
                    {
                        if (son.Name == "NAME")
                        {
                            if (!m_nameList.Contains(son.InnerText))
                            {
                                m_nameList.Add(son.InnerText);
                            }
                        }
                    }
                }
            }
        }
    }


    void OnGUI()
    {
        //Object currentObject = null;

        EditorGUILayout.Space();

        m_bundleName = EditorGUILayout.TextField("AssetBundleName ", m_bundleName);

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        {
            m_XMLFilePath = EditorGUILayout.TextField("   XML File : ", m_XMLFilePath);
            if (GUILayout.Button("select", GUILayout.Width(50)))
            {
                m_XMLFilePath = EditorUtility.OpenFilePanel("", "", "xml");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        {
            m_outputPath = EditorGUILayout.TextField("   Output folder : ", m_outputPath);
            if (GUILayout.Button("select", GUILayout.Width(50)))
            {
                m_outputPath = EditorUtility.SaveFolderPanel("Bundle saving folder", "", "");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(12);
            old_bsmoothDependencies = bsmoothDependencies; 
            bsmoothDependencies = GUILayout.Toggle(bsmoothDependencies, "   Smooth dependencies");

            if (old_bsmoothDependencies != bsmoothDependencies)
            {
                readXMLFile();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Read", GUILayout.Width(100)) && m_XMLFilePath != "")
        {
            readXMLFile();
            //Debug.Log(m_nameList.Count);
        }
        if (m_read)
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height / 2));
            {
                foreach (string s in m_nameList)
                {
                    GUILayout.Label(s);
                }
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Build", GUILayout.Width(100)))
            {
                if(bsmoothDependencies)
                    smoothDependencies();
                getPaths();
                buildAssetBundle();
            }
        }
    }

    void getPaths()
    {

        string[] paths = AssetDatabase.GetAllAssetPaths();
        foreach (string s in m_nameList)
        {
            for (int i = 0; i < paths.Length; ++i)
            {
                if (paths[i].Contains(s))
                {
                    m_pathList.Add(paths[i]);
                    continue;
                }
            }
        }
       
    }

    void smoothDependencies()
    {

        List<string> index = new List<string>();
        if (m_nameList.Count > 0)
        {
            foreach (string s in m_nameList)
            {
                for (int i = 0; i < m_nameList.Count; ++i)
                {
                    if (m_nameList[i].Contains(s) && s != m_nameList[i])
                    {
                        index.Add(m_nameList[i]);

                    }
                }
            }

            foreach (string x in index)
            {
                m_nameList.Remove(x);
            }
        }
    }

    void buildAssetBundle()
    {
        AssetBundleBuild[] tab = new AssetBundleBuild[1];
        tab[0].assetNames = m_pathList.ToArray();
        tab[0].assetBundleName = m_bundleName;
        tab[0].assetBundleVariant = "unity3d";

        BuildPipeline.BuildAssetBundles(m_outputPath, tab, BuildAssetBundleOptions.IgnoreTypeTreeChanges);
    }
}