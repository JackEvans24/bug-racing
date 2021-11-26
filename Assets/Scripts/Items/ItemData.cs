using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public string Name;
    public ItemType Type;
    public GameObject Prefab;
    public Sprite Sprite;

    public CarMovement GetTarget(CarMovement activatedBy)
    {
        switch (this.Type)
        {
            case ItemType.Self:
                return activatedBy;
            case ItemType.NextPlayer:
                return this.GetNextPlayer(activatedBy);
            case ItemType.FirstPlayer:
                return GetFirstPlayer();
            default:
                throw new System.InvalidOperationException("Shouldn't be trying to get target for this item type");
        }
    }

    private CarMovement GetNextPlayer(CarMovement relativeTo)
    {
        int position;
        CarMovement result;

        if (!RaceManager.TryGetPosition(relativeTo, out position))
            return null;
        if (!RaceManager.TryGetRacer(position, out result))
            return null;

        return result;
    }

    private CarMovement GetFirstPlayer()
    {
        RaceManager.TryGetRacer(1, out var result);
        return result;
    }
}
