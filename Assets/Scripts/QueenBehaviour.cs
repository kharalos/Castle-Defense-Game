using System.Collections;
using UnityEngine;


public class QueenBehaviour : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int ThrowSpawnSpell = Animator.StringToHash("ThrowSpawnSpell");
    private static readonly int CastSpell = Animator.StringToHash("CastSpell");
    private static readonly int ThrowHealSpell = Animator.StringToHash("ThrowHealSpell");
    private static readonly int Interrupt = Animator.StringToHash("Interrupt");
    private static readonly int Defeated = Animator.StringToHash("Defeated");
    private static readonly int F = Animator.StringToHash("Float");

    public enum States { idle, combat, defeated}
    public States states;
    private Vector3 _battlePos, _defeatPos, _targetPos;
    [SerializeField] private Animator anim;
    public GameObject leftSpell, rightSpell, leftSpellPreFab, rightSpellPreFab;
    public ParticleSystem castSpellPS;
    public Vector2 intervalRange = new(1f, 3f);

    private Coroutine _interval;
    private Vector2 _currentInterval;

    private void Start()
    {
        _currentInterval = intervalRange;
        _battlePos = new Vector3(0, 5, 13);
        _defeatPos = new Vector3(0, 0, 13);
        _targetPos = _battlePos;
        _interval = StartCoroutine(IntervalRoutine());
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator IntervalRoutine()
    {
        while (this)
        {
            yield return new WaitForSeconds(Random.Range(_currentInterval.x, _currentInterval.y));
            var number = Random.Range(1, 5);
            switch (number)
            {
                case 1:
                    anim.SetTrigger(ThrowSpawnSpell);
                    break;
                case 2:
                    anim.SetTrigger(CastSpell);
                    break;
                default:
                {
                    if (GameManager.Instance.AliveEnemies.Count > 0)
                        anim.SetTrigger(ThrowHealSpell);
                    break;
                }
            }
        }
    }

    private void Defeat()
    {
        states = States.defeated;
        StopCoroutine(_interval);
        _targetPos = _defeatPos;
        CeaseSpelling();
        anim.SetBool(Defeated, true);
    }

    private IEnumerator MoveCoroutine()
    {
        var originalDistance = (transform.position - _targetPos).magnitude;
        while (true)
        {
            var distance = (transform.position - _targetPos).magnitude;
            MoveToTarget(_targetPos);
            anim.SetFloat(F, Mathf.InverseLerp(0.1f, originalDistance, distance));
            yield return null;

            if(distance < 0.1f)
            {
                yield break;
            }
        }
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);
    }

    private EnemyBehaviour FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        EnemyBehaviour closestEnemy = null;
        foreach (var enemy in GameManager.Instance.AliveEnemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < distanceToClosestEnemy && enemy.enemyIsAlive)
            {
                distanceToClosestEnemy = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }


    private void HealSpellThrow()
    {
        GameObject spellIns = Instantiate(rightSpellPreFab, rightSpell.transform.position, Quaternion.identity);
        spellIns.GetComponent<SpellBehaviour>().target = FindClosestEnemy();
        rightSpell.SetActive(false);
        StartCoroutine(TimerRecast());
    }

    private void SpawnSpellThrow()
    {
        GameObject spellIns = Instantiate(leftSpellPreFab, leftSpell.transform.position, Quaternion.identity);
        leftSpell.SetActive(false);
        StartCoroutine(TimerRecast());
    }

    private void CastSpellStart()
    {
        castSpellPS.Play();
    }

    private void CastSpellEnd()
    {
        castSpellPS.Stop();
    }

    private void CeaseSpelling()
    {
        anim.ResetTrigger(ThrowHealSpell);
        anim.ResetTrigger(ThrowSpawnSpell);
        anim.ResetTrigger(CastSpell);
        anim.SetTrigger(Interrupt);
    }

    private IEnumerator TimerRecast()
    {
        yield return new WaitForSeconds(1);
        leftSpell.SetActive(true);
        rightSpell.SetActive(true);
    }

    public void SetSpeed(float speed)
    {
        _currentInterval = new Vector2(intervalRange.x / speed, intervalRange.y / speed);
        anim.SetFloat(Speed, speed);
    }

    public void SetActive(bool state)
    {
        if(gameObject.activeSelf != state) gameObject.SetActive(state);
    }
}
