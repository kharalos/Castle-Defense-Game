using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum EnemyClass
{
    minion,
    giant,
    shielder,
    fighter,
    bomber
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    private static readonly int IsMinion = Animator.StringToHash("isMinion");
    private static readonly int IsShielder = Animator.StringToHash("isShielder");
    private static readonly int IsFighter = Animator.StringToHash("isFighter");
    private static readonly int IsBomber = Animator.StringToHash("isBomber");
    private static readonly int SpeedPercent = Animator.StringToHash("speedPercent");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int InterruptTheAttack = Animator.StringToHash("interruptTheAttack");
    private static readonly int Die = Animator.StringToHash("die");
    private static readonly int Hurt = Animator.StringToHash("hurt");
    public EnemyClass enemyClass;

    private NavMeshAgent _agent;
    private PlayerHeroController _hero;
    private Vector3 _castleCl;
    private Vector3 _agentLoc;
    public float distance;
    private Animator _animator;
    public float health;
    private float _maxHealth;
    public Image healthBar;
    [FormerlySerializedAs("healthBarBG")] public Image healthBarBg;
    public Transform healthBarTransform;
    [FormerlySerializedAs("EnemyIsAlive")] public bool enemyIsAlive;
    public bool notHit;

    private void Start()
    {
        health *= GameManager.Instance.enemyHealthMultiplier;
        _maxHealth = health;
        notHit = true;
        enemyIsAlive = true;
        _agent = GetComponent<NavMeshAgent>();
        _agentLoc = _agent.transform.position;
        _animator = GetComponent<Animator>();
        _castleCl = GameObject.FindGameObjectWithTag("Castle").GetComponent<BoxCollider>().ClosestPoint(_agentLoc);
        DetermineClass();
        
        _hero = FindFirstObjectByType<PlayerHeroController>();
    }

    private void DetermineClass()
    {
        _animator.SetBool(IsMinion, enemyClass is EnemyClass.minion or EnemyClass.giant);

        _animator.SetBool(IsShielder, enemyClass == EnemyClass.shielder);

        _animator.SetBool(IsFighter, enemyClass == EnemyClass.fighter);

        _animator.SetBool(IsBomber, enemyClass == EnemyClass.bomber);
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (enemyIsAlive) {
            if (enemyClass != EnemyClass.fighter)
            {
                if ((_castleCl - _agentLoc).magnitude > distance)
                    _agent.SetDestination(_castleCl);
                else //Enemy is at the destination and should stop and attack
                {
                    _agent.SetDestination(_agentLoc);
                    EnemyAttacksCastle();
                }
            }
            else if (enemyClass == EnemyClass.fighter)
            {
                Vector3 heroPos = _hero.gameObject.transform.position;
                if ((heroPos - _agentLoc).magnitude > distance)
                {
                    _agent.SetDestination(heroPos);
                    InterruptAttack();
                }

                else
                {
                    _agent.SetDestination(heroPos);
                    EnemyAttacksHero();
                }
            } 
        }
        else if(enemyClass == EnemyClass.fighter)
        {
            InterruptAttack();
        }

        _agentLoc = _agent.transform.position;

        float speedPercent = _agent.velocity.magnitude / _agent.speed;
        _animator.SetFloat(SpeedPercent, speedPercent, .1f, Time.deltaTime);
        healthBar.fillAmount = health/_maxHealth;
        if (health > _maxHealth)
            healthBar.color = Color.red;
        else
            healthBar.color = Color.green;


        // Do not forget to delete this before relase, or not I mean it is a mobile game
        if (Input.GetKeyDown(KeyCode.Space))
            EnemyTakesDamage(100);
    }
    private void LateUpdate()
    {
        healthBarTransform.LookAt(GameObject.Find("ViewTarget").transform);
        //healthBarTransform.rotation = Quaternion.LookRotation(healthBarTransform.position - Camera.main.transform.position);
    }

    private void EnemyAttacksCastle()
    {
        _animator.SetTrigger(Attack);
    }

    private void EnemyAttacksHero()
    {
        _animator.SetTrigger(Attack);
    }

    private void InterruptAttack()
    {
        _animator.ResetTrigger(Attack);
        _animator.SetTrigger(InterruptTheAttack);
    }

    private void FighterHitsHero()
    {
        StartCoroutine(FindFirstObjectByType<PlayerAnimator>().HeroKnockedback(transform.position));
    }

    private void BomberExplodes()
    {
        if (enemyIsAlive && !GameManager.Instance.shielded)
        {
            GameManager.Instance.CastleHealthDecreases(50);
            AudioManager.Instance.Play("Explosion");
            FindFirstObjectByType<ExplosionController>().Explode(transform.position);
            enemyIsAlive = false;
        }
        Destroy(gameObject, .2f);
    }

    private void GoblinDamagesCastle()
    {
        if (enemyIsAlive&&!GameManager.Instance.shielded)
        {
            GameManager.Instance.CastleHealthDecreases(10);
            AudioManager.Instance.Play("Castle Hit");
        }
    }
    public void EnemyTakesDamage(float damageValue)
    {
        health -= damageValue;
        healthBar.fillAmount = health;
        AudioManager.Instance.Play("Enemy Hurts"); //You sadistic piece of shit
        _animator.SetTrigger(Hurt);
        //GameManager.Instance.HitStop(0.06f);
        if (health <= 0&&enemyIsAlive)
            EnemyDies();
    }

    private void EnemyDies()
    {
        Instantiate(GameManager.Instance.goldCoin, new Vector3(transform.position.x, 4f, transform.position.z),Quaternion.identity);
        enemyIsAlive = false;
        _agent.isStopped = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        _agent.enabled = false;
        if(enemyClass == EnemyClass.giant) AudioManager.Instance.Play("Giant Dies");
        else AudioManager.Instance.Play("Goblin Dies");
        GameManager.Instance.IncreaseSlainEnemies();
        //dead animation
        _animator.SetTrigger(Die);
        Destroy(this.gameObject, 3f);
        healthBarBg.CrossFadeAlpha(0, .15f, false);
    }
}
