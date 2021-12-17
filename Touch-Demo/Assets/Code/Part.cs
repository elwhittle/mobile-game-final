using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public GameObject[] layouts;
    public Transform end;
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, layouts.Length);
        Instantiate(layouts[index], transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
