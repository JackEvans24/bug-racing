using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDUpdater : MonoBehaviour
{
    [Header("References")]
    public CarMovement car;
    public CarItemSystem itemSystem;
    [SerializeField] private TMP_Text lapText;
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text itemText;

    private int totalLaps;
    private int currentLap;
    private int currentPosition;
    private ItemData currentItem;

    private void Start()
    {
        this.totalLaps = CheckpointManager.TotalLaps;
    }

    private void Update()
    {
        if (currentLap != car.LapsCompleted + 1)
        {
            this.currentLap = car.LapsCompleted + 1;
            this.lapText.text = $"Lap {this.currentLap}/{this.totalLaps}";
        }

        if (RaceManager.TryGetPosition(this.car, out var position) && position != this.currentPosition)
        {
            this.currentPosition = position;
            this.positionText.text = PositionHelpers.GetPositionText(this.currentPosition);
        }

        if (this.itemSystem.CurrentItems.Count == 0 && this.currentItem != null)
            this.UpdateItem(null);
        else if (this.itemSystem.CurrentItems.Count > 0)
        {
            var nextItem = this.itemSystem.CurrentItems.Peek();
            if (this.currentItem != nextItem)
                this.UpdateItem(nextItem);
        }
    }

    private void UpdateItem(ItemData item)
    {
        this.currentItem = item;
        this.itemText.text = item == null ? string.Empty : item.itemName;
    }
}
