using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Oculus.Interaction;
using static UnityEngine.CullingGroup;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhotoDetector : MonoBehaviour
{
    public bool poked;

    [Header("Trigger")]
    [SerializeField] private KeyCode photoKey = KeyCode.Space;

    [Header("Source Camera")]
    [SerializeField] private Camera phoneCam;
    [SerializeField] private LayerMask occlusionMask = ~0;

    [Header("Output UI")]
    [SerializeField] private RawImage outputImage;
    [SerializeField] private TMP_Text feedbackText;

    private Texture2D lastSnapshot;
    private List<ShapeTarget> targets = new List<ShapeTarget>();

    [Header("Required Targets")]
    private List<string> requiredShapes = new List<string>() { "WallPhoto", "Figure", "Painting" };

    private HashSet<string> collected =  new HashSet<string>();
    [SerializeField] private string nextSceneName = "AR Ending";

    public AudioSource phoneCallSound;
    public AudioSource phoneRingtone;
    public AudioSource transitionSound;

    public GameObject radioSound; // For disabling radio when all 3 photo conditions met

    public Image fadeImage; // For controlling fade to black for win condition
    public float fadeInDuration = 3f;

    private bool hasLoadedScene = false; // For guarding against scene load looping

    public GameObject wallPaintingItem;
    public GameObject canvasItem;
    public GameObject figureBeforeItem;
    public GameObject figureAfterItem;

    public GameObject phoneCallScreen;

    public GameObject phoneLight;

    void Awake()
    {
        if (!phoneCam)
        {
            var go = GameObject.Find("PhoneCameraOutput");
            if (go) phoneCam = go.GetComponent<Camera>();
        }
        targets.AddRange(FindObjectsOfType<ShapeTarget>());
    }


    void Update()
    {
        if (poked)
        //if (Input.GetKeyDown(photoKey))
        {
            poked = false; //unpoke

            if (!phoneCam || phoneCam.targetTexture == null)
            {
                Debug.LogError("[Detect] Missing camera or RenderTexture!");
                return;
            }

            lastSnapshot = RenderSnapshot(phoneCam);
            if (outputImage) outputImage.texture = lastSnapshot;

            var seen = DetectVisibleTargets();

            foreach (var s in seen) //Add the recognization target
            {
                collected.Add(s); // If object detected, will add to collection
            }
            //if WallPhoto in seen:
            // Enable Handprint
            //if Canvas in seen:
            // Enable open eye Canvas
            // if Figure in seen:
            // Disable pos 1 + enable pos 2
            if (seen.Contains("WallPhoto"))
            {
                // DoWallPhotoAction();   // ← enable handprint
                wallPaintingItem.SetActive(true);
                //play sound here
                transitionSound.Play();
            }

            if (seen.Contains("Painting"))
            {
                // DoPaintingAction();    // ← enable open eye canvas
                canvasItem.SetActive(true);
                //play sound here
                transitionSound.Play();
            }

            if (seen.Contains("Figure"))
            {
                // DoFigureAction();      // ← disable pos1 / enable pos2
                figureBeforeItem.SetActive(false);
                figureAfterItem.SetActive(true);
                //play sound here
                transitionSound.Play();
            }
            bool allFound = true;
            foreach (var req in requiredShapes)
            {
                if (!collected.Contains(req))
                {
                    allFound = false;
                    break;
                }
            }

            if (allFound && !string. IsNullOrEmpty(nextSceneName))
            {
                Debug.Log("Already Dected all target and Changing Scene");

                radioSound.SetActive(false); // Disable radio sound when all targets are found
                phoneLight.SetActive(false); // Turn off phone light

                StartCoroutine(StartPhoneRingtone());

                // Commenting out scene change for Phone Call
                // SceneManager.LoadScene(nextSceneName);
            }

            if (feedbackText)
            {
                if (seen.Count == 0)
                    feedbackText.text = "Wrong";
                else
                    feedbackText.text = "Got " + string.Join(" & ", seen);
            }

            Debug.Log("New snapshot capture outputted & detection done.");
        }
                
    }

    // Function that waits for all transition sounds to end before phone rings
    IEnumerator StartPhoneRingtone()
    {
        yield return new WaitForSeconds(7.0f);
        phoneRingtone.Play(); //play ringtone
        phoneCallScreen.SetActive(true); //switch screen to phone call 

        StartCoroutine(PlayGhostPhoneCall());
    }

    // Function that waits for phone ringtone to finish before phone call starts
    IEnumerator PlayGhostPhoneCall()
    {
        yield return new WaitForSeconds(8.0f);

        // Play the ghost phone call audio
        phoneCallSound.Play();
        StartCoroutine(SceneTransition());
    }

    // Function that waits for phone call to finish before scene transition
    IEnumerator SceneTransition()
    {
        yield return new WaitForSeconds(7.0f);

        //Fade to black code
        // Turn on the "Black Screen" Asset
        fadeImage.gameObject.SetActive(true);

        //play sound here

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

        //SceneManager.LoadScene(nextSceneName);
    }

    Texture2D RenderSnapshot(Camera cam)
    {
        var prev = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();

        Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        tex.Apply();

        RenderTexture.active = prev;
        return tex;
    }

    List<string> DetectVisibleTargets()
    {
        // This list will store all the objects we successfully detect.
        var results = new List<string>();

        // Go/Loop through every object in the scene that has a ShapeTarget script.
        foreach (var t in targets)
        {
            // Safety check:
            // If the object doesn't exist, or doesn't have a renderer
            // or collider for us to test against, skip it.
            if (!t || !t.cachedRenderer || !t.cachedCollider)
                continue;

            // Check if the ENTIRE object is inside the camera frame
            // and not blocked by anything. If not, skip it.
            if (!IsTargetFullyVisible(t))
                continue;

            // If we reach this line:
            // The object IS fully inside the picture AND not occluded.
            // Add its label (WallPhoto, Cube, etc.) to the results list.
            results.Add($"{t.shape}");
        }
        // Return all objects that were successfully detected in the snapshot.
        return results;
    }
    bool IsTargetFullyVisible(ShapeTarget t)
    {
        
        // Determine dimensions for object x y z
        // This is the invisible box that wraps around the whole mesh.
        Bounds bounds = t.cachedRenderer.bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        // 8 corners of the renderer bounds
        // We will check each of these to make sure the entire box is in view.
        Vector3[] corners =
        {
        new Vector3(min.x, min.y, min.z),
        new Vector3(min.x, min.y, max.z),
        new Vector3(min.x, max.y, min.z),
        new Vector3(min.x, max.y, max.z),
        new Vector3(max.x, min.y, min.z),
        new Vector3(max.x, min.y, max.z),
        new Vector3(max.x, max.y, min.z),
        new Vector3(max.x, max.y, max.z),
    };

        // 1) Check that ALL corners are inside the camera's view
        // If even one corner is outside, the object is not gonna show fully visible.
        foreach (var corner in corners)
        {
            Vector3 vp = phoneCam.WorldToViewportPoint(corner);

            // Behind camera?
            if (vp.z <= 0f)
                return false;

            // Outside screen?
            if (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f)
                return false;
        }

        // If we reach this point:
        // All corners are inside the camera view → object is fully on-screen.
        // So now: 2) Occlusion check: make sure nothing is blocking it
        Vector3 camPos = phoneCam.transform.position;

        // We’ll check the center + a few corners
        List<Vector3> samplePoints = new List<Vector3>();
        samplePoints.Add(t.cachedCollider.bounds.center);
        samplePoints.AddRange(corners);

        foreach (var point in samplePoints)
        {
            Vector3 dir = point - camPos; // Direction from camera to the point
            float dist = dir.magnitude; // Distance to the point

            if (dist <= 0.001f) // Ignore insanely small distances (rare edge case)
                continue;

            if (Physics.Raycast(camPos, dir.normalized, out RaycastHit hit, dist, occlusionMask))
            {
                // Something else is in front of this point → not fully visible
                if (hit.collider != t.cachedCollider)
                    return false;
            }
        }

        // If we reach this line:
        // - All corners are inside the camera view (1st condition)
        // - None of them are blocked by other objects (2nd condition)
        return true;
    }
}
