using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Main Values")]
    public float health;
    public int coin;
    public int phase;
    public int fatedNumber;
    [Header("Other Settings")]
    public bool paused;
    public GameObject spawner, goldCoin;
    public GameObject[] enemies,powerups,archers;
    public float spawnIntervalTime;
    public float powerupIntervalTime;
    public bool enemyTargeting;
    public bool shielded;

    public GameObject shields;

    public int heroDamageMultiplier;
    public int archerDamageMultiplier;
    public float enemyHealthMultiplier;

    public int selectedEnemy;// 0 is for goblin, change it for difficulty
    [SerializeField]
    int numOfEnemies;
    public int slainEnemies;

    bool deathMenuIsOn = false;

    float recordedIntervalTime;
    public GameObject theQueen;

    void Start()
    {
        Application.targetFrameRate = 60;
        coin = 0;
        phase = 1;
        Screen.orientation = ScreenOrientation.Portrait;
        Time.timeScale = 1f;
        health = 100;
        selectedEnemy = 0;
        numOfEnemies = 0;
        slainEnemies = 0;
        FindObjectOfType<AudioManager>().Play("Theme Music");
        recordedIntervalTime = spawnIntervalTime;
        StartCoroutine(SpawnIntervals(spawnIntervalTime, selectedEnemy));
        StartCoroutine(PowerupIntervals());
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !deathMenuIsOn)
        {
            CastleIsDestroyed();
        }
        if (numOfEnemies >= 0 && numOfEnemies <= 20)
            phase = 1;
        else if (numOfEnemies > 20 && numOfEnemies <= 40)
        {
            phase = 2;
        }
        else if (numOfEnemies > 40 && numOfEnemies <= 60)
        {
            phase = 3;
        }
        else if (numOfEnemies > 60 && numOfEnemies <= 80)
        {
            phase = 4;
        }
        else if (numOfEnemies > 80 && numOfEnemies <= 80)
            phase = 5;
        else if (numOfEnemies > 120)
            phase = 6;

        //Phase system
        switch (phase)
        {
            // Beginning Phase: Only first goblins.
            case 1:
                spawnIntervalTime = recordedIntervalTime - (numOfEnemies / 10f);
                selectedEnemy = 0;
                break;

            // Second Phase: Include giants.
            case 2:
                spawnIntervalTime = recordedIntervalTime - 1f - (numOfEnemies / 20f);
                if (fatedNumber < 90) selectedEnemy = 0;
                else selectedEnemy = 1;
                break;

            // Third Phase: Include shielders.
            case 3:
                spawnIntervalTime = recordedIntervalTime - 2f - (numOfEnemies / 30f);
                if (fatedNumber < 51) selectedEnemy = 0;
                else if (fatedNumber < 90) selectedEnemy = 1;
                else selectedEnemy = 2;
                break;

            // Fourth Phase: Include fighters.
            case 4:
                spawnIntervalTime = recordedIntervalTime - 4f;
                enemyHealthMultiplier = 1.5f;
                if (fatedNumber < 21) selectedEnemy = 0;
                else if (fatedNumber < 50) selectedEnemy = 1;
                else if (fatedNumber < 76) selectedEnemy = 2;
                else selectedEnemy = 3;
                break;

            // Fifth Phase: Include bombers.
            case 5:
                spawnIntervalTime = 1.6f;
                if (fatedNumber < 16) selectedEnemy = 0;
                else if (fatedNumber < 26) selectedEnemy = 1;
                else if (fatedNumber < 51) selectedEnemy = 2;
                else if (fatedNumber < 76) selectedEnemy = 3;
                else selectedEnemy = 4;
                break;

            // Sixth Phase: Release the Kraken.
            case 6:
                spawnIntervalTime = 1.4f;
                enemyHealthMultiplier = 2f;
                if (fatedNumber < 16) selectedEnemy = 0;
                else if (fatedNumber < 26) selectedEnemy = 1;
                else if (fatedNumber < 51) selectedEnemy = 2;
                else if (fatedNumber < 76) selectedEnemy = 3;
                else selectedEnemy = 4;
                // Pave the way for her.

                break;

            // Seventh and the Final Phase: This is the endgame. PREPARE FOR THE BOSS FIGHT.
            case 7:
                spawnIntervalTime = 1.2f;
                theQueen.SetActive(true);
                // She has come.
                break;

        }
    }
    IEnumerator PowerupIntervals()
    {
        yield return new WaitForSeconds(powerupIntervalTime);
        int powerupRange = Random.Range(0, powerups.Length);
        Vector3 powerupLoc = new Vector3(Random.Range(-8, 8), 1, Random.Range(-10, 20));
        Instantiate(powerups[powerupRange], powerupLoc, Quaternion.identity);
        StartCoroutine(PowerupIntervals());
    }
    IEnumerator SpawnIntervals(float time, int enemyIndexNumber)
    {
        Instantiate(enemies[enemyIndexNumber], spawner.transform.position, new Quaternion(0,180,0,0));
        numOfEnemies++;
        fatedNumber = Random.Range(0, 101);
        yield return new WaitForSeconds(time);
        StartCoroutine(SpawnIntervals(spawnIntervalTime, selectedEnemy));
    }
    public void CastleHealthDecreases(int valueChanged)
    {
        health -= valueChanged;
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        paused = true;
    }
    public void Unpause()
    {
        Time.timeScale = 1f;
        paused = false;
    }
    public void IncreaseCoinAmount()
    {
        coin++;
    }
    public void ChangeCoinAmount(int change)
    {
        coin += change;
    }
    public void ActivateArcher(int number)
    {
        archers[number].SetActive(true);
    }
    public void IncreaseDamage(int increasedValue)
    {
        heroDamageMultiplier += increasedValue;
    }
    public void IncreaseArchersDamage(int increasedValue)
    {
        archerDamageMultiplier += increasedValue;
    }
    public void Shielded(int time)
    {
        StartCoroutine(Unshield(time));
        shielded = true;
        shields.SetActive(true);
    }
    IEnumerator Unshield(int time)
    {
        yield return new WaitForSeconds(time);
        shielded = false;
        shields.SetActive(false);
        FindObjectOfType<PlayerMatManager>().GoRed();
    }

    public void IncreaseSlainEnemies()
    {
        slainEnemies++;
    }
    void CastleIsDestroyed()
    {
        deathMenuIsOn = true;
        //open the menu
        FindObjectOfType<UIManager>().OpenDeathMenu();
        Time.timeScale = 0f;
        //Debug.LogError("You are defeated");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    #region Time Management
    bool waiting;
    public void HitStop(float duration)
    {
        if (waiting)
            return;
        Time.timeScale = 0.0f;
        StartCoroutine(HitStopWait(duration));
    }

    IEnumerator HitStopWait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        waiting = false;
    }
    #endregion
}
