using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class CarItemSystem : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform dropPoint;

    [Header("DO NOT TOUCH")]
    public ItemData currentItem;
    
    private CarMovement movement;
    private bool usingItem;

    private void Awake()
    {
        this.movement = GetComponent<CarMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.currentItem != null || this.usingItem)
            return;

        if (other.CompareTag(Tags.ITEM_BOX))
            this.currentItem = ItemManager.GetItem(this.movement);
    }

    public void UseCurrentItem()
    {
        if (this.currentItem == null || this.usingItem)
            return;

        this.usingItem = true;

        // Only focus on drop for now
        switch (this.currentItem.itemType)
        {
            case ItemType.Self:
                break;

            case ItemType.Drop:
                DropItem();
                break;

            case ItemType.Shoot:
                break;

            case ItemType.NextPlayer:
                break;

            case ItemType.FirstPlayer:
                break;

            default:
                throw new System.NotImplementedException();
        }

        this.currentItem = null;
        this.usingItem = false;
    }

    private void DropItem()
    {
        var item = Instantiate(this.currentItem.itemObj, this.dropPoint.position, this.dropPoint.rotation);
    }
}
