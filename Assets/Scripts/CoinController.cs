using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float destroyDelay = 10f;
    public int coinAmount = 1;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        GameManager.Instance.IncreaseCoinAmount(coinAmount);
        AudioManager.Instance.Play(ClipType.CoinAcquired);
        Destroy(gameObject);
    }
}