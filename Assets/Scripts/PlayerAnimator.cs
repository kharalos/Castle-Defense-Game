using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    const float locoAnimSmoothTime = .1f;

    NavMeshAgent agent;
    Animator animator;
    Rigidbody body;
    Vector3 newPoint;
    public float targetingRadius;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, locoAnimSmoothTime, Time.deltaTime);
    }
    public void MoveToPoint(Vector3 point, bool holdingTouch)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool thereIsTarget = false;

        foreach (GameObject enemy in enemies)
        {
            Vector3 distance = enemy.transform.position - point;
            if (distance.magnitude < targetingRadius)
            {
                newPoint = enemy.transform.position;
                thereIsTarget = true;
            }
            if (!enemy.GetComponent<EnemyBehaviour>().EnemyIsAlive)
                thereIsTarget = false;
        }
        if (FindObjectOfType<GameManager>().enemyTargeting && thereIsTarget && holdingTouch)
        {
            agent.SetDestination(newPoint);
        }
        else
            agent.SetDestination(point);
    }
    public void Hastened()
    {
        agent.speed = agent.speed * 1.5f;
        animator.speed = animator.speed * 1.5f;
        StartCoroutine(NormalizedSpeed());
    }
    public IEnumerator HeroKnockedback(Vector3 pos)
    {
        FindObjectOfType<AudioManager>().Play("Hero Slashes");
        transform.position += (transform.position - pos).normalized;
        float savedSpeed = animator.speed;
        animator.speed = 0;
        agent.isStopped = true;
        body.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(.2f);
        body.constraints = RigidbodyConstraints.None;
        agent.isStopped = false;
        animator.speed = savedSpeed;
    }
    IEnumerator NormalizedSpeed()
    {
        yield return new WaitForSeconds(10);
        agent.speed = (agent.speed * 2) / 3;
        animator.speed = (animator.speed * 2) / 3;
        GetComponent<PlayerMatManager>().GoRed();
    }
    public void TriggerAttack()
    {
        animator.SetTrigger("attack");
    }
    public void StopAttacking()
    {
        animator.ResetTrigger("attack");
    }
    public void LowJump()
    {
        animator.SetTrigger("lowJump");
    }
    public void HighJump()
    {
        animator.SetTrigger("highJump");
    }
}
