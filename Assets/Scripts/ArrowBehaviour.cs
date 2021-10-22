using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    bool struck;
    public GameObject target;
    void Start()
    {
        Destroy(gameObject, 15f);
        struck = false;
    }
    private void FixedUpdate()
    {
        if (target&&!struck)
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 5f*Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Land")
        {
            FreezeArrow();
            transform.position += transform.up * 0.05f;
        }
        if (other.tag == "Enemy" && !struck)
        {
            other.GetComponent<EnemyBehaviour>().EnemyTakesDamage(50*FindObjectOfType<GameManager>().archerDamageMultiplier);
            FindObjectOfType<AudioManager>().Play("Arrow Pierces");
            transform.parent = other.transform;
            FreezeArrow();
        }
    }
    void FreezeArrow()
    {
        struck = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        FindObjectOfType<AudioManager>().Play("Arrow Hit");
        Destroy(gameObject, 6f);
    }
}
