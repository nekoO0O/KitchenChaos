using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSo;
    public event EventHandler OnPlayerGrabbedObject;// 玩家交互触发事件

    /// <summary>
    /// 玩家互动
    /// </summary>
    /// <param name="player"></param>
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())// 玩家没有持有KitchenObject
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
            
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);// 打开箱子动画事件
        }
    }
}