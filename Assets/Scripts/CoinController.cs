using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            FindObjectOfType<GameManager>().IncreaseCoinAmount();
            FindObjectOfType<AudioManager>().Play("Coin Acquired");
            Destroy(gameObject);
        }
    }
}
