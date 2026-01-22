using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerHeroController : MonoBehaviour
{
    public LayerMask movementMask;
    [SerializeField]
    private SphereCollider range;
    Camera cam;
    PlayerAnimator animator;
    bool hasForce;
    public ParticleSystem healedEffect, hastenedEffect;

    bool attackNearby;
    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)&&!GameManager.Instance.paused)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 100, movementMask))
            {
                animator.MoveToPoint(hit.point,true);
            }
        }
        if (Input.GetMouseButtonUp(0) && !GameManager.Instance.paused)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 100, movementMask))
            {
                animator.MoveToPoint(hit.point, false);
            }
        }
    }
    void HeroSwings()
    {
        attackNearby = true;
        AudioManager.Instance.Play("Hero Swings");
    }
    void SwingStopped()
    {
        attackNearby = false;
        foreach (EnemyBehaviour e in FindObjectsOfType<EnemyBehaviour>())
        {
            e.notHit = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Haste"))
        {
            GetComponent<PlayerMatManager>().GoBlue();
            AudioManager.Instance.Play("Hero Hastened");
            animator.Hastened();
            hastenedEffect.Play();
            Destroy(other.gameObject);
        }
        /*if (other.CompareTag("Force"))
        {
            AudioManager.Instance.Play("Hero Gained Force");
            Destroy(other.gameObject);
        }*/
        if (other.CompareTag("Shield"))
        {
            GetComponent<PlayerMatManager>().GoYellow();
            GameManager.Instance.Shielded(10);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Health"))
        {
            AudioManager.Instance.Play("Health Restored");
            GameManager.Instance.CastleHealthDecreases(-20);
            healedEffect.Play();
            Destroy(other.gameObject);
        }
        /*if (other.CompareTag("Star"))
        {
            GetComponent<PlayerMatManager>().GoCrazy();
            Destroy(other.gameObject);
        }*/
    }
    //if enemy on range attack also if idle
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //if idle state then enter state attacking then target the enemy
            animator.TriggerAttack();

            //enemy loses hp
            if (attackNearby && other.GetComponent<EnemyBehaviour>().notHit)
            {
                other.GetComponent<EnemyBehaviour>().EnemyTakesDamage(50*GameManager.Instance.heroDamageMultiplier);
                AudioManager.Instance.Play("Hero Slashes");
                other.GetComponent<EnemyBehaviour>().notHit = false;
            }
            if (!other.GetComponent<EnemyBehaviour>().enemyIsAlive)
                animator.StopAttacking();

            //if has special ability not on cooldown use it
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            animator.StopAttacking();
        }
    }
}
