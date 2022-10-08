using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public Text scoreText;

    private int currentScore = 0;

    public void AddScore(int score)
    {
        currentScore += score;

        scoreText.text = "Score:" + currentScore; 
    }


}
