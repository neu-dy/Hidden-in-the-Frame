using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeFromBlack : MonoBehaviour
{
    public Image fadeImage;
    public float delayBeforeFade = 3f; // before fade starts
    public float fadeDuration = 2f; // how long fade takes
    
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
        // Delay process
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - (elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Testing transparency
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}
