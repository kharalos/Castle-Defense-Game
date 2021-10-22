using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QueenBehaviour : MonoBehaviour
{
    public enum States { idle, combat, defeated}
    public States states;
    Vector3 battlePos, defeatPos, oldPos, targetPos;
    Rigidbody body;
    Animator anim;
    public GameObject leftSpell, rightSpell, leftSpellPreFab, rightSpellPreFab;
    public ParticleSystem castSpellPS;
    GameObject[] enemies;
    GameObject closestEnemy;
    public float intervalTime;
    Coroutine interval;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        body = gameObject.GetComponent<Rigidbody>();
        battlePos = new Vector3(0, 5, 13);
        defeatPos = new Vector3(0, 0, 13);
        targetPos = battlePos;
        interval = StartCoroutine(IntervalRoutine());
    }
    void Update()
    {
        FindClosestEnemy();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float velocity = (transform.position - oldPos).magnitude;
        MoveToTarget(targetPos);
        anim.SetFloat("Speed", velocity, .2f, Time.deltaTime);
        oldPos = transform.position;
    }
    IEnumerator IntervalRoutine()
    {
        yield return new WaitForSeconds(Random.Range(intervalTime-1,intervalTime+1));
        int number = Random.Range(1, 5);
        if (number == 1)
            anim.SetTrigger("ThrowSpawnSpell");
        else if (number == 2)
            anim.SetTrigger("CastSpell");
        else if (GameObject.FindGameObjectWithTag("Enemy"))
            anim.SetTrigger("ThrowHealSpell");
        StartCoroutine(IntervalRoutine());
    }
    void Defeat()
    {
        states = States.defeated;
        StopCoroutine(interval);
        targetPos = defeatPos;
        CeaseSpelling();
        anim.SetBool("Defeated", true);
    }
    void MoveToTarget(Vector3 targetPosition)
    {
        body.MovePosition(body.position + ((targetPosition - body.position) * 2 * Time.deltaTime));
    }
    void FindClosestEnemy()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float distanceToClosestEnemy = Mathf.Infinity;
        closestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < distanceToClosestEnemy && enemy.GetComponent<EnemyBehaviour>().EnemyIsAlive)
            {
                distanceToClosestEnemy = distance;
                closestEnemy = enemy;
            }
        }
    }


    void HealSpellThrow()
    {
        GameObject spellIns = Instantiate(rightSpellPreFab, rightSpell.transform.position, Quaternion.identity);
        spellIns.GetComponent<SpellBehaviour>().target = closestEnemy;
        rightSpell.SetActive(false);
        StartCoroutine(TimerRecast());
    }
    void SpawnSpellThrow()
    {
        GameObject spellIns = Instantiate(leftSpellPreFab, leftSpell.transform.position, Quaternion.identity);
        leftSpell.SetActive(false);
        StartCoroutine(TimerRecast());
    }
    void CastSpellStart()
    {
        castSpellPS.Play();
    }
    void CastSpellEnd()
    {
        castSpellPS.Stop();
    }
    void CeaseSpelling()
    {
        anim.ResetTrigger("ThrowHealSpell");
        anim.ResetTrigger("ThrowSpawnSpell");
        anim.ResetTrigger("CastSpell");
        anim.SetTrigger("Interrupt");
    }
    IEnumerator TimerRecast()
    {
        yield return new WaitForSeconds(1);
        leftSpell.SetActive(true);
        rightSpell.SetActive(true);
    }
}
