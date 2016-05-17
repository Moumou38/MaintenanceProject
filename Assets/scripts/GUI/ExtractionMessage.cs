using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExtractionMessage : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        m_text = GetComponent<Text>();
        InvokeRepeating("LoadingMessage", 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadingMessage()
    {
        switch (step)
        {
            case 0:
                m_text.text = m_message;
                break;
            case 1:
                m_text.text = m_message + ".";
                break;
            case 2:
                m_text.text = m_message + "..";
                break;
            case 3:
                m_text.text = m_message  + "...";
                break;
            default:
                m_text.text = m_message + "....";
                step = 0;
                break;
        }
        step++;

    }

    int step = 0;
    Text m_text;
    public string m_message = ""; 
}

