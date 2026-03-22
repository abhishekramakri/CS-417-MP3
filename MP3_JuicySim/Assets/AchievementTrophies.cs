using UnityEngine;

/// <summary>
/// Achievement Trophies (2pts):
/// When a resource counter hits a threshold, a trophy object (e.g. a tree) spawns at a set position.
/// Add at least three entries. Checks against sunlight by default.
/// </summary>
public class AchievementTrophies : MonoBehaviour
{
    public enum ResourceType { Sunlight, Money }

    [System.Serializable]
    public class Trophy
    {
        [Tooltip("Resource value that triggers this trophy.")]
        public float threshold;

        [Tooltip("Prefab to spawn (e.g. a tree).")]
        public GameObject prefab;

        [Tooltip("Where to spawn it in the world.")]
        public Vector3 spawnPosition;

        [HideInInspector]
        public bool spawned = false;
    }

    [Tooltip("Which resource to watch.")]
    public ResourceType resourceType = ResourceType.Sunlight;

    public Trophy[] trophies;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip spawnSound;

    void Update()
    {
        if (GameManager.instance == null) return;

        float value = resourceType == ResourceType.Sunlight
            ? GameManager.instance.sunlight
            : GameManager.instance.money;

        foreach (var trophy in trophies)
        {
            if (!trophy.spawned && value >= trophy.threshold)
            {
                Instantiate(trophy.prefab, trophy.spawnPosition, Quaternion.identity);
                if (audioSource != null && spawnSound != null)
                    audioSource.PlayOneShot(spawnSound);
                trophy.spawned = true;
            }
        }
    }
}
