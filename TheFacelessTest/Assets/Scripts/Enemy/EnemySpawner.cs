using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject spawnArea;

    
    
    // Start is called before the first frame update
    void Start()
    {
        spawnArea.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            spawnArea.SetActive(true);
        }
    }
}
