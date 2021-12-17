using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < player.transform.position.y)
        {
            transform.position += Vector3.up * 10f * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * 10f * Time.deltaTime;
        }
    }
}
