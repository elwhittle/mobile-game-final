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

    public bool byEnemy = false;

    //public bool ByEnemy() { return byEnemy; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            byEnemy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            byEnemy = false;
            print("no longer by enemy");
        }
    }
}
