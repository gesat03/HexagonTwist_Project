using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public ScoreSystem scoreSystem;

    public GridSystem gridSystem;

    private void Start()
    {
        gridSystem.InitializingFunction();
    }

    private void Update()
    {
        gridSystem.TouchMechanism();

        ColorMatch();

    }

    private void ColorMatch()
    {
        if (gridSystem.colorMatch && gridSystem.waitColorCrash)
        {
            StartCoroutine(ColorCrash());
        }

        IEnumerator ColorCrash()
        {
            gridSystem.waitColorCrash = false;
            yield return new WaitForSeconds(1f);
            gridSystem.waitColorCrash = true;

            gridSystem.SlidingColors();

            gridSystem.colorMatch = false;
        }

    }


}
