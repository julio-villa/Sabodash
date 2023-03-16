using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawning : MonoBehaviour
{
    // Start is called before the first frame update
    public int probability = 3;
    void Start()
    {
        var random = Random.Range(0, probability);
        if (random != 0) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
