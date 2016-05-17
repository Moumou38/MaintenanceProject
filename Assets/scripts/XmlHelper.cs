using UnityEngine;
using System.Collections;
using System.Xml;

namespace dassault
{
    public class XmlHelper
    {
        public static void AddParameter(XmlDocument document, XmlElement root, string parameterName, int value)
        {
            XmlElement newElement = document.CreateElement(parameterName);
            newElement.SetAttribute("value", value.ToString());
            root.AppendChild(newElement);
        }
        public static void AddParameter(XmlDocument document, XmlElement root, string parameterName, float value)
        {
            XmlElement newElement = document.CreateElement(parameterName);
            newElement.SetAttribute("value", value.ToString());
            root.AppendChild(newElement);
        }
        public static void AddParameter(XmlDocument document, XmlElement root, string parameterName, string value)
        {
            XmlElement newElement = document.CreateElement(parameterName);
            newElement.SetAttribute("value", value);
            root.AppendChild(newElement);
        }
        public static void AddParameter(XmlDocument document, XmlElement root, string parameterName, Vector3 value)
        {
            XmlElement newElement = document.CreateElement(parameterName);
            newElement.SetAttribute("x", value.x.ToString());
            newElement.SetAttribute("y", value.y.ToString());
            newElement.SetAttribute("z", value.z.ToString());
            root.AppendChild(newElement);
        }
        public static void AddParameter(XmlDocument document, XmlElement root, string parameterName, Quaternion value)
        {
            XmlElement newElement = document.CreateElement(parameterName);
            newElement.SetAttribute("x", value.x.ToString());
            newElement.SetAttribute("y", value.y.ToString());
            newElement.SetAttribute("z", value.z.ToString());
            newElement.SetAttribute("w", value.w.ToString());
            root.AppendChild(newElement);
        }

        public static int ReadParameterInt(XmlElement root, string parameterName)
        {
            XmlNodeList nodes = root.GetElementsByTagName(parameterName);
            int result = 0;
            if(nodes.Count != 0)
            {
                XmlElement parameter = nodes[0] as XmlElement;
                if(parameter.HasAttribute("value"))
                {
                    string value = parameter.GetAttribute("value");
                    if(!int.TryParse(value, out result))
                    {
                        Debug.LogWarning("unable to read value from parameter " + parameterName);
                    }
                }
            }
            return result;
        }
        
        public static float ReadParameterFloat(XmlElement root, string parameterName)
        {
            XmlNodeList nodes = root.GetElementsByTagName(parameterName);
            float result = 0;
            if(nodes.Count != 0)
            {
                XmlElement parameter = nodes[0] as XmlElement;
                if(parameter.HasAttribute("value"))
                {
                    string value = parameter.GetAttribute("value");
                    if(!float.TryParse(value, out result))
                    {
                        Debug.LogWarning("unable to read value from parameter " + parameterName);
                    }
                }
            }
            return result;
        }
        
        public static string ReadParameterString(XmlElement root, string parameterName)
        {
            XmlNodeList nodes = root.GetElementsByTagName(parameterName);
            string result = string.Empty;
            if(nodes.Count != 0)
            {
                XmlElement parameter = nodes[0] as XmlElement;
                if(parameter.HasAttribute("value"))
                {
                    result = parameter.GetAttribute("value");
                }
            }
            return result;
        }

        public static Vector3 ReadParameterVector3(XmlElement root, string parameterName)
        {
            XmlNodeList nodes = root.GetElementsByTagName(parameterName);
            Vector3 result = Vector3.zero;
            if(nodes.Count != 0)
            {
                XmlElement parameter = nodes[0] as XmlElement;
                if(parameter.HasAttribute("x") && parameter.HasAttribute("y") && parameter.HasAttribute("z"))
                {
                    string valueX = parameter.GetAttribute("x");
                    string valueY = parameter.GetAttribute("y");
                    string valueZ = parameter.GetAttribute("z");
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    if(!float.TryParse(valueX, out x) || !float.TryParse(valueY, out y) || !float.TryParse(valueZ, out z))
                    {
                        Debug.LogWarning("unable to read value from parameter " + parameterName);
                    }
                    result.Set(x, y, z);
                }
            }
            return result;
        }

        public static Quaternion ReadParameterQuaternion(XmlElement root, string parameterName)
        {
            XmlNodeList nodes = root.GetElementsByTagName(parameterName);
            Quaternion result = Quaternion.identity;
            if(nodes.Count != 0)
            {
                XmlElement parameter = nodes[0] as XmlElement;
                if(parameter.HasAttribute("x") && parameter.HasAttribute("y") && parameter.HasAttribute("z") && parameter.HasAttribute("w"))
                {
                    string valueX = parameter.GetAttribute("x");
                    string valueY = parameter.GetAttribute("y");
                    string valueZ = parameter.GetAttribute("z");
                    string valueW = parameter.GetAttribute("w");
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    float w = 0;
                    if(!float.TryParse(valueX, out x) || !float.TryParse(valueY, out y) || !float.TryParse(valueZ, out z) || !float.TryParse(valueW, out w))
                    {
                        Debug.LogWarning("unable to read value from parameter " + parameterName);
                    }
                    result.Set(x, y, z, w);
                }
            }
            return result;
        }

    }
}

