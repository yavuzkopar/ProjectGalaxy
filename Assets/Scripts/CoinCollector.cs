using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    CoinPool coinPool;
    [SerializeField] ParticleSystem particle;
    void Start()
    {
        coinPool = FindObjectOfType<CoinPool>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            Debug.Log("CO�N GA�NED");
            particle.transform.position = other.transform.position;
            particle.Play();
            other.transform.position = coinPool.RandomPosition();
        }
    }
}
