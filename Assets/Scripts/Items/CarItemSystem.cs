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
                this.CreateItem(currentItem, this.transform);
                break;

            case ItemType.Drop:
                this.CreateItem(currentItem, this.dropPoint);
                break;

            case ItemType.Shoot:
                this.CreateItem(currentItem, this.firePoint);
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

    private void CreateItem(ItemData currentItem, Transform createPoint)
    {
        var itemObj = Instantiate(currentItem.Prefab, createPoint.position, createPoint.rotation);
        
        var item = itemObj.GetComponent<Item>();
        if (item != null)
            item.UsedBy = this.movement;
    }
}
