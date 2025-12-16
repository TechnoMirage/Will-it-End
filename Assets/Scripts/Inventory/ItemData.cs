using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/NewItem")]

    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        public GameObject prefab;
        public int magMaxSize;
        public int mag;
    }
}
