using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    public EnemyBehaviour target;
    public float destroyDelay = 2f;
    public int archerIndex;
    
    private bool _struck;

    private void Start()
    {
        Destroy(gameObject, 15f);
        _struck = false;
    }
    
    private void FixedUpdate()
    {
        if (target&&!_struck)
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 5f*Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(_struck) return;
        
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyBehaviour>().EnemyTakesDamage(50 * GameManager.Instance.archerDamageMultiplier[archerIndex]);
            AudioManager.Instance.Play(ClipType.ArrowPierces);
            transform.parent = other.transform;
            FreezeArrow();
        }
        else if (other.CompareTag("Land"))
        {
            FreezeArrow();
            transform.position += transform.up * 0.05f;
        }
    }

    private void FreezeArrow()
    {
        _struck = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        AudioManager.Instance.Play(ClipType.ArrowHit);
        Destroy(gameObject, destroyDelay);
    }
}
