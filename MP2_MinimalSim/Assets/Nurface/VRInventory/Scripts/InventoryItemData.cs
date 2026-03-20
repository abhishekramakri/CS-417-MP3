using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace MobileVRInventory
{
    /// <summary>
    /// This represents an inventory item in the database.
    /// </summary>
    [Serializable]
    public class InventoryItemData
    {
        /// <summary>
        /// The name of this inventory item. Should be unique.
        /// </summary>
        [SerializeField]
        public string name;

        /// <summary>
        /// The image to display for this item when it is in the inventory.
        /// </summary>
        [SerializeField]
        public Sprite image;        

        /// <summary>
        /// How many of this item can be in one stack?
        /// (-1 == no limit)
        /// </summary>
        [SerializeField]
        public int stackSize = 1;
        
        /// <summary>
        /// How many stacks of this item should we allow? (If stackSize = 1, how many copies of this item should we allow?)
        /// </summary>
        [SerializeField]
        public int maxNumberOfStacks = 1;

        /// <summary>
        /// If this is set to true, then this item will appear before any other non-pinned items in the inventory.
        /// Use this to display things like currency/etc.
        /// </summary>
        [SerializeField]
        public bool pinItemInInventory = false;
        
        /// <summary>
        /// Can this item be equipped? (Placed in the hand)
        /// </summary>
        [SerializeField]
        public bool equippable = false;

        /// <summary>
        /// The prefab to use for this item in the player's hand.
        /// </summary>
        [SerializeField]
        public Transform itemPrefab;

        /// <summary>
        /// What is the maximum distance away the player can be from this item in order to pick it up?
        /// </summary>
        [SerializeField]
        public float maxPickupDistance = 5f;

        [SerializeField]
        public AudioClip pickupSound = null;

        /// <summary>
        /// This sound will be played when more than one of the item is picked up at once.
        /// (If it is not set, then pickupSound will be used instead)
        /// </summary>
        [SerializeField]
        public AudioClip pickupMultipleSound = null;

        [SerializeField]
        public AudioClip useSound = null;

        public override bool Equals(object obj) {
            if (obj is InventoryItemData) {
                return ((InventoryItemData)obj).name == this.name;
            }

            return false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
