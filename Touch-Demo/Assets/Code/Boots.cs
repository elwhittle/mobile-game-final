using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots : MonoBehaviour
{
    //private void OnCollisionEnter2D(Collision collision)
    //{
    //    if (collision.collider.gameObject.CompareTag("enemy"))
    //    {
    //        Destroy(collision.collider.gameObject);
    //    }
    //}

    public LayerMask enemyLayer;      // touching enemies?
    public float areaSize = 1.1f;
    //public bool byEnemy = false;

    private float a;

    public void Start()
    {
        a = areaSize / 2f;
    }
    //public bool ByEnemy() { return byEnemy; }

    public bool ByEnemy()
    {
        return Physics2D.OverlapCircle(transform.position, a, enemyLayer);
        //return Physics2D.OverlapArea(new Vector2(transform.position.x - a, transform.position.y + .3f), 
        //    new Vector2(transform.position.x + a, transform.position.y - .3f), enemyLayer);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("enemy"))
    //    {
    //        byEnemy = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("enemy"))
    //    {
    //        byEnemy = false;
    //        print("no longer by enemy");
    //    }
    //}
}
