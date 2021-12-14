using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Goal nextGoal;
    public Color doneColor;

    public Vector2 NextGoal()
    {
        GetComponent<SpriteRenderer>().color = doneColor;
        PublicVars.currentGoal = this;
        nextGoal.gameObject.SetActive(true);
        return gameObject.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
