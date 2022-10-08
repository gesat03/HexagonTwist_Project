using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GridSystem : MonoBehaviour
{

    public GameObject HexPrefab;
    public GameObject BombPrefab;
    public GameObject TriplePointSelectableObject;
    public GameObject TouchScreenLockingObj;

    public ScoreSystem scoreSystem;
    
    //Instantiating Object
    public GameObject capsulatingObj;

    [Header("Grid Map Referance Container")]
    [Space]
    public GameObject HexagonParent;
    public GameObject TouchPointParent;

    [Header("Grid Map Dimensions")]
    [Space]
    // Grid Map Dimensions
    // !!!This properties are related to Canvas dimensions!!!
    private static readonly int gridWidth = 1080;
    private static readonly int gridHeight = 1920;

    [Header("Grid Properties")]
    [Space]
    public int hexWidthSize;
    public int hexHeightSize;

    private int initialPos;
    private int currentXPos;
    private int currentYPos;

    private int totalHexsNumber;
    private int totalTriplePointNumber;

    //Single Hex Variables
    private static readonly float hexWidth = 160;
    private static readonly float hexHeight = 140;

    internal Hexagon[] hexContainer;

    private int[,] triplePointContainer;

    private GameObject[] triplePointPrefab;

    private Vector2[] encapsulatedPointPositions;

    private Vector3 _firstInputPosition;
    private Vector3 _currentInputPosition;

    private Sequence turningSequence;// = DOTween.Sequence();
    List<Sequence> animationSequenceList;

    public bool colorMatch;
    public bool waitColorCrash;
    private bool waitSlindingHex; 

    // Encapsulated same color hex groups
    List<int> colorlesTriplePointHolder;

    //Debuging
    public GameObject debugPointTextObj;
    private GameObject debugObj;
    public int encapsulatedTriplePoint;
    int[] debugEncapsulatedPrevious = new int[3];

    private Vector3 CalculateCurrentInputPosition()
    {
        //calculate the mouse Y position for canvas position which is the giving height
        return _currentInputPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y - (gridHeight - 500f), 0);   
    }


    #region For testing...
    //private void Start()
    //{
    //    totalHexsNumber = hexWidthSize * hexHeightSize;
    //    totalTriplePointNumber = ((hexWidthSize - 1) * (hexHeightSize - 1)) * 2;

    //    heightArranger = HexagonParent.GetComponent<RectTransform>().position.y;

    //    triplePointPrefab = new GameObject[totalTriplePointNumber];
    //    hexContainer = new Hexagon[totalHexsNumber];
    //    triplePointContainer = new int[totalTriplePointNumber,3];
    //    encapsulatedPointPositions = new Vector2[totalTriplePointNumber];

    //    mySequence = DOTween.Sequence();

    //    Initial_Starting_Point();
    //    currentXPos = initialPos;
    //    currentYPos = 0;

    //    HexGenerator();
    //    EncapsulatedAreaDesigne();
    //    ColorGroupArranger();
    //    EncapsulatedPointPositions();
    //    TriplePointSelectionPosition();

    //    DebugPointText();

    //    Debug.Log(triplePointContainer.Length);
    //}

    //void Update()
    //{

    //    if (Input.GetKeyUp("d")) TurningDynamics(true);

    //    if (Input.GetKeyUp("s")) SlidingColors();

    //    if (Input.GetKeyUp("e")) EncapsulateColors();

    //    if (Input.GetKeyUp("a")) TurningDynamics(false);

    //    TouchMechanism();

    //}
    #endregion


    public void InitializingFunction()
    {
        int OddCheck(int number)
    {
        float output;

        output = ((float)number / 2.0f) - (number / 2);

        if (output > 0)
        {
            return (number) + 1;
        }
        else
        {
            return (number);
        }
    }

        int Initial_Starting_Point()
    {
        //Hex in Parent Starting Pos Formula
        //In One Row Hex Number = x
        //Hex Edge Length = a
        //Distance Between Width Bounderies = y
        //Starting pos = GridWidth - [ (x/2 * (3*a) + a/2) + y/2 ] + a

        initialPos = (int)((gridWidth - ((hexWidthSize / 2.0f * (3 * (hexWidth / 2.0f))) + (hexWidth / 4.0f))) / 2 + (hexWidth / 2.0f));
        return initialPos;
    }

        totalHexsNumber = hexWidthSize * hexHeightSize;
        totalTriplePointNumber = ((hexWidthSize - 1) * (hexHeightSize - 1)) * 2;

        triplePointPrefab = new GameObject[totalTriplePointNumber];
        hexContainer = new Hexagon[totalHexsNumber];
        triplePointContainer = new int[totalTriplePointNumber, 3];
        encapsulatedPointPositions = new Vector2[totalTriplePointNumber];

        waitColorCrash = true;
        waitSlindingHex = false;

        //Dotween Sequences
        turningSequence = DOTween.Sequence();
        animationSequenceList = new List<Sequence>();

        Initial_Starting_Point();
        currentXPos = initialPos;
        currentYPos = 0;

        HexGenerator();
        EncapsulatedAreaDesigne();
        ColorGroupArranger();
        EncapsulatedPointPositions();
        TriplePointSelectionPosition();

        #region Setup Fucntions

        // Arranging colors which is not conflict with each others 
        void ColorGroupArranger()
        {
            for (int i = 0; i < totalTriplePointNumber; i++)
            {
                if (hexContainer[triplePointContainer[i, 0]].hexColor == hexContainer[triplePointContainer[i, 1]].hexColor &&
                    hexContainer[triplePointContainer[i, 0]].hexColor == hexContainer[triplePointContainer[i, 2]].hexColor)
                {
                    int x = (int)hexContainer[triplePointContainer[i, 1]].hexColor;
                    int randomNumber;
                    while (true)
                    {
                        randomNumber = Random.Range(0, 5);
                        if (randomNumber != x)
                        {
                            hexContainer[triplePointContainer[i, 2]].HexColor = (HexColor)randomNumber;
                            #region Debug...
                            //Debug.Log("Point is: " + i);
                            //Debug.Log("Hex Number is:" + triplePointContainer[i, 2]);
                            //Debug.Log("Changed Color: " + (HexColor)x + " to " + (HexColor)randomNumber);
                            #endregion
                            break;
                        }
                    }

                }
            }
        }

        // Instantiating Touch Points For Hex Groups
        void TriplePointSelectionPosition()
        {
            for (int i = 0; i < totalTriplePointNumber; i++)
            {

                int number = i;

                triplePointPrefab[i] = Instantiate(TriplePointSelectableObject, TouchPointParent.transform);

                triplePointPrefab[i].name = "TriplePoint - " + i;

                triplePointPrefab[i].GetComponent<RectTransform>().localPosition = encapsulatedPointPositions[i];

                triplePointPrefab[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectEncapsulatedArea(number));

            }
        }

        // Positioning For Hex Groups
        void EncapsulatedPointPositions()
        {
            int shifterNumber = hexWidthSize - 1;
            int counter = 0;
            int remainder = 0;

            for (int i = 0; i < totalTriplePointNumber; i++)
            {

                if (counter % 2.0f == remainder)
                {
                    //Even Hex X-Position Formula 0 - 2 - 4 - ...
                    encapsulatedPointPositions[i].x = hexContainer[triplePointContainer[i, 0]].
                        hexObject.GetComponent<RectTransform>().localPosition.x + (hexWidth / 4);
                    //Even Hex Y-Position Formula 0 - 2 - 4 - ...
                    encapsulatedPointPositions[i].y = hexContainer[triplePointContainer[i, 0]].
                        hexObject.GetComponent<RectTransform>().localPosition.y - (hexHeight / 2);
                }
                else
                {
                    //Odd Hex X-Position Formula 1 - 3 - 5 - ...
                    encapsulatedPointPositions[i].x = hexContainer[triplePointContainer[i, 0]].
                        hexObject.GetComponent<RectTransform>().localPosition.x - (hexWidth / 4);
                    //Odd Hex Y-Position Formula 1 - 3 - 5 - ...
                    encapsulatedPointPositions[i].y = hexContainer[triplePointContainer[i, 0]].
                        hexObject.GetComponent<RectTransform>().localPosition.y - (hexHeight / 2);
                }

                //Debug.Log(counter % 2.0f + " " + remainder);

                //Shifting Row Section
                if (counter == shifterNumber - 1)
                {
                    counter = 0;

                    if (remainder == 0)
                    {
                        remainder = 1;
                    }
                    else
                    {
                        remainder = 0;
                    }
                }
                else
                {
                    counter++;
                }

                //Debug.Log(encapsulatedPointPositions[i]);
            }
        }

        // Placing hexagons inside the hex groups
        void EncapsulatedAreaDesigne()
        {
            int upperHex = 0;
            int lowerHex = hexWidthSize;
            int sideHex = OddCheck(hexWidthSize) / 2;
            int lineCounter = 1;
            int remainder = 0;

            for (int i = 0; i < totalTriplePointNumber; i++)
            {
                triplePointContainer[i, 0] = upperHex;
                triplePointContainer[i, 1] = sideHex;
                triplePointContainer[i, 2] = lowerHex;

                if (lineCounter < hexWidthSize - 1)
                {
                    if (i % 2 == remainder)
                    {
                        upperHex++;
                        lowerHex++;
                    }
                    else
                    {
                        sideHex++;
                    }
                    lineCounter++;
                }
                else
                {
                    lineCounter = 1;
                    upperHex++;
                    lowerHex++;
                    sideHex++;
                    if (OddCheck(hexWidthSize) - hexWidthSize > 0)
                    {
                        if (remainder == 0)
                        {
                            remainder = 1;
                        }
                        else
                        {
                            remainder = 0;
                        }
                        //Debug.Log(OddCheck(hexWidthSize) - hexWidthSize);
                    }
                }
                //Debug.Log(triplePointContainer[i, 0] + "," + triplePointContainer[i, 1] + "," + triplePointContainer[i, 2]);
            }
        }

        // Generating Hexes
        void HexGenerator()
        {
            int counter = 0;
            int upperLine = 1;
            //Hex Colomus
            for (int i = 0; i < hexHeightSize; i++)
            {
                //Hex Rows
                for (int t = 0; t < hexWidthSize; t++)
                {
                    //Instantiating Hexagon with variables 
                    hexContainer[counter] = new Hexagon(hexWidth, hexHeight, counter, currentXPos, currentYPos,
                                                            HexColor.Blue, HexPrefab, HexagonParent.transform, false);

                    //Random Color Assign
                    hexContainer[counter].HexColor = (HexColor)Random.Range(0, 5);
                    //Debug Text
                    //hexContainer[counter].DebugTexting("" + counter);

                    //Upper Line Function
                    if (upperLine < OddCheck(hexWidthSize) / 2)
                    {
                        currentXPos += (int)(3 * (hexWidth / 2.0f));
                    }
                    //Lower Line Function
                    else if (upperLine < hexWidthSize)
                    {
                        //Entering only once to give space for first below row
                        if (upperLine == OddCheck(hexWidthSize) / 2)
                        {
                            currentXPos = (int)(initialPos + (3 * (hexWidth / 4.0f)));
                            currentYPos -= (int)hexHeight / 2;
                        }
                        //Continue rest of lower row
                        else
                        {
                            currentXPos += (int)(3 * (hexWidth / 2.0f));
                        }
                    }
                    //Preparation for next Coloum
                    else
                    {
                        currentXPos = initialPos;
                        upperLine = 0;
                    }

                    counter++;
                    upperLine++;
                }

                currentYPos -= (int)hexHeight / 2;
            }
        }

        #endregion
    }

    // Touch mechanism
    public void TouchMechanism()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CalculateCurrentInputPosition();

            _firstInputPosition = _currentInputPosition;

        }

        else if (Input.GetMouseButtonUp(0))
        {
            CalculateCurrentInputPosition();

            if (Vector2.Distance(_firstInputPosition, _currentInputPosition) > 5)
            {
                if (!turningSequence.active && !waitSlindingHex)
                {
                    TurningDynamics(GetRotationDirection(encapsulatedTriplePoint));
                }
            }
        }
    }
    #region Turning Animation With Dotween
    /// <summary> (Taken from https://github.com/dogukansoysal/Hexfall-Demo and integrated to my system)
    /// Calculate the rotation direction with angles.
    /// 
    /// RULE:
    ///     If last input angle > first input angle then the rotation is anti-clockwise
    /// EXCEPTION:
    ///     Due to 360 degree coordinate system, angle between 0 to 355 is actually 1 degree, not 355.
    ///     So the function assumes that, if SIGNED angle between inputs greater then 180, it will be clockwise rotation.
    /// </summary>
    /// <returns> Rotation Direction in integer.</returns>
    private bool GetRotationDirection(int tripplePos)
    {
        var selectedCorner = new Vector3(encapsulatedPointPositions[tripplePos].x, encapsulatedPointPositions[tripplePos].y, 0);
        var heading = (Vector3)_firstInputPosition - selectedCorner;
        var direction = heading / heading.magnitude;

        var firstTouchAngle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
        firstTouchAngle += firstTouchAngle < 0 ? 360 : 0;

        #region ----Debug----
        //Debug.Log("selectedCorner.position: " + selectedCorner + "\n"
        //    + "_firstInputPosition: " + (Vector3)_firstInputPosition + "\t"
        //    + "selectedCorner.right: " + Vector3.right + "\t"
        //    + "heading = (Vector3)_firstInputPosition - selectedCorner.position: " + ((Vector3)_firstInputPosition - selectedCorner) + "\t"
        //    + "direction = heading / heading.magnitude: " + (heading / heading.magnitude) + "\t"
        //    + "firstTouchAngle: " + Vector3.SignedAngle(Vector3.right, direction, Vector3.back) + "\n");
        #endregion

        heading = (Vector3)_currentInputPosition - selectedCorner;
        direction = heading / heading.magnitude;

        #region ----Debug----
        //Debug.Log("_currentInputPosition: " + (Vector3)_currentInputPosition + "\t"
        //            + "heading = (Vector3)_currentInputPosition - selectedCorner.position: " + ((Vector3)_currentInputPosition - selectedCorner) + "\t"
        //            + "direction = heading / heading.magnitude: " + (heading / heading.magnitude) + "\t"
        //            + "currentTouchAngle: " + Vector3.SignedAngle(Vector3.right, direction, Vector3.back) + "\t");
        #endregion

        var currentTouchAngle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
        currentTouchAngle += currentTouchAngle < 0 ? 360 : 0;

        // Touch started from top right and finished bottom right
        if (currentTouchAngle - firstTouchAngle > 180)
        {
            return true;
        }

        // Touch started from bottom right and finished top right
        if (currentTouchAngle - firstTouchAngle < -180)
        {
            return false;
        }

        if (currentTouchAngle > firstTouchAngle)
        {
            return false;
        }

        return true;
    }

    private void TurningDynamics(bool clockwise)
    {
        Vector3 clockWiseRotation;
        float rotationDegree;

        turningSequence = DOTween.Sequence();

        if (clockwise)
        {
            rotationDegree = -120;
            clockWiseRotation = new Vector3(0, 0, rotationDegree);
        }
        else
        {
            rotationDegree = +120;
            clockWiseRotation = new Vector3(0, 0, rotationDegree);
        }

        #region -------------------(1st Try)Official Approach------------------
        for (int i = 0; i < 3; i++)
        {
            if (!colorMatch)
            {
                turningSequence.Append(TurningRotationTween(capsulatingObj, clockWiseRotation));

                clockWiseRotation += new Vector3(0, 0, rotationDegree);

                AfterTurningTriplePointHexPlacement();
            }
            else
            {
                break;
            }
        }
        #endregion

        #region -------------------(2st Try)Different Approach------------------
        //StartCoroutine(OneTurnAfter(mySequence, capsulatingObj, clockWiseRotation, rotationDegree));
        #endregion


        void AfterTurningTriplePointHexPlacement()
        {
            #region -------------------(3th Try)Official Approach------------------

            int[] displacementNumbers = new int[triplePointContainer.GetLength(1)];
            HexagonContainer[,] hexagonContainers = new HexagonContainer[3, 6];

            int[] mainContainer = new int[3];

            //Taking the hexagon #s inside the Encapsulated point
            for (int i = 0; i < triplePointContainer.GetLength(1); i++)
            {
                displacementNumbers[i] = triplePointContainer[encapsulatedTriplePoint, i];
            }

            //Finding the triple points which has the encapsulated hexagons and add them inside HexagonContainer class
            for (int h = 0; h < 3; h++)
            {
                int counter = 0;

                for (int i = 0; i < triplePointContainer.GetLength(0); i++)
                {
                    for (int t = 0; t < triplePointContainer.GetLength(1); t++)
                    {
                        if (triplePointContainer[i, t] == displacementNumbers[h])
                        {
                            hexagonContainers[h, counter] = new HexagonContainer(i, t, displacementNumbers[h]);
                            #region Debug...
                            //Debug.Log("Hex. Con. Group: " + h + "\t"
                            //        + "Element: " + counter + "\t"
                            //        + "TriplePoint: " + hexagonContainers[h, counter].triplePointNumber + "\t"
                            //        + "InGroup: " + hexagonContainers[h, counter].pointGroup + "\t"
                            //        + "Hex #: " + hexagonContainers[h, counter].hexNumber);
                            #endregion
                            counter++;
                        }
                    }
                }
            }

            #region Clockwise After Turning Placement

            if (clockwise)
            {
                int extractor;
                if (hexWidthSize % 2 == 0)
                {
                    if (encapsulatedTriplePoint % 2 == 0)
                    {
                        extractor = 1;
                    }
                    else
                    {
                        extractor = 2;
                    }
                }
                else
                {
                    if (encapsulatedTriplePoint % 2 == 0)
                    {
                        if ((encapsulatedTriplePoint / (hexWidthSize - 1)) % 2 == 0)
                        {
                            extractor = 1;
                        }
                        else
                        {
                            extractor = 2;
                        }
                    }
                    else
                    {
                        if ((encapsulatedTriplePoint / (hexWidthSize - 1)) % 2 == 1)
                        {
                            extractor = 2;
                        }
                        else
                        {
                            extractor = 1;
                        }
                    }
                }

                for (int i = 0; i < hexagonContainers.GetLength(0); i++)
                {
                    for (int t = 0; t < hexagonContainers.GetLength(1); t++)
                    {
                        if (hexagonContainers[extractor, t] != null)
                        {
                            triplePointContainer[hexagonContainers[extractor, t].triplePointNumber,
                                hexagonContainers[extractor, t].pointGroup] = displacementNumbers[i];
                        }
                    }
                    extractor++;
                    if (extractor > 2) extractor = 0;
                }
            }

            #endregion

            #region Counter Clockwise After Turning Placement

            if (!clockwise)
            {
                int extractor;
                if (hexWidthSize % 2 == 0)
                {
                    if (encapsulatedTriplePoint % 2 == 0)
                    {
                        extractor = 2;
                    }
                    else
                    {
                        extractor = 1;
                    }
                }
                else
                {
                    if (encapsulatedTriplePoint % 2 == 0)
                    {
                        if ((encapsulatedTriplePoint / (hexWidthSize - 1)) % 2 == 0)
                        {
                            extractor = 2;
                        }
                        else
                        {
                            extractor = 1;
                        }
                    }
                    else
                    {
                        if ((encapsulatedTriplePoint / (hexWidthSize - 1)) % 2 == 1)
                        {
                            extractor = 1;
                        }
                        else
                        {
                            extractor = 2;
                        }
                    }
                }

                for (int i = 0; i < hexagonContainers.GetLength(0); i++)
                {
                    for (int t = 0; t < hexagonContainers.GetLength(1); t++)
                    {
                        if (hexagonContainers[extractor, t] != null)
                        {
                            triplePointContainer[hexagonContainers[extractor, t].triplePointNumber,
                                hexagonContainers[extractor, t].pointGroup] = displacementNumbers[i];
                        }
                    }
                    extractor++;
                    if (extractor > 2) extractor = 0;
                }
            }

            #endregion

            #endregion

            #region -------------------(1st Try)Different Approach------------------
            //#region Clocwise Turning and Changing Hexis Inside the triplePointContainer
            ////Clockwise Even Turn
            //if (debugEncapsulation % 2 == 0)
            //{
            //    int counterEven = 0;
            //    int hexnum = 17;
            //    for (int i = 0; i < 3; i++)
            //    {
            //        for (int t = 0; t < hexagonContainers[counterEven].turnNumber; t++)
            //        {
            //            triplePointContainer[hexagonContainers[counterEven].triplePointNumber,
            //                hexagonContainers[counterEven].pointGroup] = hexagonContainers[hexnum].hexNumber;
            //            #region Debug...
            //            //Debug.Log("TriplePoint: " + hexagonContainers[counterEven].triplePointNumber + "\t"
            //            //            + "InGroup: " + hexagonContainers[counterEven].pointGroup + "\t"
            //            //            + hexagonContainers[hexnum].hexNumber);
            //            #endregion
            //            counterEven++;
            //        }
            //        if (hexnum == 17) hexnum = 5;
            //        else if (hexnum == 5) hexnum = 11;
            //        else hexnum = 17;
            //    }
            //}
            ////Clockwise Odd Turn
            //else
            //{
            //    int counterEven = 0;
            //    int hexnum = 11;
            //    for (int i = 0; i < 3; i++)
            //    {
            //        for (int t = 0; t < hexagonContainers[counterEven].turnNumber; t++)
            //        {
            //            triplePointContainer[hexagonContainers[counterEven].triplePointNumber, 
            //                hexagonContainers[counterEven].pointGroup] = hexagonContainers[hexnum].hexNumber;
            //            #region Debug...
            //            //Debug.Log("TriplePoint: " + hexagonContainers[counterEven].triplePointNumber + "\t"
            //            //            + "InGroup: " + hexagonContainers[counterEven].pointGroup + "\t"
            //            //            + hexagonContainers[hexnum].hexNumber);
            //            #endregion
            //            counterEven++;
            //        }
            //        if (hexnum == 11) hexnum = 17;
            //        else if (hexnum == 17) hexnum = 5;
            //        else hexnum = 11;
            //    }
            //}
            //#endregion

            //#region Debug...
            ////for (int i = 0; i < 3; i++)
            ////{
            ////    Debug.Log("TriplePoint # is: " + (i) + "\t" +
            ////                        "Hex # is: " + triplePointContainer[debugEncapsulation, i] + "\t" +
            ////                        "Color is: " + hexContainer[triplePointContainer[debugEncapsulation, i]].HexColor);
            ////}
            //#endregion
            #endregion

            #region-------------------(2nd Try)Different Approach------------------
            //List<int> counter = new List<int>();

            //for (int i = 0; i < triplePointContainer.GetLength(0); i++)
            //{
            //    for (int t = 0; t < triplePointContainer.GetLength(1); t++)
            //    {
            //        if(triplePointContainer[i, t] == triplePointContainer[debugEncapsulation, 0])
            //        {
            //            counter.Add(i);
            //            Debug.Log(i);
            //        }

            //    }
            //}

            //for (int i = 0; i < counter.Count; i++)
            //{
            //    for (int t = 0; t < triplePointContainer.GetLength(1); t++)
            //    {
            //        if(hexContainer[triplePointContainer[counter[i], t]].HexColor == 
            //            hexContainer[triplePointContainer[debugEncapsulation, 0]].HexColor)
            //        {
            //            Debug.Log(  "TriplePoint # is: " + counter[i] + "\t" +
            //                        "Hex # is: " + triplePointContainer[counter[i], t] + "\t" +
            //                        "Color is: " + hexContainer[triplePointContainer[counter[i], t]].HexColor);
            //        }
            //    }
            //}
            #endregion


            EncapsulateColors();

        }

    }

    Tween TurningRotationTween(GameObject obj, Vector3 direction)
    {
        obj.transform.DOKill();
        return obj.transform.DOLocalRotate(direction, 0.3f);
    }

    #endregion


    // If is there any same color group this function detectes
    void EncapsulateColors()
    {
        colorlesTriplePointHolder = new List<int>();

        List<int> sameColorHexs = new List<int>();

        colorMatch = false;

        AlternativeLockingScreen(true);

        for (int i = 0; i < triplePointContainer.GetLength(0); i++)
        {
            if (hexContainer[triplePointContainer[i, 1]].hexColor ==
                hexContainer[triplePointContainer[i, 0]].hexColor &&
                hexContainer[triplePointContainer[i, 2]].hexColor ==
                hexContainer[triplePointContainer[i, 0]].hexColor)
            {
                sameColorHexs.Add(i);
            }
        }

        if (sameColorHexs.Count > 0)
        {
            colorMatch = true;

            SpecifingColorlessHexes(sameColorHexs);

            //SlidingColors();
        }

        else
        {
            colorMatch = false;

            SetupInitialTriplePointRotation();

            if (turningSequence.active)
            {
                turningSequence.OnComplete(() => AlternativeLockingScreen(false));
            }
            else
            {
                AlternativeLockingScreen(false);
            }

        }

        // Reset hex group rotation and select the current group
        void SetupInitialTriplePointRotation()
        {
            if (!colorMatch)
            {
                SelectEncapsulatedArea(encapsulatedTriplePoint);
                capsulatingObj.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
            }
        }

        // Locking colorless hex's function
        void SpecifingColorlessHexes(List<int> sameColorHexes)
        {
            // Making matched colors to colorless
            foreach (var item in sameColorHexes)
            {
                for (int i = 0; i < 3; i++)
                {
                    hexContainer[triplePointContainer[item, i]].HexColor = (HexColor)5;
                }
            }
            // Taking the colorles hex groups in an array
            for (int i = 0; i < triplePointContainer.GetLength(0); i++)
            {
                if (hexContainer[triplePointContainer[i, 1]].hexColor ==
                    hexContainer[triplePointContainer[i, 0]].hexColor &&
                    hexContainer[triplePointContainer[i, 2]].hexColor ==
                    hexContainer[triplePointContainer[i, 0]].hexColor)
                {
                    colorlesTriplePointHolder.Add(i);
                }
            }
        }

        //Alternating Lock Screen (more efficiant way)
        void AlternativeLockingScreen(bool key)
        {
            TouchScreenLockingObj.SetActive(key);
        }

        // Enable or Disable Hex groups (alternative function)
        void LockSelectableTriplePoints(bool key)
        {
            foreach (var item in triplePointPrefab)
            {
                item.GetComponent<UnityEngine.UI.Button>().interactable = key;
            }
        }
       
    }


    // After detecting same color groups this function groups executes
    public void SlidingColors()
    {
        #region -------------------(1st Try)Official Approach----------------------

        List<List<int>> coloumHexes = new List<List<int>>();
        List<List<Hexagon>> decoyHexes = new List<List<Hexagon>>();

        List<List<int>> dropingColorlessHex = new List<List<int>>();
        List<List<int>> dropingHexList = new List<List<int>>();
        List<List<int>> dropingHexPath = new List<List<int>>();

        List<int> pickedLines = new List<int>();

        animationSequenceList = new List<Sequence>();

        waitSlindingHex = true;

        ReleaseCapsulatedHexes();

        ScanWholeHexes();

        RevealOrHideHexes(false);

        AnimationPhase();

        // After animation phase endend this function execute endphase
        animationSequenceList[animationSequenceList.Count - 1].OnComplete(() => EndPhase());


        // Release selected hex groups from hex group parent
        void ReleaseCapsulatedHexes()
        {
            if (capsulatingObj.transform.childCount > 0)
            {
                hexContainer[debugEncapsulatedPrevious[0]].hexObject.GetComponent<Transform>().SetParent(HexagonParent.transform);
                hexContainer[debugEncapsulatedPrevious[1]].hexObject.GetComponent<Transform>().SetParent(HexagonParent.transform);
                hexContainer[debugEncapsulatedPrevious[2]].hexObject.GetComponent<Transform>().SetParent(HexagonParent.transform);

                hexContainer[debugEncapsulatedPrevious[0]].hexObject.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
                hexContainer[debugEncapsulatedPrevious[1]].hexObject.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
                hexContainer[debugEncapsulatedPrevious[2]].hexObject.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
            }
        }

        // Scaning whole hexes by coloum and shifting hexes also taking the hex numbers for animation
        void ScanWholeHexes()
        {
            int cSEven() { return (hexWidthSize - 1) * 2; }


            ListingColoums(coloumHexes);

            ScaningColoums();

            ShiftingColoums();


            // Dividing number for each coloums
            void ListingColoums(List<List<int>> subColoumHexes)
            {
                for (int i = 0; i < hexWidthSize; i++)
                {
                    subColoumHexes.Add(new List<int>());

                    if (i % 2 == 0)
                    {
                        EmplacingHexes(subColoumHexes, i, i);
                    }
                    else
                    {
                        if (i == hexWidthSize - 1)
                        {
                            if (hexWidthSize % 2 == 0)
                            {
                                EmplacingHexes(subColoumHexes, i, i + hexWidthSize - 2);
                            }
                            else
                            {
                                EmplacingHexes(subColoumHexes, i, i - 1);
                            }
                        }
                        else
                        {
                            EmplacingHexes(subColoumHexes, i, i + (hexWidthSize - 1));
                        }
                    }
                }

                // Emplacing hexes for each divided coloums
                void EmplacingHexes(List<List<int>> coloumHexesSub, int i, int coloumNumber)
                {
                    for (int t = 0; t < triplePointContainer.GetLength(0); t++)
                    {
                        if (t % cSEven() == coloumNumber)
                        {
                            if (coloumHexesSub[i].Count < 1)
                            {
                                coloumHexesSub[i].Add(triplePointContainer[t, 0]);
                                coloumHexesSub[i].Add(triplePointContainer[t, 2]);
                            }
                            else
                            {
                                coloumHexesSub[i].Add(triplePointContainer[t, 2]);
                            }
                        }
                    }
                }
            }

            void ScaningColoums()
            {
                for (int i = 0; i < coloumHexes.Count; i++)
                {
                    for (int t = 0; t < coloumHexes[i].Count; t++)
                    {
                        if (hexContainer[coloumHexes[i][t]].hexColor == HexColor.Colorless)
                        {
                            if (!pickedLines.Contains(i))
                            {
                                pickedLines.Add(i);

                                dropingColorlessHex.Add(new List<int>());
                                dropingHexList.Add(new List<int>());
                                dropingHexPath.Add(new List<int>());

                                Debug.Log("Picked Line #: " + i);
                            }

                            dropingColorlessHex[pickedLines.Count - 1].Add(coloumHexes[i][t]);

                            Debug.Log("Colorless Hex #: " + coloumHexes[i][t]);
                        }
                    }
                }
            }

            void ShiftingColoums()
            {
                for (int i = 0; i < pickedLines.Count; i++)
                {
                    for (int t = coloumHexes[pickedLines[i]].Count - 1; t >= 0; t--)
                    {
                        if (hexContainer[coloumHexes[pickedLines[i]][t]].hexColor == HexColor.Colorless && t != 0)
                        {
                            for (int k = t - 1; k >= 0; k--)
                            {
                                if (hexContainer[coloumHexes[pickedLines[i]][k]].hexColor != HexColor.Colorless)
                                {
                                    dropingHexList[i].Add(coloumHexes[pickedLines[i]][k]);
                                    dropingHexPath[i].Add(coloumHexes[pickedLines[i]][t]);

                                    // Shifting upper hex to least bottom colorless hex after make, the hex colorless
                                    hexContainer[coloumHexes[pickedLines[i]][t]].HexColor = hexContainer[coloumHexes[pickedLines[i]][k]].hexColor;
                                    hexContainer[coloumHexes[pickedLines[i]][k]].HexColor = HexColor.Colorless;

                                    Debug.Log("Hex #: " + coloumHexes[pickedLines[i]][k] + " Hex Path: " + coloumHexes[pickedLines[i]][t]);

                                    break;
                                }
                            }
                        }
                    }
                }
            }


            #region Debuging
            //int y = 0;
            //foreach (var item in coloumHexes)
            //{
            //    Debug.Log("Coloum #: " + y);
            //    y++;
            //    foreach (var subItem in item)
            //    {
            //        Debug.Log("Hex: " + subItem);
            //    }
            //}
            #endregion
        }

        // Hiding hexes for decoy animating ones
        void RevealOrHideHexes(bool reveal)
        {
            for (int i = 0; i < pickedLines.Count; i++)
            {
                for (int t = 0; t < dropingHexPath[i].Count; t++)
                {
                    hexContainer[dropingHexPath[i][t]].hexObject.SetActive(reveal);
                    hexContainer[dropingHexList[i][t]].hexObject.SetActive(reveal);
                }

                for (int k = 0; k < dropingColorlessHex[i].Count; k++)
                {
                    hexContainer[dropingColorlessHex[i][k]].hexObject.SetActive(reveal);
                }
            }
        }


        void AnimationPhase()
        {

            GenerateByOrderAndAnimate();

            void GenerateByOrderAndAnimate()
            {
                for (int i = 0; i < pickedLines.Count; i++)
                {
                    int debugNum = 0;
                    float delayTime = 0f;

                    animationSequenceList.Add(DOTween.Sequence());
                    decoyHexes.Add(new List<Hexagon>());

                    for (int l = 0; l < dropingHexList[i].Count; l++)
                    {
                        GenerateHex(decoyHexes[i],
                                    dropingHexList[i][l],
                                    hexContainer[dropingHexList[i][l]].PosX,
                                    hexContainer[dropingHexList[i][l]].PosY,
                                    hexContainer[dropingHexPath[i][l]].hexColor,
                                    debugNum,
                                    hexContainer[dropingHexList[i][l]].isItBomb);

                        // Making destination for generated decoy hex
                        animationSequenceList[i].Insert(
                            0,
                            AnimateHex(
                                decoyHexes[i][debugNum], 
                                (int)hexContainer[dropingHexPath[i][l]].hexObject.GetComponent<RectTransform>().localPosition.y,
                                delayTime)
                            );

                        delayTime += 0.1f;
                        debugNum++;
                    }

                    for (int c = dropingColorlessHex[i].Count - 1; c >= 0; c--)
                    {
                        HexColor newHexColor = (HexColor)Random.Range(0, 5);
                        bool isItBomb;

                        if (Random.Range(0, 10) == 4) isItBomb = true;
                        else isItBomb = false;

                        GenerateHex(decoyHexes[i],
                                    coloumHexes[pickedLines[i]][c],
                                    hexContainer[coloumHexes[pickedLines[i]][c]].PosX,
                                    500,
                                    newHexColor,
                                    debugNum,
                                    isItBomb);

                        // Decideing bomb or hex
                        hexContainer[coloumHexes[pickedLines[i]][c]].isItBomb = isItBomb;

                        // Changing the real hex color
                        hexContainer[coloumHexes[pickedLines[i]][c]].HexColor = newHexColor;

                        // Making destination for generated decoy hex
                        animationSequenceList[i].Insert(
                            0,
                            AnimateHex(
                                decoyHexes[i][debugNum],
                                (int)hexContainer[coloumHexes[pickedLines[i]][c]].hexObject.GetComponent<RectTransform>().localPosition.y,
                                delayTime)   
                            );

                        // Adding score
                        scoreSystem.AddScore(5);

                        delayTime += 0.1f;
                        debugNum++;
                    }

                }
            }

            void GenerateHex(List<Hexagon> _decoyHexes, int _hexNumber, int posX, int posY, HexColor hexcolor, int _debugNum, bool isItBomb)
            {
                _decoyHexes.Add(new Hexagon(
                                        hexWidth,
                                        hexHeight,
                                        hexContainer[_hexNumber].placementNumber,
                                        posX,
                                        posY,
                                        hexcolor,
                                        HexPrefab,
                                        HexagonParent.transform,
                                        isItBomb
                                        ));

                _decoyHexes[_debugNum].HexColor = hexcolor;

                //_decoyHexes[_debugNum].DebugTexting("" + hexContainer[_hexNumber].placementNumber);
                //debugNum++;

                //Debug.Log("DecoyHex#:  " + (debugNum - 1) + "  Hex#From_ForAnimation:  " + _hexNumber);
            }

            Tween AnimateHex(Hexagon _decoyhex, int hexDestination, float delay)
            {
                return _decoyhex.Animation(hexDestination, 0.2f).SetDelay(delay);
            }
        }


        void DestroyDecoyHexes()
        {
            for (int i = 0; i < decoyHexes.Count; i++)
            {
                for (int t = 0; t < decoyHexes[i].Count; t++)
                {
                    Destroy(decoyHexes[i][t].hexObject);
                }
            }
        }


        void EndPhase()
        {
            DestroyDecoyHexes();

            RevealOrHideHexes(true);

            EncapsulateColors();

            waitSlindingHex = false;
        }


        #endregion

    }


    // Encapsulate the area where was touched 
    public void SelectEncapsulatedArea(int capsulatingNumber)
    {
        if(capsulatingObj.transform.childCount > 0)
        {
            hexContainer[debugEncapsulatedPrevious[0]].hexObject.GetComponent<Transform>().SetParent(HexagonParent.transform);
            hexContainer[debugEncapsulatedPrevious[1]].hexObject.GetComponent<Transform>().SetParent(HexagonParent.transform);
            hexContainer[debugEncapsulatedPrevious[2]].hexObject.GetComponent<Transform>().SetParent(HexagonParent.transform);
        }

        capsulatingObj.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;

        capsulatingObj.GetComponent<RectTransform>().localPosition = encapsulatedPointPositions[capsulatingNumber];

        //Upper Hex inside the encapsulated point
        hexContainer[triplePointContainer[capsulatingNumber, 0]].hexObject.GetComponent<Transform>().SetParent(capsulatingObj.transform);
        debugEncapsulatedPrevious[0] = triplePointContainer[capsulatingNumber, 0];
        //Side Hex inside the encapsulated point
        hexContainer[triplePointContainer[capsulatingNumber, 1]].hexObject.GetComponent<Transform>().SetParent(capsulatingObj.transform);
        debugEncapsulatedPrevious[1] = triplePointContainer[capsulatingNumber, 1];
        //Lower Hex inside the encapsulated point
        hexContainer[triplePointContainer[capsulatingNumber, 2]].hexObject.GetComponent<Transform>().SetParent(capsulatingObj.transform);
        debugEncapsulatedPrevious[2] = triplePointContainer[capsulatingNumber, 2];

        encapsulatedTriplePoint = capsulatingNumber;

        #region Debug
        //Debug.Log("TriplePoint: " + encapsulatedTriplePoint + "\t" + "\t" +
        //            "Upper: " + triplePointContainer[capsulatingNumber, 0] + "\t" +
        //            "Side: " + triplePointContainer[capsulatingNumber, 1] + "\t" +
        //            "Lower: " + triplePointContainer[capsulatingNumber, 2] + "\t");
        #endregion
    }


    [ContextMenu("DebugPointText")]
    public void DebugPointText()
    {
        for (int i = 0; i < totalTriplePointNumber; i++)
        {
            debugObj = Instantiate(debugPointTextObj, TouchPointParent.transform);
            debugObj.GetComponent<UnityEngine.UI.Text>().text = "" + i;
            debugObj.gameObject.name = "Triple Point " + i;
            debugObj.GetComponent<RectTransform>().localPosition = encapsulatedPointPositions[i];
        }
    }


}
