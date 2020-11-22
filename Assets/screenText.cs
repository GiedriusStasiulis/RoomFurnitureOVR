using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class screenText : MonoBehaviour
{
    public Text mytext = null;
    private int counter = 0;

    public void nextStep()
    {
        counter++;
        mytext.text = "Test " + counter;
    }

    public void prevStep()
    {
        counter--;
        mytext.text = "Test " + counter;
    }

}
