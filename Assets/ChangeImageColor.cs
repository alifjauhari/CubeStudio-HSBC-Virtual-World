using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour
{
    public Text text;
    public void ChangeColor2White()
    {
        gameObject.GetComponent<Image>().color = Color.white;
        text.color = Color.black;
    }

    public void ChangeColor2Red()
    {
        gameObject.GetComponent<Image>().color = Color.red;
        text.color = Color.white;
    }
}
