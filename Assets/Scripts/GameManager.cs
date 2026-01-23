using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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

    public BoxCollider castleCollider;
    public GameObject shields;
    public ArcherController archer1, archer2, archer3;
    public Transform viewTarget;
    public QueenBehaviour theQueen;

    public int heroDamageMultiplier;
    public float[] archerSpeedMultiplier = {1, 1, 1};
    public int[] archerDamageMultiplier = {1, 1, 1};
    public float enemyHealthMultiplier;
    
    public float jumpDropCooldownTime = 5f;

    public int selectedEnemy;// 0 is for goblin, change it for difficulty
    [SerializeField] private int numOfEnemies;
    public int slainEnemies;
    

    private bool _deathMenuIsOn = false;
    private float _recordedIntervalTime;
    private float _startingAttackRange;
    
    public HashSet<EnemyBehaviour> AliveEnemies { get; } = new();

    private void Awake()
    {
        Instance = this;
        archerSpeedMultiplier = new[] {1f, 1f, 1f};
        archerDamageMultiplier = new[] {1, 1, 1};
        _startingAttackRange = attackRange;
    }

    private void Start()
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
        _recordedIntervalTime = spawnIntervalTime;
        FindFirstObjectByType<PlayerHeroController>().SetAttackRange(attackRange, attackRange / _startingAttackRange);
        StartCoroutine(SpawnIntervals());
        StartCoroutine(PowerUpIntervals());

        for (int i = 0; i < 3; i++)
        {
            UpdateArcherSpeed(i);
        }
    }

    private void UpdatePhase()
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
            > 200 and <= 400 => 8,
            > 400 and <= 600=> 9,
            > 600 => 10,
            _ => phase
        };

        //Phase system
        switch (phase)
        {
            // Beginning Phase: Only first goblins.
            case 1:
                spawnIntervalTime = _recordedIntervalTime - (numOfEnemies / 10f);
                selectedEnemy = 0;
                break;

            // Second Phase: Include giants.
            case 2:
                spawnIntervalTime = _recordedIntervalTime - 1f - (numOfEnemies / 20f);
                selectedEnemy = fatedNumber < 90 ? 0 : 1;
                break;

            // Third Phase: Include shielders.
            case 3:
                spawnIntervalTime = _recordedIntervalTime - 2f - (numOfEnemies / 30f);
                selectedEnemy = fatedNumber switch {
                    < 51 => 0,
                    < 90 => 1,
                    _ => 2 };
                break;

            // Fourth Phase: Include fighters.
            case 4:
                spawnIntervalTime = _recordedIntervalTime - 4f;
                enemyHealthMultiplier = 1.5f;
                selectedEnemy = fatedNumber switch {
                    < 21 => 0,
                    < 50 => 1,
                    < 76 => 2,
                    _ => 3 };
                break;

            // Fifth Phase: Include bombers.
            case 5:
                spawnIntervalTime = 1.6f;
                selectedEnemy = fatedNumber switch {
                    < 16 => 0,
                    < 26 => 1,
                    < 51 => 2,
                    < 76 => 3,
                    _ => 4 };
                break;

            // Sixth Phase: Release the Kraken.
            case 6:
                spawnIntervalTime = 1.4f;
                enemyHealthMultiplier = 2f;
                selectedEnemy = fatedNumber switch {
                    < 16 => 0,
                    < 26 => 1,
                    < 51 => 2,
                    < 76 => 3,
                    _ => 4 };
                // Pave the way for her.

                break;

            // Seventh and the Final Phase: This is the endgame. PREPARE FOR THE BOSS FIGHT.
            case 7:
                spawnIntervalTime = 1.2f;
                theQueen.SetActive(true);
                // She has arrived.

                selectedEnemy = fatedNumber switch {
                    < 5 => 0,
                    < 10 => 1,
                    < 51 => 2,
                    < 76 => 3,
                    _ => 4 };

                break;
            case 8:
                theQueen.SetActive(true);
                var inverseLerp = Mathf.InverseLerp(200f, 400, numOfEnemies);
                spawnIntervalTime = Mathf.Lerp(1f, 0.2f, inverseLerp);
                theQueen.SetSpeed(Mathf.Lerp(1f, 2f, inverseLerp));
                
                selectedEnemy = fatedNumber switch {
                    < 33 => 2,
                    < 66 => 3,
                    _ => 4 };
                break;
            case 9:
                theQueen.SetActive(true);
                inverseLerp = Mathf.InverseLerp(400f, 600f, numOfEnemies);
                spawnIntervalTime = Mathf.Lerp(0.2f, 0.1f, inverseLerp);
                theQueen.SetSpeed(Mathf.Lerp(2f, 6f, inverseLerp));
                
                selectedEnemy = fatedNumber switch {
                    < 33 => 2,
                    < 66 => 3,
                    _ => 4 };
                break;
            case 10:
                theQueen.SetActive(true);
                spawnIntervalTime = 0.05f;
                theQueen.SetSpeed(10f);

                selectedEnemy = 4;
                break;
        }
    }
    
    public int GetSpawnCountFromSpell()
    {
        return phase switch
        {
            < 8 => Random.Range(1, 3),
            < 9 => Random.Range(2, 5),
            < 10 => Random.Range(5, 10),
            _ => 30
        };
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

    private IEnumerator SpawnIntervals()
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
        
        if (health <= 0 && !_deathMenuIsOn)
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
        UpdateArcherSpeed(number);
    }

    private void UpdateArcherSpeed(int number)
    {
        if(number == 0) archer1.SetSpeed(archerSpeedMultiplier[number]);
        else if(number == 1) archer2.SetSpeed(archerSpeedMultiplier[number]);
        else if(number == 2) archer3.SetSpeed(archerSpeedMultiplier[number]);
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

    private IEnumerator Unshield(int time)
    {
        yield return new WaitForSeconds(time);
        shielded = false;
        shields.SetActive(false);
        FindFirstObjectByType<PlayerMatManager>().GoRed();
    }

    public void IncreaseSlainEnemies(EnemyBehaviour diedEnemy)
    {
        slainEnemies++;
        UIManager.Instance.SetEnemyNumber(slainEnemies);
        AliveEnemies.Remove(diedEnemy);
    }
    
    public void UpdateShopItems()
    {
        UIManager.Instance.UpdateButtons(coin);
    }

    private void CastleIsDestroyed()
    {
        _deathMenuIsOn = true;
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

    private bool _waiting;
    public void HitStop(float duration)
    {
        if (_waiting) return;
        Time.timeScale = 0.0f;
        StartCoroutine(HitStopWait(duration));
    }

    private IEnumerator HitStopWait(float duration)
    {
        _waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        _waiting = false;
    }
    #endregion
}
