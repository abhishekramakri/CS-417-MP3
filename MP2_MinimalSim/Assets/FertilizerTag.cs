using UnityEngine;

/// <summary>
/// Add this to the Fertilizer prefab (the one assigned as itemPrefab on the "Fertilizer" inventory item).
/// When the fertilizer touches a rose (GreenFlowerClickVR), the rose will consume the fertilizer and give 2x coins per click.
/// Ensure the prefab (or a child) has a Collider set as Trigger so overlap is detected.
/// </summary>
public class FertilizerTag : MonoBehaviour
{
}
