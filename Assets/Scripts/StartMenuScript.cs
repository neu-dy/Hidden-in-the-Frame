using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    public bool poked = false;
    private PokeInteractable phoneButton;
  

    // Start is called before the first frame update
    void Start()
    {
        phoneButton = GetComponent<PokeInteractable>();

        phoneButton.WhenStateChanged += OnStateChanged;

    }

    // Update is called once per frame
    void Update()
    {
        if (poked)
        {
            SceneManager.LoadScene("Main Scene");
            poked = false;
        }
    }
    public void OnStateChanged(InteractableStateChangeArgs args)
    {
        // Select = the "pressing" event
        if (args.NewState == InteractableState.Select)
        {
            poked = true;
        }
    }
    /*IEnumerator FadeAndLoad()
    {
        // Turn on fade image
        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = elapsed / fadeInDuration;
            fadeImage.color = color;
            yield return null;
        }

        // Fully opaque
        color.a = 1f;
        fadeImage.color = color;

        if (!hasLoadedScene)
        {
            hasLoadedScene = true;
            SceneManager.LoadScene("Main Scene");
        }
    }*/
}
