using UnityEngine;
using System.Collections;

//public class ScriptSceneFinder : MonoBehaviour {

//	// Use this for initialization
//	void Start () {
	
//	}
	
//	// Update is called once per frame
//	void Update () {
	
//	}
//}
    using UnityEditor;
    using System.Collections.Generic;
    using System.Reflection;   
    using System;
 
 
     public class ScriptSceneFinder : EditorWindow
{
    static ScriptSceneFinder window;

    static string scriptValue = "";
    static string oldScriptValue;

    static List<Type> components;
    static List<string> componentNames;
    static List<GameObject> sceneObjects;
    static int selectedIndex = 0;
    static int prevIndex = 0;
    static Vector2 scrollValue = Vector2.zero;

    static string sceneObjectsText = "";

    //public 
    // Use this for initialization
    [MenuItem("EditorUtility/Script Finder")]
    static void OpenScriptFinder()
    {
        //EditorUtility.FocusProjectWindow();        
        window = (ScriptSceneFinder)EditorWindow.GetWindow(typeof(ScriptSceneFinder));

        sceneObjects = new List<GameObject>();
        foreach (UnityEngine.GameObject _obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            sceneObjects.Add(_obj);
            Debug.Log("scene object: " + _obj.ToString());
        }

        sceneObjectsText = "";
        foreach (GameObject _obj in sceneObjects)
        {  foreach(var component in _obj.GetComponents<MonoBehaviour>())
            {
                if (_obj.transform != _obj.transform.root)
                {
                    sceneObjectsText += _obj.transform.root.name + "/" + AnimationUtility.CalculateTransformPath(_obj.transform, _obj.transform.root)
                        + " (" + _obj.GetType().FullName + ")";
                }
                else
                {
                    sceneObjectsText += _obj.name
                        + " (" + _obj.GetType().FullName + ")";
                }
                sceneObjectsText += "\n\r";
                sceneObjectsText += "\t\t" + component.GetType().FullName + "\n\r";
            }
        }
    }


    void OnGUI()
    {
        GUILayoutOption[] layout= { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };

        scrollValue = EditorGUILayout.BeginScrollView(scrollValue, layout);
        EditorGUILayout.TextArea(sceneObjectsText,layout);
        EditorGUILayout.EndScrollView();
    }
}