using UnityEngine;
using System.Collections;
using dassault;
using System.Collections.Generic;


namespace assetDispatchingModule
{

    public class ConcreteDispatcher : MonoBehaviour
    {
        public void init(AnimationModule iAnimationModule, GlassApplicationModule iGlassApplicationModule, PlaneViewModule iPlaneViewModule)
        {
            m_animationModule = iAnimationModule;
            m_glassApplicationModule = iGlassApplicationModule;
            m_planeViewModule = iPlaneViewModule; // highlights in scene
        }
           
        public void startDispatching(Dictionary<string, Object> iGlassObject, Dictionary<string, Object> iAnimationObject, Dictionary<string, Object> iPlaneViewObject, TextAsset iScenario)
        {
            
            //Step 1 : clean the current modules from previous inspection
            cleanPreviousInspection(); 

            // step 2 : instantiate necessary objects 
            Transform highlightRoot = getHightlightRoot();
            foreach (string s in iPlaneViewObject.Keys)
            {
                GameObject tmp = Instantiate(iPlaneViewObject[s], highlightRoot.position, highlightRoot.rotation) as GameObject;
                tmp.transform.parent = highlightRoot;
                tmp.name = s;
                tmp.SetActive(false); 
            }

            foreach (string s in iAnimationObject.Keys)
            {
                GameObject EmptyGO = new GameObject();
                EmptyGO.transform.parent = m_animationModule.transform;
                EmptyGO.name = s;
                AnimationDescriptor d = EmptyGO.AddComponent<AnimationDescriptor>();

                GameObject animation = Instantiate(iAnimationObject[s], m_animationModule.transform.position, m_animationModule.transform.rotation) as GameObject;
                animation.transform.parent = EmptyGO.transform;
                animation.name = s;

                d.name = s;
                d.setAnimation(animation.GetComponent<Animation>());

                foreach (Transform child in animation.transform)
                {
                    if (child.name.Contains("camera"))
                    {
                        foreach (Transform Animation in child.transform)
                        {
                            if (Animation.name.Contains("camera") && !Animation.name.Contains("cube"))
                            {
                                Animation.transform.Rotate(new Vector3(0, -90, 0));
                                d.setCameraPosition(Animation);
                                break;
                            }
                        }
                    }
                }

                EmptyGO.gameObject.layer = LayerMask.NameToLayer("Animation");
                ChangeLayersRecursively(EmptyGO.transform, "Animation");
            }
            
            // step 3 : transmit the Object list to the right modules
            m_glassApplicationModule.setScenario(iScenario, iGlassObject); 
            m_animationModule.loadDescriptors(); 

        }

        void ChangeLayersRecursively(Transform t, string name)
        {
            foreach (Transform child in t)
            {
                 child.gameObject.layer = LayerMask.NameToLayer(name);
                 ChangeLayersRecursively(child, name);
            }
        }

        Transform getHightlightRoot()
        {
            Transform highlightRoot = null;
            foreach (Transform child in m_planeViewModule.transform)
            {
                if (child.name == "Plane")
                {
                    foreach (Transform PlaneChild in child.transform)
                    {
                        if (PlaneChild.name == "highlights")
                        {
                            highlightRoot = PlaneChild;
                            break;
                        }
                    }
                }
            }

            return highlightRoot;
        }

        void cleanPreviousInspection()
        {
            if (m_animationModule != null && m_glassApplicationModule != null && m_planeViewModule != null)
            {
                List<Transform> m_childrenToDelete = new List<Transform>();
                foreach (Transform child in m_animationModule.transform)
                {
                    if (child.GetComponent<AnimationDescriptor>() != null)
                    {
                        m_childrenToDelete.Add(child);
                    }
                }
                foreach (Transform HighlightChild in getHightlightRoot().transform)
                {
                    m_childrenToDelete.Add(HighlightChild);
                }



                for (int i = 0; i < m_childrenToDelete.Count; ++i)
                {
                    Destroy(m_childrenToDelete[i].gameObject);
                }
            }
        }


        AnimationModule m_animationModule;
        GlassApplicationModule m_glassApplicationModule;
        PlaneViewModule m_planeViewModule; 
    }
}