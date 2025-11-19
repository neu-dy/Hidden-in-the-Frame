using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeFromBlack : MonoBehaviour
{
    public Image fadeImage;
    public float delayBeforeFadeOut = 7f; // before fade starts
    public float fadeOutDuration = 10f; // how long fade takes

    //private bool hasLoadedScene = false; // For guarding against scene load looping

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeOut()
    {
        // Delay process: default duration of game. Currently 120 second game duration.
        yield return new WaitForSeconds(delayBeforeFadeOut);

        float elapsed = 0f;
        Color color = fadeImage.color;

        // Only load scene if it's not active
        /* if (!hasLoadedScene)
        {
            hasLoadedScene = true;
            SceneManager.LoadScene("AR Ending");
        }
        */

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - (elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Testing transparency
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}
