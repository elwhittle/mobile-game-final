using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    public Image SnotBackground;
    public Image SnotMeter;
    public Text distanceDisplay;
    public float scale = 0.5f;
    public Slider slider;

    private GameObject player;
    private int distance = 0;
    private float startingX;
    private float xDist = 0;
    private float maxDist = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startingX = player.transform.position.x;
        slider.maxValue = 1f;
        slider.value = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        /* display distance */
        xDist = player.transform.position.x - startingX;
        maxDist = Mathf.Max(xDist, maxDist);
        if (maxDist - xDist <= 0.00001f)
        {
            distanceDisplay.text = ((int)(maxDist*scale)).ToString();
        }

        //SetSnot(0.5f);
    }

    /*
     * Sets snot to the given percentage
     */
    void SetSnot(float percent)
    {
        slider.value = percent;
    }

}
