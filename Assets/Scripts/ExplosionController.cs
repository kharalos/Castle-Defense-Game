using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    ParticleSystem ps;
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    public void Explode(Vector3 pos)
    {
        ps.Stop();
        transform.position = pos;
        ps.Play();
    }
}
