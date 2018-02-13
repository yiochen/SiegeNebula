using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class PlanetStatScript : MonoBehaviour
{

    public PlanetScriptable planetType;
    public TextAsset stats;
    // Use this for initialization

    void ParseXML(string xmlString)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);
        Debug.Log(xmlDoc.FirstChild.Name);
    }

    void Start()
    {
        ParseXML(stats.ToString());
    }
}
