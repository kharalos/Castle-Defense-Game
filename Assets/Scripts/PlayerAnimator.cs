using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Jump = Animator.StringToHash("lowJump");
    private static readonly int HighJumpTrigger = Animator.StringToHash("highJump");
    private static readonly int SpeedPercent = Animator.StringToHash("speedPercent");

    private const float LocoAnimSmoothTime = .1f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Rigidbody _body;
    private Vector3 _newPoint;
    
    public float targetingRadius;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        float speedPercent = _agent.velocity.magnitude / _agent.speed;
        _animator.SetFloat(SpeedPercent, speedPercent, LocoAnimSmoothTime, Time.deltaTime);
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
                _newPoint = enemy.transform.position;
                thereIsTarget = true;
            }
            if (!enemy.GetComponent<EnemyBehaviour>().enemyIsAlive)
                thereIsTarget = false;
        }
        if (FindFirstObjectByType<GameManager>().enemyTargeting && thereIsTarget && holdingTouch)
        {
            _agent.SetDestination(_newPoint);
        }
        else
            _agent.SetDestination(point);
    }
    public void Hastened()
    {
        _agent.speed *= 1.5f;
        _animator.speed *= 1.5f;
        StartCoroutine(NormalizedSpeed());
    }
    public IEnumerator HeroKnockedback(Vector3 pos)
    {
        AudioManager.Instance.Play(ClipType.HeroSlashes);
        transform.position += (transform.position - pos).normalized;
        float savedSpeed = _animator.speed;
        _agent.isStopped = true;
        _body.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(.2f);
        _body.constraints = RigidbodyConstraints.None;
        _agent.isStopped = false;
    }

    private IEnumerator NormalizedSpeed()
    {
        yield return new WaitForSeconds(10);
        _agent.speed = (_agent.speed * 2) / 3;
        _animator.speed = (_animator.speed * 2) / 3;
        GetComponent<PlayerMatManager>().GoRed();
    }
    
    public void TriggerAttack()
    {
        _animator.SetTrigger(Attack);
    }
    
    public void StopAttacking()
    {
        _animator.ResetTrigger(Attack);
    }
    
    public void LowJump()
    {
        _animator.SetTrigger(Jump);
    }
    
    public void HighJump()
    {
        _animator.SetTrigger(HighJumpTrigger);
    }
}
