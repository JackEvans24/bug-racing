using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDUpdater : MonoBehaviour
{
    [Header("References")]
    public CarMovement car;
    [SerializeField] private TMP_Text lapText;
    [SerializeField] private TMP_Text positionText;

    private int totalLaps;
    private int currentLap;
    private int currentPosition;

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

            var positionOrdinal = this.GetPositionOrdinal(this.currentPosition);
            this.positionText.text = $"{this.currentPosition}{positionOrdinal}";
        }
    }

    private string GetPositionOrdinal(int position)
    {
        switch (position)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }
}
