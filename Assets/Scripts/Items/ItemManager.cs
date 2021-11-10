using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemData[] items;

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

    public static ItemData GetItem(CarMovement requestedBy) => _instance.GetItemLocal(requestedBy);

    private ItemData GetItemLocal(CarMovement requestedBy) => this.items[Random.Range(0, this.items.Length)];
}
