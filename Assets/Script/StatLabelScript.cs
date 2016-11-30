using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StatLabelScript : MonoBehaviour {
    Text text;
    public string title = "title";
    public string value = "0";
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        SetValue(value);
	}

	public void SetValue(string value)
    {
        this.value = value;
        if (text)
        {

            text.text = title + ": " + this.value;
        }
    }
}
