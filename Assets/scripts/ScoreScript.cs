using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static int scoreValue;
    Text score;
    void Start()
    {
        score = GetComponent<Text>();
        scoreValue = 0;
        score.text = scoreValue + "";
    }

    // Update is called once per frame
    void Update()
    {
        score.text = scoreValue + "";
    }
}
