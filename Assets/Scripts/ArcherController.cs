using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    Animator anim;
    public Transform firePoint;
    public GameObject arrow;
    public GameObject arrow1, arrow2;
    public float health;

    GameObject[] enemies;
    GameObject closestEnemy;
    public float attackRange;
    Vector3 enemyPos;
    Vector3 targetPos;
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (FindObjectOfType<EnemyBehaviour>() && !anim.IsInTransition(0))
        {
            FindClosestEnemy();
        }
        targetPos = new Vector3(enemyPos.x,enemyPos.y+1,enemyPos.z);

        Debug.DrawLine(transform.position, targetPos);

        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
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
                enemyPos = closestEnemy.transform.position;
            }
            if (closestEnemy&&(closestEnemy.transform.position - transform.position).magnitude < attackRange)
            {
                anim.SetTrigger("Fire");
                anim.SetBool("IsIdle", false);
            }
            else
            {
                CeaseFire();
            }
        }
    }
    void CeaseFire()
    {
        anim.ResetTrigger("Fire");
        anim.SetBool("IsIdle",true);
    }
    void TakeArrow()
    {
        arrow1.SetActive(true);
        anim.SetBool("HoldingOne",true);
    }
    void PullArrow()
    {
        arrow1.SetActive(false);
        arrow2.SetActive(true);
    }
    void FireArrow()
    {
        arrow2.SetActive(false);
        anim.SetBool("HoldingOne", false);
        GameObject arrowIns = Instantiate(arrow, firePoint.transform.position, transform.rotation);
        Vector3 arrowPos = arrowIns.transform.position;

        Vector3 projectileXZPos = new Vector3(arrowPos.x, 0.0f, arrowPos.z);
        Vector3 targetXZPos = new Vector3(targetPos.x, 0.0f, targetPos.z);

        // Projectile Motion Formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(0f * Mathf.Deg2Rad);
        float H = targetPos.y - arrowPos.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = arrowIns.transform.TransformDirection(localVelocity);

        arrowIns.transform.LookAt(targetPos);
        arrowIns.GetComponent<Rigidbody>().velocity = globalVelocity;
        arrowIns.GetComponent<ArrowBehaviour>().target = closestEnemy;
        FindObjectOfType<AudioManager>().Play("Arrow Fired");
    }
}
