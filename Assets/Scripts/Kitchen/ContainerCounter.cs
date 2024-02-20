using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO KitchenObjectSo;
    public event EventHandler OnPlayerGrabbedObject;// 玩家交互触发事件

    /// <summary>
    /// 玩家互动
    /// </summary>
    /// <param name="player"></param>
    public override void Interact(Player player)
    {
        Transform kitchenObjectTransform = Instantiate(KitchenObjectSo.prefab, GetKitchenObjectFollowTransform());
        kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}