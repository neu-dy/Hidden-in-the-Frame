using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TakePhoto : MonoBehaviour
{
    // KeyCode used until function can be transferred to VR touch element
    private KeyCode photoKey = KeyCode.Space;
    private Camera phoneCam;
    public RawImage outputImage;
    private Texture2D outputTexture;

    // Start is called before the first frame update
    void Start()
    {
        // Identify the Phone Camera used for photo output
        phoneCam = GameObject.Find("PhoneCameraOutput").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(photoKey))
        {
            outputTexture = RenderSnapshot(phoneCam);
            ShowSnapshot(outputTexture);
            Debug.Log("New snapshot capture outputted.");
        }
    }

    // Takes a screenshot of a camera's current Render Texture
    public Texture2D RenderSnapshot(Camera camera)
    {
        
        // Set the current Render Texture to the active camera texture
        // RenderTexture.active is read by ReadPixels
        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view
        camera.Render();

        // Make a new texture and set the snapshot of the active Render Texture into it
        Texture2D snapshotTexture = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        snapshotTexture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        snapshotTexture.Apply();

        // Replace the original active Render Texture
        RenderTexture.active = currentRenderTexture;

        return snapshotTexture;
    }

    void ShowSnapshot(Texture2D snapshot)
    {
        if (outputImage != null)
        {
            outputImage.texture = snapshot;
        }
    }
    public void TakePhotoButton()
    {
        outputTexture = RenderSnapshot(phoneCam);
        ShowSnapshot(outputTexture);
        Debug.Log("New snapshot capture outputted (via button).");
    }
    //void OnMouseDown()
    //{
    //    TakePhotoButton();
    //}
}