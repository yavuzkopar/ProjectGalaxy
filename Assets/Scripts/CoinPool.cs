using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int coinCount;
    [SerializeField] int aiCount;
    [SerializeField] Transform[] planet;
    [SerializeField] GameObject Ai;
    void Start()
    {
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 position = RandomPosition();
            
                
             while (!IsInPlanet(position))
            {
                position = RandomPosition();
            }
            Instantiate(coinPrefab, position, Quaternion.identity);
        }
        int aa = coinCount / 5;
        for (int i = 0; i < aa; i++)
        {
            Instantiate(coinPrefab, GetRandomPlanetPoint(), Quaternion.identity);
        }
        for (int i = 0; i < aiCount; i++)
        {
            Instantiate(Ai, GetRandomPlanetPoint(), Quaternion.identity);
        }
    }
    bool IsInPlanet(Vector3 pos)
    {
        foreach (var item in planet)
        {
            float desiredDistance = item.localScale.x /2;
            if (Vector3.Distance(pos, item.position) < desiredDistance)
                return false;
        }
        return true;
    }
    Vector3 GetRandomPlanetPoint()
    {
        Transform p = planet[Random.Range(0,planet.Length)];
        Vector3 position = p.position + Random.onUnitSphere * p.localScale.x/2;
        return position;
    }
    public Vector3 RandomPosition()
    {
        Vector3 position = Random.insideUnitSphere;
        position = new Vector3(position.x * transform.localScale.x/2, position.y * transform.localScale.y/2, position.z * transform.localScale.z / 2);
        position += transform.position;
        return position;
    }

}
