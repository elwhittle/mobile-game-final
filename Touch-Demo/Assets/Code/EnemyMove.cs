using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private GameObject player;
    private float speed;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = Random.Range(30f, 50f);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < player.transform.position.y)
        {
            rb.velocity = Vector2.up * speed * Time.deltaTime;
            //transform.position += Vector3.up * speed * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector2.down * speed * Time.deltaTime;
            //transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }
}
