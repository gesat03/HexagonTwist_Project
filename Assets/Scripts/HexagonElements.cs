using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexagonElements : MonoBehaviour
{
    public Sprite[] HexagonBaseColorElements;
    public Sprite[] BombBaseColorElements;

    public Text debugText;
    public Text bombText;

    public void ChangeHexagonColor(int i)
    {
        this.gameObject.GetComponent<Image>().sprite = HexagonBaseColorElements[i];
    }

    public void ChangeBombColor(int i)
    {
        this.gameObject.GetComponent<Image>().sprite = BombBaseColorElements[i];
    }

    public void DebugText(string text)
    {
        this.debugText.text = text;
    }

}
