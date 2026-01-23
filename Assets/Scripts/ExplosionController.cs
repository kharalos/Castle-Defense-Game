using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private ParticleSystem _ps;

    private void Start()
    {
        _ps = gameObject.GetComponent<ParticleSystem>();
    }

    public void Explode(Vector3 pos)
    {
        _ps.Stop();
        transform.position = pos;
        _ps.Play();
    }
}
