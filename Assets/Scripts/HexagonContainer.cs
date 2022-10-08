using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonContainer
{

    internal int triplePointNumber;
    internal int pointGroup;
    internal int hexNumber;

     public HexagonContainer (int triplePointNumber, int pointGroup, int hexNumber)
    {
        this.triplePointNumber = triplePointNumber;
        this.pointGroup = pointGroup;
        this.hexNumber = hexNumber;
    }

}
