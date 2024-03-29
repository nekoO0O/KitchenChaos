using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    // 控制UI显示的事件
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    // 处理声音的事件
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;


    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList; // 等待食谱列表
    private int waitingRecipesMax = 4;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;

    private int successfulRecipesAmount; // 成功完成的食谱数量

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        // 增加等待食谱列表
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// 检查提交的食谱是否是指定的食谱
    /// </summary>
    /// <param name="plateKitchenObject"></param>
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) // 遍历等待食谱列表
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count ==
                plateKitchenObject.GetKitchenObjectSOList().Count) // 当盘子和食谱的原料数量一致
            {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) // 遍历食谱中的原料
                {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in
                             plateKitchenObject.GetKitchenObjectSOList()) // 遍历盘子中的原料
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound) // 盘子中没有找到该原料
                    {
                        plateContentsMatchesRecipe = false;
                        break;
                    }
                }

                if (plateContentsMatchesRecipe) // 找到了匹配的订单，送餐成功
                {
                    waitingRecipeSOList.RemoveAt(i);
                    successfulRecipesAmount++;

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // 走到这里说明没有找到食谱
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        Debug.Log("没有找到食谱");
    }

    /// <summary>
    /// 获得等待食谱列表
    /// </summary>
    /// <returns></returns>
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    /// <summary>
    /// 获取成功完成的食谱数量
    /// </summary>
    /// <returns></returns>
    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}