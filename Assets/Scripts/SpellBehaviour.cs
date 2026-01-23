using UnityEngine;

public enum SpellClass
{
    spawnSpell,
    healSpell
}

public class SpellBehaviour : MonoBehaviour
{
    public SpellClass spellClass;
    public GameObject target;
    
    private Vector3 _randomPos;
    void Start()
    {
        Destroy(gameObject, 10f);
        _randomPos = new Vector3(Random.Range(-8, 8), -1, Random.Range(5, 15));
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target ? target.transform.position : _randomPos, 5f * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Land"))
        {
            Disperse();
        }
        else if (other.CompareTag("Enemy") && spellClass == SpellClass.healSpell && other.GetComponent<EnemyBehaviour>().enemyIsAlive)
        {
            other.GetComponent<EnemyBehaviour>().health += 100;
            Disperse();
        }
    }
    void Disperse()
    {
        if(spellClass == SpellClass.spawnSpell)
        {
            //SpawnVFX
            Instantiate(GameManager.Instance.enemies[Random.Range(0, GameManager.Instance.enemies.Length)], transform.position, transform.rotation);
        }
        if(spellClass == SpellClass.healSpell)
        { 
            //HealVFX
        }

        Destroy(gameObject);
    }
}
