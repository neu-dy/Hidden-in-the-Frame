using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRest : MonoBehaviour
{
    public float waitTime = 25.0f;
    private KeyCode photoKey = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerForEnding());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(photoKey))
        {
            SceneManager.LoadScene("Main Scene");
            Debug.Log("Reset to menu.");
        }
    }

    IEnumerator TimerForEnding()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("Start Menu");
    }
}
