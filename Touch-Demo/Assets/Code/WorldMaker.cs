using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{

    public GameObject[] parts;
    private Transform nextSpawn;

    public GameObject mostRecentlySpawned;

    // Start is called before the first frame update
    void Start()
    {
        //mostRecentlySpawned = Instantiate(parts[0], GameObject.FindGameObjectWithTag("start").transform);
        Part p = mostRecentlySpawned.GetComponent<Part>();
        PublicVars.part = p;
        nextSpawn = p.end;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > nextSpawn.position.x)
        {
            mostRecentlySpawned = Instantiate(parts[Random.Range(0, parts.Length)], nextSpawn, false);
            PublicVars.nextPart = mostRecentlySpawned.GetComponent<Part>();
            PublicVars.nextPartSet = true;
            nextSpawn = mostRecentlySpawned.GetComponent<Part>().end;
        }
    }
}
