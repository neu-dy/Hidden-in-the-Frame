using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public Image fadeImage;
    public float delayBeforeFadeIn = 1f;  // optional delay
    public float fadeInDuration = 4f;

    private bool hasLoadedScene = false; // For guarding against scene load looping

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Delay before fade in to black starts.
        yield return new WaitForSeconds(delayBeforeFadeIn);

        // Turn on the "Black Screen" Asset
        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;

        // Start fully transparent
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = elapsed / fadeInDuration;   // 0 → 1; 1 is fully opaque
            fadeImage.color = color;
            yield return null;
        }

        if (!hasLoadedScene)
        {
            hasLoadedScene = true;
            SceneManager.LoadScene("AR Ending");
        }

        // Ensure full opacity after fuction completes
        color.a = 1f;
        fadeImage.color = color;
    }
}
