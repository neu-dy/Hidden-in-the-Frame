using UnityEngine;

public class ShapeTarget : MonoBehaviour
{
    public enum ShapeType { WallPhoto, Figure, Painting, other }
    public ShapeType shape = ShapeType.other;

    [HideInInspector] public Renderer cachedRenderer;
    [HideInInspector] public Collider  cachedCollider;

    void Awake()
    {
        cachedRenderer = GetComponentInChildren<Renderer>();
        cachedCollider = GetComponentInChildren<Collider>();
        if (!cachedRenderer) Debug.LogWarning($"{name}: no Renderer found.");
        if (!cachedCollider) Debug.LogWarning($"{name}: no Collider found.");
    }
}

