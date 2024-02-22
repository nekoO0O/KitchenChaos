using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO KitchenObjectSo;

    /// <summary>
    /// 玩家互动
    /// </summary>
    /// <param name="player"></param>
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // 玩家拿的是盘子
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                } // 结果：玩家将物品放入盘子，盘子在玩家手中
                else // 玩家拿的不是盘子，是其他东西
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) // 该柜台上有盘子
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSo()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                } // 结果：玩家将物品放入盘子，盘子在柜台上
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}