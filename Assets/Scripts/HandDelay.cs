using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDelay : MonoBehaviour
{
    public float delayTime = 6f; // Type seconds until animation starts
    public GameObject handL;
    public GameObject handR;
    public GameObject ghostAsset;
    
    // Start is called before the first frame update
    void Start()
    {
        handL.SetActive(false);
        handR.SetActive(false);
        ghostAsset.SetActive(false);
        StartCoroutine(StartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartAnimation()
    {
        // Delay time before animation
        yield return new WaitForSeconds(delayTime);
        handL.SetActive(true);
        handR.SetActive(true);
        ghostAsset.SetActive(true);
    }
}
