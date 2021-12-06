using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject pauseButtonCanvas;

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
        //StartCoroutine(StartTime(.2f));
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseCanvas.SetActive(true);
    }

    public void Play()
    {
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
    }

    public void ToggleGravity()
    {
        PublicVars.useGravity = !PublicVars.useGravity;
    }

    private IEnumerator StartTime(float time)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 1;
    }
}
