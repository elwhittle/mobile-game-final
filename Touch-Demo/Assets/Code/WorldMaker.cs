using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{

    public GameObject[] parts;
    private Transform nextSpawn;

    private GameObject mostRecentlySpawned;

    // Start is called before the first frame update
    void Start()
    {
        mostRecentlySpawned = Instantiate(parts[0], GameObject.FindGameObjectWithTag("start").transform);
        nextSpawn = mostRecentlySpawned.GetComponent<Part>().end;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > nextSpawn.position.x)
        {
            mostRecentlySpawned = Instantiate(parts[0], nextSpawn, false);
            nextSpawn = mostRecentlySpawned.GetComponent<Part>().end;
        }
    }
}
