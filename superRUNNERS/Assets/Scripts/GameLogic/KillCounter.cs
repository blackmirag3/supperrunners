using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    [SerializeField]
    private float needToKill;
    [SerializeField]
    private int leftToKill = 0;
    private int enemiesKilled;

    public TextMeshProUGUI countDisplay;
    public TextMeshProUGUI killsDisplay;
    public GameObject victoryScreen;

    public GameEvent gameStopped;

    public bool LevelWon { get; private set; }

    private void Start()
    {
        enemiesKilled = 0;
        LevelWon = false;
    }

    private void UpdateText()
    {
        UpdateTextCol();
        countDisplay.SetText(leftToKill.ToString());
        killsDisplay.SetText(enemiesKilled.ToString());
    }

    private void UpdateTextCol()
    {
        if (leftToKill > needToKill * 0.66f)
        {
            // Green
            countDisplay.color = new Color32(0, 255, 0, 255);
        }
        else if (leftToKill > needToKill * 0.33f)
        {
            // Yellow
            countDisplay.color = new Color32(255, 255, 0, 255);
        }
        else
        {
            // Red
            countDisplay.color = new Color32(255, 0, 0, 255);
        }
    }

    public void OnEnemyKill()
    {
        leftToKill--;
        enemiesKilled++;
        UpdateText();

        // Check for all enemies killed within single level
        if (leftToKill == 0 && !LevelWon)
        {
            LevelWon = true;
            gameStopped.CallEvent(this, null);
        }
    }

    public void ResetKillCount(Component sender, object data)
    {
        if (data is int)
        {
            int enemyCount = (int)data;
            Debug.Log("Kill count resetting");
            LevelWon = false;
            needToKill = enemyCount;
            leftToKill = enemyCount;
            UpdateText();
        }
        else
        {
            Debug.Log($"Error in data received from event listener {sender}");
        }
    }
}
