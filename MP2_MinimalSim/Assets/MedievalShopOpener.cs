using UnityEngine;

/// <summary>
/// Attach to the medieval shop object (the 3D building/object). When the user clicks it, the shop UI Canvas opens.
/// The shop object itself stays visible; only the UI Canvas is shown/hidden.
/// In Inspector: assign the GameObject that has the ShopPopup component (your shop UI Canvas or panel).
/// Wire this script's OnShopClicked() to your click/select event:
/// - XR Simple Interactable: add to Select Entered or Activate → drag this GameObject → MedievalShopOpener.OnShopClicked
/// - EventTrigger: add Pointer Down (or Click) → drag this GameObject → MedievalShopOpener.OnShopClicked
/// </summary>
public class MedievalShopOpener : MonoBehaviour
{
    [Tooltip("The shop UI popup (GameObject with ShopPopup). Assign in Inspector.")]
    public ShopPopup shopPopup;

    /// <summary>Call from XR Simple Interactable (Select Entered / Activate) or EventTrigger (Pointer Down).</summary>
    public void OnShopClicked()
    {
        if (shopPopup != null)
            shopPopup.ShowPopup();
        else
            Debug.LogWarning("[MedievalShopOpener] No ShopPopup assigned. Drag the shop popup GameObject into the Shop Popup field.");
    }
}
