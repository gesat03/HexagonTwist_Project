using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum HexColor
{
    Red = 0,
    Blue = 1,
    Yellow = 2,
    Green = 3,
    Purple = 4,
    Colorless = 5
}

public enum HexGroupOrder
{
    Upper,
    Side,
    Lower
}

public class Hexagon 
{

    private float width;
    private float height;
    internal int placementNumber;
    public HexColor hexColor;
    internal GameObject hexObject;
    private Transform parentTransform;
    public bool isItBomb;


    //Dinamicly change the X-position of Hexagon
    public int PosX 
    {
        get 
        { 
            if(hexObject.gameObject != null)
            {
                return (int)hexObject.GetComponent<RectTransform>().localPosition.x;
            }
            else
            {
                return -1;
            }
        }

        set 
        {
            if (hexObject.gameObject != null)
            {
                hexObject.GetComponent<RectTransform>().localPosition =
                   new Vector2(value , hexObject.GetComponent<RectTransform>().localPosition.y);
            }
        } 
    }

    //Dinamicly change the Y-position of Hexagon
    public int PosY
    {
        get
        {
            if (hexObject.gameObject != null)
            {
                return (int)hexObject.GetComponent<RectTransform>().localPosition.y;
            }
            else
            {
                return -1;
            }
        }

        set
        {
            if (hexObject.gameObject != null)
            {
                hexObject.GetComponent<RectTransform>().localPosition =
                    new Vector2(hexObject.GetComponent<RectTransform>().localPosition.x, value);
            }
        }
    }

    //Dinamicly change the Color Enum of Hexagon
    public HexColor HexColor
    {
        get
        {
            return hexColor;
        }
        set
        {
            hexColor = value;

            if (isItBomb)
            {
                switch (hexColor)
                {
                    case HexColor.Red:
                        hexObject.GetComponent<HexagonElements>().ChangeBombColor(0);
                        break;
                    case HexColor.Green:
                        hexObject.GetComponent<HexagonElements>().ChangeBombColor(1);
                        break;
                    case HexColor.Blue:
                        hexObject.GetComponent<HexagonElements>().ChangeBombColor(2);
                        break;
                    case HexColor.Yellow:
                        hexObject.GetComponent<HexagonElements>().ChangeBombColor(3);
                        break;
                    case HexColor.Purple:
                        hexObject.GetComponent<HexagonElements>().ChangeBombColor(4);
                        break;
                    case HexColor.Colorless:
                        hexObject.GetComponent<HexagonElements>().ChangeBombColor(5);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (hexColor)
                {
                    case HexColor.Red:
                        hexObject.GetComponent<HexagonElements>().ChangeHexagonColor(0);
                        break;
                    case HexColor.Green:
                        hexObject.GetComponent<HexagonElements>().ChangeHexagonColor(1);
                        break;
                    case HexColor.Blue:
                        hexObject.GetComponent<HexagonElements>().ChangeHexagonColor(2);
                        break;
                    case HexColor.Yellow:
                        hexObject.GetComponent<HexagonElements>().ChangeHexagonColor(3);
                        break;
                    case HexColor.Purple:
                        hexObject.GetComponent<HexagonElements>().ChangeHexagonColor(4);
                        break;
                    case HexColor.Colorless:
                        hexObject.GetComponent<HexagonElements>().ChangeHexagonColor(5);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //Debug Text From HexagonElements
    public void DebugTexting(string text)
    {
        hexObject.GetComponent<HexagonElements>().DebugText(text);
    }

    //Constractor Function
    public Hexagon( 
                    float width, 
                    float height, 
                    int placementNumber, 
                    int posX, 
                    int posY,
                    HexColor hexColor, 
                    GameObject hexObject, 
                    Transform parentTransform,
                    bool isItBomb
                  )
    {
        this.hexObject = GameObject.Instantiate(hexObject, parentTransform);

        this.width = width;
        this.height = height;
        this.placementNumber = placementNumber;
        this.hexColor = hexColor;
        this.isItBomb = isItBomb;

        //Dinamicly Changable Functions
        PosX = posX;
        PosY = posY;
        

        //if (!isItBomb) HexColor = hexColor;
        //else BombColor = hexColor;
    }

    public Tween Animation(int moveDirection, float time)
    {
        return this.hexObject.GetComponent<RectTransform>().DOLocalMoveY(moveDirection, time).SetEase(Ease.OutCubic);
    }

}
