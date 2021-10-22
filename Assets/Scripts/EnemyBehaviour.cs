using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    public EnemyClass enemyClass;

    GameManager gm;

    NavMeshAgent agent;
    Vector3 castleCL;
    Vector3 agentLoc;
    public Vector3 destination;
    public float distance;
    Animator animator;
    public float health;
    float maxHealth;
    public Image healthBar;
    public Image healthBarBG;
    public Transform healthBarTransform;
    public bool EnemyIsAlive;
    public bool notHit;
    void Start()
    {
        if(GameObject.Find("GameManager").GetComponent<GameManager>())
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        else
        {
            Debug.LogError("Game Manager could not be found.");
        }
        health *= gm.enemyHealthMultiplier;
        maxHealth = health;
        notHit = true;
        EnemyIsAlive = true;
        agent = GetComponent<NavMeshAgent>();
        agentLoc = agent.transform.position;
        animator = GetComponent<Animator>();
        castleCL = GameObject.FindGameObjectWithTag("Castle").GetComponent<BoxCollider>().ClosestPoint(agentLoc);
        DetermineClass();
    }
    void DetermineClass()
    {
        if (enemyClass == EnemyClass.minion || enemyClass == EnemyClass.giant)
            animator.SetBool("isMinion", true);
        else
            animator.SetBool("isMinion", false);

        if (enemyClass == EnemyClass.shielder)
            animator.SetBool("isShielder", true);
        else
            animator.SetBool("isShielder", false);

        if (enemyClass == EnemyClass.fighter)
            animator.SetBool("isFighter", true);
        else
            animator.SetBool("isFighter", false);

        if (enemyClass == EnemyClass.bomber)
            animator.SetBool("isBomber", true);
        else
            animator.SetBool("isBomber", false);
    }
    // Update is called once per frame
    void Update()
    {
        if (EnemyIsAlive) {
            if (enemyClass != EnemyClass.fighter)
            {
                if ((castleCL - agentLoc).magnitude > distance)
                    agent.SetDestination(castleCL);
                else //Enemy is at the destination and should stop and attack
                {
                    agent.SetDestination(agentLoc);
                    EnemyAttacksCastle();
                }
            }
            else if (enemyClass == EnemyClass.fighter)
            {
                Vector3 heroPos = FindObjectOfType<PlayerHeroController>().gameObject.transform.position;
                if ((heroPos - agentLoc).magnitude > distance)
                {
                    agent.SetDestination(heroPos);
                    InterruptAttack();
                }

                else
                {
                    agent.SetDestination(heroPos);
                    EnemyAttacksHero();
                }
            } 
        }
        else if(enemyClass == EnemyClass.fighter)
        {
            InterruptAttack();
        }

        agentLoc = agent.transform.position;

        destination = agent.destination;

        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, .1f, Time.deltaTime);
        healthBar.fillAmount = health/maxHealth;
        if (health > maxHealth)
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
    void EnemyAttacksCastle()
    {
        animator.SetTrigger("attack");
    }
    void EnemyAttacksHero()
    {
        animator.SetTrigger("attack");
    }
    void InterruptAttack()
    {
        animator.ResetTrigger("attack");
        animator.SetTrigger("interruptTheAttack");
    }
    void FighterHitsHero()
    {
        StartCoroutine(FindObjectOfType<PlayerAnimator>().HeroKnockedback(transform.position));
    }
    void BomberExplodes()
    {
        if (EnemyIsAlive && !FindObjectOfType<GameManager>().shielded)
        {
            gm.CastleHealthDecreases(50);
            FindObjectOfType<AudioManager>().Play("Explosion");
            FindObjectOfType<ExplosionController>().Explode(transform.position);
            EnemyIsAlive = false;
        }
        Destroy(gameObject, .2f);
    }
    void GoblinDamagesCastle()
    {
        if (EnemyIsAlive&&!FindObjectOfType<GameManager>().shielded)
        {
            gm.CastleHealthDecreases(10);
            FindObjectOfType<AudioManager>().Play("Castle Hit");
        }
    }
    public void EnemyTakesDamage(float damageValue)
    {
        health -= damageValue;
        healthBar.fillAmount = health;
        FindObjectOfType<AudioManager>().Play("Enemy Hurts"); //You sadistic piece of shit
        animator.SetTrigger("hurt");
        //gm.HitStop(0.06f);
        if (health <= 0&&EnemyIsAlive)
            EnemyDies();
    }

    private void EnemyDies()
    {
        Instantiate(gm.goldCoin, new Vector3(transform.position.x, 4f, transform.position.z),Quaternion.identity);
        EnemyIsAlive = false;
        agent.isStopped = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        agent.enabled = false;
        if(enemyClass == EnemyClass.giant) FindObjectOfType<AudioManager>().Play("Giant Dies");
        else FindObjectOfType<AudioManager>().Play("Goblin Dies");
        gm.IncreaseSlainEnemies();
        //dead animation
        animator.SetTrigger("die");
        Destroy(this.gameObject, 3f);
        healthBarBG.CrossFadeAlpha(0, .15f, false);
    }
}
