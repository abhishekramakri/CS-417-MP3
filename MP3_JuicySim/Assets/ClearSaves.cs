#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ClearSaves
{
    [MenuItem("Tools/Clear Saved Data")]
    static void Clear()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Saved data cleared.");
    }
}
#endif
