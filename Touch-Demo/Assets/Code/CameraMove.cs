using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private float offset;
    private GameObject player;
    public Part part;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position.x - player.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PublicVars.part.CameraPosition(player.transform.position.x, offset);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("part"))
    //    {
    //        part = collision.gameObject.GetComponent<Part>();
    //    }
    //}
}
