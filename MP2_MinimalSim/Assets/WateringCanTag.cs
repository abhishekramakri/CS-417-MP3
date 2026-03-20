using UnityEngine;

/// <summary>
/// Add this to the Watering Can prefab (the one assigned as itemPrefab on the "Watering Can" inventory item).
/// When the can touches a sunflower that has SunflowerClickVR, the sunflower will consume the can and give 2x sunlight per click.
/// Ensure the prefab (or a child) has a Collider set as Trigger so overlap is detected.
/// </summary>
public class WateringCanTag : MonoBehaviour
{
}
