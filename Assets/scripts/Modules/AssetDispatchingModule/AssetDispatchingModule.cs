using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using dassault;
using loadingPackageModule; 

namespace assetDispatchingModule
{

    
    public class AssetDispatchingModule : ModuleInstance
    {
        LoadingPackageModule loadingPackageModule;
        public override void OnAllModuleLoaded(ModuleRepository repository)
        {
            Activate(true);
            m_dispatcher = gameObject.AddComponent<ConcreteDispatcher>();
            loadingPackageModule = repository.Get("LoadingPackageModule") as LoadingPackageModule;
            loadingPackageModule.onAssetDeployment += deployAssets; 
            AnimationModule animationModule = repository.Get("AnimationModule") as AnimationModule;
            GlassApplicationModule glassApplicationModule = repository.Get("GlassApplicationModule") as GlassApplicationModule;
            PlaneViewModule planeViewModule = repository.Get("PlaneViewModule") as PlaneViewModule;

            m_dispatcher.init(animationModule, glassApplicationModule, planeViewModule); 
        }

        public override HashSet<string> GetModuleDependencies()
        {
            HashSet<string> result = new HashSet<string>();
            result.Add("PlaneViewModule");
            result.Add("AnimationModule");
            result.Add("LoadingPackageModule");
            result.Add("GlassApplicationModule");
            return result;
        }


        // Update is called once per frame
        void Update()
        {

        }

        public void deployAssets(List<UnityEngine.Object> iObjectList)
        {
            m_animationList = new Dictionary<string, UnityEngine.Object>();
            m_planeViewList = new Dictionary<string, UnityEngine.Object>();
            m_glassList = new Dictionary<string, UnityEngine.Object>(); 

            TextAsset t = null;
            m_ObjectList = iObjectList;

            if (m_ObjectList != null && m_ObjectList.Count != 0)
            {
                foreach (UnityEngine.Object o in m_ObjectList)
                {
                    if (o as TextAsset != null)
                        t = o as TextAsset;
                }

                if (t != null)
                {
                    readXMLFile(t);
                    m_dispatcher.startDispatching(m_glassList, m_animationList, m_planeViewList, t); 

                }
            }
        }

        void readXMLFile(TextAsset iDocument)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(iDocument.text);

                XmlNode root = document.FirstChild;
                if (root == null)
                    Debug.Log("Error while reading file");
                else
                    parseXML(root as XmlElement);
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
                    if (!m_planeViewList.ContainsKey(son.InnerText))
                    {
                        string[] stringSeparators = new string[] { ";" };
                        string[] tab;
                        tab = son.InnerText.Split(new char[] { ';' });

                        foreach (string s in tab)
                        {
                            UnityEngine.Object o = m_ObjectList.Find(obj => obj.name == s);
                            if (o != null)
                            {
                                m_planeViewList.Add(s, o); 
                            }
                        }
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
                                if (!m_glassList.ContainsKey(son.InnerText))
                                {
                                    string[] stringSeparators = new string[] { ";" };
                                    string[] tab;
                                    tab = son.InnerText.Split(new char[] { ';' });

                                    foreach (string s in tab)
                                    {
                                        UnityEngine.Object o = m_ObjectList.Find(obj => s.Contains(obj.name));
                                        if (o != null)
                                        {
                                            m_glassList.Add(s, o);
                                        }
                                    }
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
                                if (!m_animationList.ContainsKey(son.InnerText))
                                {
                                    UnityEngine.Object o = m_ObjectList.Find(obj => obj.name == son.InnerText);
                                    if (o != null)
                                    {
                                        m_animationList.Add(son.InnerText, o);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Dictionary<string, UnityEngine.Object> m_planeViewList;
        Dictionary<string, UnityEngine.Object> m_glassList;
        Dictionary<string, UnityEngine.Object> m_animationList; 
        List<UnityEngine.Object> m_ObjectList; 
        ConcreteDispatcher m_dispatcher; 
    }

}
