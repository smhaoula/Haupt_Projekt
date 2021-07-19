using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    PlayerMovement player;
    public static int scoreValue;
    public int levelLimit;
    public static int currentScore;
    public ScoreBar scoreBar;
    public HealthBar healthBar;
    Text score;
    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
        levelLimit = 30;
        currentScore = 0;
        scoreBar.SetMaxScore(levelLimit);
        
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Score " + scoreValue;
        scoreBar.SetScore(currentScore);

        if(currentScore >= levelLimit)
        {
            LevelUp();
            IncreaseLevelLimit();
            scoreBar.SetMaxScore(levelLimit);
        }
    }

    public void IncreaseLevelLimit()
    {
        levelLimit +=20;
        currentScore = 0;    
    }

    public void LevelUp(){
        player = FindObjectOfType<PlayerMovement>();
        player.IncreasePlayerDamage();
        int MaxHealth = player.Health;
        healthBar.SetHealth(MaxHealth);
    }
}
