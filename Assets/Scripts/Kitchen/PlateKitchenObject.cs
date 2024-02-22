using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> KitchenObjectSOList;

    private void Awake()
    {
        KitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // 判断是否可以添加到盘子中
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }

        if (KitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        else
        {
            KitchenObjectSOList.Add(kitchenObjectSO);
            return true;
        }
    }
}