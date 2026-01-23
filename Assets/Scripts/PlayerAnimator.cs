using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Jump = Animator.StringToHash("lowJump");
    private static readonly int HighJumpTrigger = Animator.StringToHash("highJump");
    private static readonly int SpeedPercent = Animator.StringToHash("speedPercent");

    private const float LocoAnimSmoothTime = .1f;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody body;
    [SerializeField] private PlayerMatManager matManager;
    private Vector3 _newPoint;
    
    public float targetingRadius;
    
    private void Update()
    {
        var speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(SpeedPercent, speedPercent, LocoAnimSmoothTime, Time.deltaTime);
    }
    
    public void MoveToPoint(Vector3 point, bool holdingTouch)
    {
        var thereIsTarget = false;

        var gameManager = GameManager.Instance;
        foreach (var enemy in gameManager.AliveEnemies)
        {
            var distance = enemy.transform.position - point;
            if (distance.magnitude < targetingRadius)
            {
                _newPoint = enemy.transform.position;
                thereIsTarget = true;
            }
            if (!enemy.enemyIsAlive)
                thereIsTarget = false;
        }

        if (gameManager.enemyTargeting && thereIsTarget && holdingTouch)
            agent.SetDestination(_newPoint);
        else
            agent.SetDestination(point);
    }
    
    public void Hastened()
    {
        agent.speed *= 1.5f;
        animator.speed *= 1.5f;
        StartCoroutine(NormalizedSpeed());
    }
    
    public IEnumerator HeroKnockedBack(Vector3 pos)
    {
        AudioManager.Instance.Play(ClipType.HeroSlashes);
        transform.position += (transform.position - pos).normalized;
        agent.isStopped = true;
        body.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(.2f);
        body.constraints = RigidbodyConstraints.None;
        agent.isStopped = false;
    }

    private IEnumerator NormalizedSpeed()
    {
        yield return new WaitForSeconds(10);
        agent.speed = (agent.speed * 2) / 3;
        animator.speed = (animator.speed * 2) / 3;
        matManager.GoRed();
    }
    
    public void TriggerAttack()
    {
        animator.SetTrigger(Attack);
    }
    
    public void StopAttacking()
    {
        animator.ResetTrigger(Attack);
    }
    
    public void LowJump()
    {
        animator.SetTrigger(Jump);
    }
    
    public void HighJump()
    {
        animator.SetTrigger(HighJumpTrigger);
    }
}
