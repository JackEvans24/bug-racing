using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemData[] items;
    [Header("Item drop percentages")]
    [SerializeField] private int[] firstPlacePercentages;
    [SerializeField] private int[] firstHalfPercentages;
    [SerializeField] private int[] rearHalfPercentages;

    private static ItemManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public static ItemData[] GetItems(CarMovement requestedBy) => _instance.GetItemLocal(requestedBy);

    private ItemData[] GetItemLocal(CarMovement requestedBy)
    {
        var percentages = this.rearHalfPercentages;
        if (RaceManager.TryGetPosition(requestedBy, out var position))
        {
            if (position == 1)
                percentages = this.firstPlacePercentages;
            else if (position <= GameController.TotalRacers / 2)
                percentages = this.firstHalfPercentages;
        }

        var result = Random.Range(0f, 1f) * 100;
        var total = 0;

        for (int i = 0; i < percentages.Length; i++)
        {
            total += percentages[i];
            if (result <= total)
                return new[] { this.items[i] };
        }

        int maxValue = percentages.Max();
        int maxIndex = percentages.ToList().IndexOf(maxValue);
        if (maxIndex < 0)
            maxIndex = 0;

        return new[] { this.items[maxIndex] };
    }
}
