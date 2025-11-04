using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoDetector : MonoBehaviour
{
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
        if (Input.GetKeyDown(photoKey))
        {
            if (!phoneCam || phoneCam.targetTexture == null)
            {
                Debug.LogError("[Detect] Missing camera or RenderTexture!");
                return;
            }

            lastSnapshot = RenderSnapshot(phoneCam);
            if (outputImage) outputImage.texture = lastSnapshot;

            var seen = DetectVisibleTargets();

            if (feedbackText)
            {
                if (seen.Count == 0)
                    feedbackText.text = "Wrong";
                else
                    feedbackText.text = "Got " + string.Join("、", seen);
            }

            Debug.Log("New snapshot capture outputted & detection done.");
        }
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
        var results = new List<string>();
        var planes = GeometryUtility.CalculateFrustumPlanes(phoneCam);

        foreach (var t in targets)
        {
            if (!t || !t.cachedRenderer || !t.cachedCollider) continue;

            bool inFrustum = GeometryUtility.TestPlanesAABB(planes, t.cachedRenderer.bounds);
            if (!inFrustum) continue;

            Vector3 camPos = phoneCam.transform.position;
            Vector3 targetCenter = t.cachedCollider.bounds.center;
            Vector3 dir = targetCenter - camPos;
            float dist = dir.magnitude;

            if (Physics.Raycast(camPos, dir.normalized, out RaycastHit hit, dist, occlusionMask))
            {
                if (hit.collider != t.cachedCollider) continue;
            }

            results.Add($"{t.shape}");
        }

        return results;
    }
}
