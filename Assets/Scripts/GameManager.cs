using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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
    public float attackRange = 1.5f;
    public bool enemyTargeting;
    public bool shielded;

    public GameObject shields;
    public Transform viewTarget;

    public int heroDamageMultiplier;
    public float[] archerSpeedMultiplier = {1, 1, 1};
    public int[] archerDamageMultiplier = {1, 1, 1};
    public float enemyHealthMultiplier;
    
    public float jumpDropCooldownTime = 5f;

    public int selectedEnemy;// 0 is for goblin, change it for difficulty
    [SerializeField] private int numOfEnemies;
    public int slainEnemies;

    bool deathMenuIsOn = false;

    float recordedIntervalTime;
    public QueenBehaviour theQueen;
    
    private float _startingAttackRange;

    private void Awake()
    {
        Instance = this;
        archerSpeedMultiplier = new[] {1f, 1f, 1f};
        archerDamageMultiplier = new[] {1, 1, 1};
        _startingAttackRange = attackRange;
    }

    void Start()
    {
        // Application.targetFrameRate = 60;
        coin = 0;
        phase = 1;
        Screen.orientation = ScreenOrientation.Portrait;
        Time.timeScale = 1f;
        health = 100;
        selectedEnemy = 0;
        numOfEnemies = 0;
        slainEnemies = 0;
        AudioManager.Instance.Play(ClipType.ThemeMusic);
        recordedIntervalTime = spawnIntervalTime;
        FindFirstObjectByType<PlayerHeroController>().SetAttackRange(attackRange, attackRange / _startingAttackRange);
        StartCoroutine(SpawnIntervals());
        StartCoroutine(PowerUpIntervals());
    }

    void UpdatePhase()
    {
        phase = numOfEnemies switch
        {
            >= 0 and <= 20 => 1,
            > 20 and <= 40 => 2,
            > 40 and <= 60 => 3,
            > 60 and <= 80 => 4,
            > 80 and <= 100 => 5,
            > 100 and <= 120 => 6,
            > 120 and <= 200 => 7,
            > 200  and <= 1000 => 8,
            > 1000 => 9,
            _ => phase
        };

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
                // She has arrived.
                break;

            case 8:
                var inverseLerp = Mathf.InverseLerp(200f, 1000f, numOfEnemies);
                spawnIntervalTime = Mathf.Lerp(1f, 0.2f, inverseLerp);
                theQueen.SetSpeed(Mathf.Lerp(1f, 2f, inverseLerp));
                break;
            case 9:
                inverseLerp = Mathf.InverseLerp(1000f, 2600f, numOfEnemies);
                spawnIntervalTime = Mathf.Lerp(0.2f, 0.1f, inverseLerp);
                theQueen.SetSpeed(Mathf.Lerp(2f, 6f, inverseLerp));
                break;
        }
    }

    private IEnumerator PowerUpIntervals()
    {
        while (this)
        {
            yield return new WaitForSeconds(powerupIntervalTime);
            int powerupRange = Random.Range(0, powerups.Length);
            Vector3 powerupLoc = new Vector3(Random.Range(-8, 8), 1, Random.Range(-10, 20));
            Instantiate(powerups[powerupRange], powerupLoc, Quaternion.identity);
        }
    }
    
    IEnumerator SpawnIntervals()
    {
        while (this)
        {
            Instantiate(enemies[selectedEnemy], spawner.transform.position, new Quaternion(0,180,0,0));
            numOfEnemies++;
            fatedNumber = Random.Range(0, 101);
            yield return new WaitForSeconds(spawnIntervalTime);
            UpdatePhase();
        }
    }
    
    public void CastleHealthDecreases(int valueChanged)
    {
        health -= valueChanged;
        UIManager.Instance.SetHealth(health);
        
        if (health <= 0 && !deathMenuIsOn)
        {
            CastleIsDestroyed();
        }
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
    public void IncreaseCoinAmount(int coinAmount)
    {
        coin += coinAmount;
        UIManager.Instance.SetCoin(coin);
    }
    public void ChangeCoinAmount(int change)
    {
        coin += change;
        UIManager.Instance.SetCoin(coin);
    }
    public void ActivateArcher(int number)
    {
        archers[number].SetActive(true);
    }
    
    public void ActivateFastenArcher(int number)
    {
        archerSpeedMultiplier[number] += 0.5f;
    }
    
    public void ActivateDamageArcher(int number)
    {
        archerDamageMultiplier[number] *= 2;
    }
    
    public void IncreaseDamage(int increasedValue)
    {
        heroDamageMultiplier += increasedValue;
    }
    
    public void IncreaseRange()
    {
        attackRange += 0.5f;
        FindFirstObjectByType<PlayerHeroController>().SetAttackRange(attackRange, attackRange / _startingAttackRange);
    }
    
    public void StartJumpCooldown()
    {
        UIManager.Instance.StartJumpCooldown(jumpDropCooldownTime);
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
        FindFirstObjectByType<PlayerMatManager>().GoRed();
    }

    public void IncreaseSlainEnemies()
    {
        slainEnemies++;
        UIManager.Instance.SetEnemyNumber(slainEnemies);
    }
    
    public void UpdateShopItems()
    {
        UIManager.Instance.UpdateButtons(coin);
    }
    
    void CastleIsDestroyed()
    {
        deathMenuIsOn = true;
        //open the menu
        UIManager.Instance.OpenDeathMenu();
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
