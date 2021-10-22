using System.Collections;
using System.Collections.Generic;
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
    Vector3 randomPos;
    void Start()
    {
        Destroy(gameObject, 10f);
        randomPos = new Vector3(Random.Range(-8, 8), -1, Random.Range(5, 15));
    }
    private void FixedUpdate()
    {
        if (target)
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 5f * Time.deltaTime);
        else transform.position = Vector3.Lerp(transform.position, randomPos, 5f * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Land")
        {
            Disperse();
        }
        if (other.tag == "Enemy" && spellClass == SpellClass.healSpell && other.GetComponent<EnemyBehaviour>().EnemyIsAlive)
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
            Instantiate(FindObjectOfType<GameManager>().enemies[Random.Range(0, FindObjectOfType<GameManager>().enemies.Length)], transform.position, transform.rotation);
        }
        if(spellClass == SpellClass.healSpell)
        { 
            //HealVFX
        }

        Destroy(gameObject);
    }
}
