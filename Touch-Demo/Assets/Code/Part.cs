using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public enum PartType
    {
        straight150,
        angledUp75
    }

    public PartType type;
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

    public Vector3 CameraPosition(float x, float offset)
    {
        switch(type)
        {
            case PartType.straight150:
                return new Vector3(x + offset, transform.position.y + 12, -10);
            case PartType.angledUp75:
                return new Vector3(x + offset, (x - transform.position.x) * Mathf.Tan(25 * Mathf.PI / 180f) + transform.position.y + 12, -10);
            default:
                return new Vector3(x + offset, transform.position.y + 12, -10);
        }
    }
}
