using UnityEngine;

public class ArcherController : MonoBehaviour
{
    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int HoldingOne = Animator.StringToHash("HoldingOne");
    private static readonly int Speed = Animator.StringToHash("Speed");

    private Animator _anim;
    public Transform firePoint;
    public GameObject arrow;
    public GameObject arrow1, arrow2;
    public int archerIndex;

    private GameObject[] _enemies;
    private GameObject _closestEnemy;
    public float attackRange;
    private Vector3 _enemyPos;
    private Vector3 _targetPos;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (FindFirstObjectByType<EnemyBehaviour>() && !_anim.IsInTransition(0))
        {
            FindClosestEnemy();
        }
        _targetPos = new Vector3(_enemyPos.x,_enemyPos.y+1,_enemyPos.z);

        Debug.DrawLine(transform.position, _targetPos);

        Vector3 direction = (_targetPos - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        
        _anim.SetFloat(Speed, GameManager.Instance.archerSpeedMultiplier[archerIndex]);
    }

    private void FindClosestEnemy()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float distanceToClosestEnemy = Mathf.Infinity;
        _closestEnemy = null;
        foreach (GameObject enemy in _enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < distanceToClosestEnemy && enemy.GetComponent<EnemyBehaviour>().enemyIsAlive)
            {
                distanceToClosestEnemy = distance;
                _closestEnemy = enemy;
                _enemyPos = _closestEnemy.transform.position;
            }
            if (_closestEnemy&&(_closestEnemy.transform.position - transform.position).magnitude < attackRange)
            {
                _anim.SetTrigger(Fire);
                _anim.SetBool(IsIdle, false);
            }
            else
            {
                CeaseFire();
            }
        }
    }

    private void CeaseFire()
    {
        _anim.ResetTrigger(Fire);
        _anim.SetBool(IsIdle, true);
    }

    private void TakeArrow()
    {
        arrow1.SetActive(true);
        _anim.SetBool(HoldingOne, true);
    }

    private void PullArrow()
    {
        arrow1.SetActive(false);
        arrow2.SetActive(true);
    }

    private void FireArrow()
    {
        arrow2.SetActive(false);
        _anim.SetBool(HoldingOne, false);
        GameObject arrowIns = Instantiate(arrow, firePoint.transform.position, transform.rotation);
        Vector3 arrowPos = arrowIns.transform.position;

        Vector3 projectileXZPos = new Vector3(arrowPos.x, 0.0f, arrowPos.z);
        Vector3 targetXZPos = new Vector3(_targetPos.x, 0.0f, _targetPos.z);

        // Projectile Motion Formula
        float r = Vector3.Distance(projectileXZPos, targetXZPos);
        float g = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(0f * Mathf.Deg2Rad);
        float h = _targetPos.y - arrowPos.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float vz = Mathf.Sqrt(g * r * r / (2.0f * (h - r * tanAlpha)));
        float vy = tanAlpha * vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, vy, vz);
        Vector3 globalVelocity = arrowIns.transform.TransformDirection(localVelocity);

        arrowIns.transform.LookAt(_targetPos);
        arrowIns.GetComponent<Rigidbody>().linearVelocity = globalVelocity;
        var arrowBehaviour = arrowIns.GetComponent<ArrowBehaviour>();   
        arrowBehaviour.target = _closestEnemy;
        arrowBehaviour.archerIndex = archerIndex;
        AudioManager.Instance.Play(ClipType.ArrowFired);
    }
}
