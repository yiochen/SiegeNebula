using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SlideManagerScript : MonoBehaviour {
    List<SlideScript> slides = new List<SlideScript>();
    private int currentSlide = 0;
    // Use this for initialization
	void Start () {
	    foreach (Transform child in transform)
        {
            slides.Add(child.GetComponent<SlideScript>());
        }
        clearScreen();
        ShowSlide(0);
	}
    void clearScreen()
    {
        foreach (SlideScript slide in slides)
        {
            slide.gameObject.SetActive(false);
        }
    }
    void ShowSlide(int slideNumber)
    {
        currentSlide = slideNumber;
        SlideScript slide = slides[slideNumber];
        if (slide)
        {
            if (slide.clearScreen)
            {
                clearScreen();
            }
            slide.gameObject.SetActive(true);
        }
    }
    public bool Next()
    {
        if (currentSlide < slides.Count - 1)
        {
            ShowSlide(currentSlide + 1);
            return true;
        } else
        {
            return false;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Next();
        }
    }
}
