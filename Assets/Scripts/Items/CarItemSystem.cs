using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class CarItemSystem : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform dropPoint;

    [Header("DO NOT TOUCH")]
    public Stack<ItemData> CurrentItems = new Stack<ItemData>();
    
    private CarMovement movement;
    private bool usingItem;

    private void Awake()
    {
        this.movement = GetComponent<CarMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.CurrentItems.Count > 0 || this.usingItem)
            return;

        if (other.CompareTag(Tags.ITEM_BOX))
        {
            foreach (var item in ItemManager.GetItems(this.movement))
                this.CurrentItems.Push(item);
        }
    }

    public void UseCurrentItem()
    {
        if (this.CurrentItems.Count <= 0 || this.usingItem || this.movement.IsStunned)
            return;

        this.usingItem = true;

        // Only focus on drop for now
        var currentItem = this.CurrentItems.Pop();
        switch (currentItem.Type)
        {
            case ItemType.Self:
                break;

            case ItemType.Drop:
                this.DropItem(currentItem);
                break;

            case ItemType.Shoot:
                this.ShootItem(currentItem);
                break;

            case ItemType.NextPlayer:
                break;

            case ItemType.FirstPlayer:
                break;

            default:
                throw new System.NotImplementedException();
        }

        this.usingItem = false;
    }

    private void DropItem(ItemData currentItem)
    {
        Instantiate(currentItem.Prefab, this.dropPoint.position, this.dropPoint.rotation);
    }

    private void ShootItem(ItemData currentItem)
    {
        Instantiate(currentItem.Prefab, this.firePoint.position, this.firePoint.rotation);
    }
}
