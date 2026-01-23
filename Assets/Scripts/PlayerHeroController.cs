using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerHeroController : MonoBehaviour
{
    public LayerMask movementMask;
    [SerializeField] private float jumpHitRadius = 3f;
    [SerializeField] private ParticleSystem healedEffect, hastenedEffect, jumpDropEffect, swingAroundEffect;
    [SerializeField] private SphereCollider attackRangeCollider;
    [SerializeField] private Transform attackSwingEffectPoint;
    [SerializeField] private float attackSwingRotationSpeed = 10f;

    private Camera _cam;
    private PlayerAnimator _animator;

    private bool _attackNearby;
    private bool _isRadialSwinging;
    private float _attackRangeMultiplier = 1f;

    private void Start()
    {
        _cam = Camera.main;
        _animator = GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(GameManager.Instance.paused) return;
        
        if (Input.GetMouseButton(0))
        {
            var ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100, movementMask))
            {
                _animator.MoveToPoint(hit.point,true);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            var ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100, movementMask))
            {
                _animator.MoveToPoint(hit.point, false);
            }
        }

        if (_isRadialSwinging)
        {
            //rotate around the hero, y axis
            attackSwingEffectPoint.Rotate(Vector3.up * attackSwingRotationSpeed * Time.deltaTime);
        }
    }
    
    public void SetAttackRange(float attackRange, float scaleMultiplier)
    {
        attackRangeCollider.radius = attackRange;
        var shape = swingAroundEffect.shape;
        shape.radius = attackRange;
        var position = shape.position;
        position.z = attackRange / 2f;
        shape.position = position;
        _attackRangeMultiplier = scaleMultiplier;
        
        var jumpDropMain = jumpDropEffect.main;
        var x = jumpDropMain.startSizeX;
        x.constant = 4.58f * scaleMultiplier * 2f;
        jumpDropMain.startSizeX = x;
        
        var y = jumpDropMain.startSizeY;
        y.constant = 4.58f * scaleMultiplier * 2f;
        jumpDropMain.startSizeY = y;
    }

    private void HeroSwings()
    {
        _attackNearby = true;
        AudioManager.Instance.Play(ClipType.HeroSwings);
    }

    private void HeroRadialSwing()
    {
        _isRadialSwinging = true;
        swingAroundEffect.Play();
        attackSwingEffectPoint.localEulerAngles = Vector3.zero;
    }

    private void SwingStopped()
    {
        _attackNearby = false;
        _isRadialSwinging = false;
        foreach (var e in FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.InstanceID)) e.notHit = true;
        swingAroundEffect.Stop();
    }

    private readonly RaycastHit[] _jumpDropHits = new RaycastHit[50];

    private void JumpDrop()
    {
        jumpDropEffect.Play();
        AudioManager.Instance.Play(ClipType.JumpDrop);
        GameManager.Instance.HitStop(0.06f);

        //create a spherecast downwards to hit enemies in range
        Physics.SphereCastNonAlloc(transform.position + Vector3.up,
            jumpHitRadius * _attackRangeMultiplier, Vector3.down, _jumpDropHits, jumpHitRadius * _attackRangeMultiplier);
        foreach (var hit in _jumpDropHits)
        {
            if (hit.collider==null || !hit.collider.CompareTag("Enemy")) continue;
            
            hit.collider.GetComponent<EnemyBehaviour>()
                .EnemyTakesDamage(100 * GameManager.Instance.heroDamageMultiplier);
        }
        
        GameManager.Instance.StartJumpCooldown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Haste"))
        {
            GetComponent<PlayerMatManager>().GoBlue();
            AudioManager.Instance.Play(ClipType.HeroHastened);
            _animator.Hastened();
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
            AudioManager.Instance.Play(ClipType.HealthRestored);
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
            _animator.TriggerAttack();

            //enemy loses hp
            if (_attackNearby && other.GetComponent<EnemyBehaviour>().notHit)
            {
                other.GetComponent<EnemyBehaviour>().EnemyTakesDamage(50*GameManager.Instance.heroDamageMultiplier);
                AudioManager.Instance.Play(ClipType.HeroSlashes);
                other.GetComponent<EnemyBehaviour>().notHit = false;
            }
            if (!other.GetComponent<EnemyBehaviour>().enemyIsAlive)
                _animator.StopAttacking();

            //if has special ability not on cooldown use it
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _animator.StopAttacking();
        }
    }
}
