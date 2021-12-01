using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowcase : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private ItemData[] items;

    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image itemImage;

    private ItemData selectedItem;
    private int selectedItemIndex;

    private void Start()
    {
        this.UpdateSelectedItem();
    }

    public void NextItem()
    {
        this.selectedItemIndex++;
        if (this.selectedItemIndex >= this.items.Length)
            this.selectedItemIndex = 0;

        this.UpdateSelectedItem();
    }

    public void PreviousItem()
    {
        this.selectedItemIndex--;
        if (this.selectedItemIndex < 0)
            this.selectedItemIndex = this.items.Length - 1;

        this.UpdateSelectedItem();
    }

    private void UpdateSelectedItem()
    {
        this.selectedItem = this.items[this.selectedItemIndex];

        this.itemImage.sprite = this.selectedItem.Sprite;
        this.nameText.text = this.selectedItem.Name;
        this.descriptionText.text = this.selectedItem.Description;
    }
}
