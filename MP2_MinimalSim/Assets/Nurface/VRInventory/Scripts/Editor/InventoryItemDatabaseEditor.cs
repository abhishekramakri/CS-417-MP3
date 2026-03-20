using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MobileVRInventory
{
    [CustomEditor(typeof(InventoryItemDatabase))]
    public class InventoryItemDatabaseEditor : Editor
    {
        SerializedProperty m_ItemsProperty;
        SerializedProperty m_OutlineConfigurationProperty;

        void OnEnable()
        {
            m_ItemsProperty = serializedObject.FindProperty("items");
            m_OutlineConfigurationProperty = serializedObject.FindProperty("outlineConfiguration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var itemDatabase = (InventoryItemDatabase)target;
            if (itemDatabase.items == null) itemDatabase.items = new List<InventoryItemData>();
            if (itemDatabase.outlineConfiguration == null) itemDatabase.outlineConfiguration = new ItemOutlineConfiguration();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_OutlineConfigurationProperty, true);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorGUI.BeginChangeCheck(); // Track changes for entire items section and Add New

            EditorGUILayout.Space(8f);

            if (m_ItemsProperty == null) { serializedObject.ApplyModifiedProperties(); return; }

            List<int> indicesToDelete = new List<int>();

            for (int i = 0; i < m_ItemsProperty.arraySize; i++)
            {
                SerializedProperty itemProp = m_ItemsProperty.GetArrayElementAtIndex(i);
                InventoryItemData item = itemDatabase.items.Count > i ? itemDatabase.items[i] : null;
                if (item == null) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.Space(4f);
                EditorGUILayout.LabelField("Item Configuration", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                {
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(64), GUILayout.Height(64));

                    rect.position = new Vector2(rect.position.x + 8, rect.position.y);
                    rect.width = 42;
                    rect.height = 42;                    

                    if (item.image != null)
                    {
                        EditorGUI.DrawPreviewTexture(rect, item.image.texture);
                    }

                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Name", GUILayout.Width(175));                            
                            item.name = EditorGUILayout.TextField(item.name);
                            // strip out any quotation marks - we're using them as a special character
                            // (You can still use single quotes however)
                            var validatedName = item.name.Replace("\"", "");

                            var validationFailed = validatedName != item.name;

                            item.name = validatedName;

                            if (validationFailed)
                            {
                                EditorUtility.DisplayDialog("Warning", @"Inventory item names may not contain quotation marks '""'. Single quotes are fine.", "Okay");                                
                                EditorGUI.FocusTextInControl(null);                                
                            }

                            if (GUILayout.Button(new GUIContent("X", "Remove this item"), GUILayout.Width(32)))
                            {
                                indicesToDelete.Add(i);
                            }                            
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Image", GUILayout.Width(175));
                            var originalImage = item.image;
                            item.image = (Sprite)EditorGUILayout.ObjectField(item.image, typeof(Sprite), false);

                            if (originalImage == null && item.image != null)
                            {
                                if (string.IsNullOrEmpty(item.name)) item.name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.image.name);
                            }

                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Equippable", "Can this item be equipped?"), GUILayout.Width(175));
                            item.equippable = EditorGUILayout.Toggle(item.equippable);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        if (item.equippable)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Prefab", "The model used to represent this item in the player's hand."), GUILayout.Width(175));
                                item.itemPrefab = (Transform)EditorGUILayout.ObjectField(item.itemPrefab, typeof(Transform), false);
                                EditorGUILayout.LabelField("", GUILayout.Width(32));
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Stacking", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Stack size", "The maximum quantity of this item to allow in one stack. -1 == no limit."), GUILayout.Width(175));
                            item.stackSize = EditorGUILayout.IntField(item.stackSize);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Max number of stacks", "The maximum number of stacks to allow in the inventory. -1 == no limit."), GUILayout.Width(175));
                            item.maxNumberOfStacks = EditorGUILayout.IntField(item.maxNumberOfStacks);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Pin in inventory", "If this is set, then this item will appear before any other non-pinned items in the inventory."), GUILayout.Width(175));
                            item.pinItemInInventory = EditorGUILayout.Toggle(item.pinItemInInventory);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Pickup Sound", "Audio clip to play when this item is picked up"), GUILayout.Width(175));
                            item.pickupSound = (AudioClip)EditorGUILayout.ObjectField(item.pickupSound, typeof(AudioClip), false);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Pickup Sound (Multiple items)", "Audio clip to play when multiple items of this type are picked up"), GUILayout.Width(175));
                            item.pickupMultipleSound = (AudioClip)EditorGUILayout.ObjectField(item.pickupMultipleSound, typeof(AudioClip), false);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Use Sound", "Audio clip to play when this item is used or selected in the inventory"), GUILayout.Width(175));
                            item.useSound = (AudioClip)EditorGUILayout.ObjectField(item.useSound, typeof(AudioClip), false);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Miscellaneous", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Max Pickup Distance", "This specifies the maximum distance the player can be from the item in order to pick it up."), GUILayout.Width(175));
                            item.maxPickupDistance = EditorGUILayout.FloatField(item.maxPickupDistance);
                            EditorGUILayout.LabelField("", GUILayout.Width(32));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(4f);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(6f);
            }

            for (int d = indicesToDelete.Count - 1; d >= 0; d--)
            {
                m_ItemsProperty.DeleteArrayElementAtIndex(indicesToDelete[d]);
            }
            if (indicesToDelete.Count > 0) serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (GUILayout.Button("Add New", GUILayout.Width(200)))
            {
                m_ItemsProperty.InsertArrayElementAtIndex(m_ItemsProperty.arraySize);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(target);
                Repaint();
            }

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(itemDatabase);
                AssetDatabase.SaveAssets();

                Object.FindObjectsByType<VRInventory>(FindObjectsSortMode.None)
                          .Where(vr => vr.itemDatabase == itemDatabase)
                          .ToList()
                          .ForEach(vr => vr.ItemsUpdated());
            }
        }
    }
}
