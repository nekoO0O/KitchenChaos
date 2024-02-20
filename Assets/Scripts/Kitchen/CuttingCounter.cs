using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO cutKitchenObjectSo;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // 柜台没有KitchenObject
        {
            if (player.HasKitchenObject()) // 玩家拿着KitchenObject
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else // 玩家没有拿着KitchenObject
            {
            }
        }
        else // 柜台有KitchenObject
        {
            if (player.HasKitchenObject())
            {
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject()) // 柜台有KitchenObject
        {
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(cutKitchenObjectSo, this);
        }
    }
}