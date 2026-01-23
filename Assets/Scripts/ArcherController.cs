using UnityEngine;

public class ArcherController : MonoBehaviour
{
    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int HoldingOne = Animator.StringToHash("HoldingOne");
    private static readonly int Speed = Animator.StringToHash("Speed");

    [SerializeField] private Animator anim;
    public Transform firePoint;
    public GameObject arrow;
    public GameObject arrow1, arrow2;
    public int archerIndex;

    private EnemyBehaviour _closestEnemy;
    public float attackRange;
    private Vector3 _enemyPos;
    private Vector3 _targetPos;
    
    private void FixedUpdate()
    {
        if (GameManager.Instance.AliveEnemies.Count > 0 && !anim.IsInTransition(0))
        {
            FindClosestEnemy();
        }
        _targetPos = new Vector3(_enemyPos.x,_enemyPos.y+1,_enemyPos.z);

        var direction = (_targetPos - transform.position).normalized;
        var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
    }

    public void SetSpeed(float speed)
    {
        anim.SetFloat(Speed, speed);
    }

    private void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        _closestEnemy = null;
        foreach (var enemy in GameManager.Instance.AliveEnemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < distanceToClosestEnemy && enemy.enemyIsAlive)
            {
                distanceToClosestEnemy = distance;
                _closestEnemy = enemy;
                _enemyPos = _closestEnemy.transform.position;
            }
            if (_closestEnemy&&(_closestEnemy.transform.position - transform.position).magnitude < attackRange)
            {
                anim.SetTrigger(Fire);
                anim.SetBool(IsIdle, false);
            }
            else
            {
                CeaseFire();
            }
        }
    }

    private void CeaseFire()
    {
        anim.ResetTrigger(Fire);
        anim.SetBool(IsIdle, true);
    }

    private void TakeArrow()
    {
        arrow1.SetActive(true);
        anim.SetBool(HoldingOne, true);
    }

    private void PullArrow()
    {
        arrow1.SetActive(false);
        arrow2.SetActive(true);
    }

    private void FireArrow()
    {
        arrow2.SetActive(false);
        anim.SetBool(HoldingOne, false);
        var arrowIns = Instantiate(arrow, firePoint.transform.position, transform.rotation);
        var arrowPos = arrowIns.transform.position;

        var projectileXZPos = new Vector3(arrowPos.x, 0.0f, arrowPos.z);
        var targetXZPos = new Vector3(_targetPos.x, 0.0f, _targetPos.z);

        // Projectile Motion Formula
        var r = Vector3.Distance(projectileXZPos, targetXZPos);
        var g = Physics.gravity.y;
        var tanAlpha = Mathf.Tan(0f * Mathf.Deg2Rad);
        var h = _targetPos.y - arrowPos.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        var vz = Mathf.Sqrt(g * r * r / (2.0f * (h - r * tanAlpha)));
        var vy = tanAlpha * vz;

        // create the velocity vector in local space and get it in global space
        var localVelocity = new Vector3(0f, vy, vz);
        var globalVelocity = arrowIns.transform.TransformDirection(localVelocity);

        arrowIns.transform.LookAt(_targetPos);
        arrowIns.GetComponent<Rigidbody>().linearVelocity = globalVelocity;
        var arrowBehaviour = arrowIns.GetComponent<ArrowBehaviour>();   
        arrowBehaviour.target = _closestEnemy;
        arrowBehaviour.archerIndex = archerIndex;
        AudioManager.Instance.Play(ClipType.ArrowFired);
    }
}
