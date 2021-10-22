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
        if (Input.GetMouseButton(0)&&!FindObjectOfType<GameManager>().paused)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 100, movementMask))
            {
                animator.MoveToPoint(hit.point,true);
            }
        }
        if (Input.GetMouseButtonUp(0) && !FindObjectOfType<GameManager>().paused)
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
        FindObjectOfType<AudioManager>().Play("Hero Swings");
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
        if(other.tag == "Haste")
        {
            GetComponent<PlayerMatManager>().GoBlue();
            FindObjectOfType<AudioManager>().Play("Hero Hastened");
            animator.Hastened();
            hastenedEffect.Play();
            Destroy(other.gameObject);
        }
        if (other.tag == "Force")
        {
            FindObjectOfType<AudioManager>().Play("Hero Gained Force");
            Destroy(other.gameObject);
        }
        if (other.tag == "Shield")
        {
            GetComponent<PlayerMatManager>().GoYellow();
            FindObjectOfType<GameManager>().Shielded(10);
            Destroy(other.gameObject);
        }
        if (other.tag == "Health")
        {
            FindObjectOfType<AudioManager>().Play("Health Restored");
            FindObjectOfType<GameManager>().CastleHealthDecreases(-20);
            healedEffect.Play();
            Destroy(other.gameObject);
        }
        if (other.tag == "Star")
        {
            GetComponent<PlayerMatManager>().GoCrazy();
            Destroy(other.gameObject);
        }
    }
    //if enemy on range attack also if idle
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //if idle state then enter state attacking then target the enemy
            animator.TriggerAttack();

            //enemy loses hp
            if (attackNearby && other.GetComponent<EnemyBehaviour>().notHit)
            {
                other.GetComponent<EnemyBehaviour>().EnemyTakesDamage(50*FindObjectOfType<GameManager>().heroDamageMultiplier);
                FindObjectOfType<AudioManager>().Play("Hero Slashes");
                other.GetComponent<EnemyBehaviour>().notHit = false;
            }
            if (!other.GetComponent<EnemyBehaviour>().EnemyIsAlive)
                animator.StopAttacking();

            //if has special ability not on cooldown use it
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            animator.StopAttacking();
        }
    }
}
