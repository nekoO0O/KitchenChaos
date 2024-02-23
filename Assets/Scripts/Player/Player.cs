using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    // 玩家输入
    [SerializeField] private GameInputManager gameInputManager;

    // 玩家移动
    [SerializeField] private float moveSpeed = 7f;
    private bool isWalking;

    // 玩家与柜台交互
    [SerializeField] private LayerMask countersLayerMask;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    // 玩家与物品交互
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("已经有一个玩家实例！");
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gameInputManager.OnInteractAction += GameInputManager_OnInteractAction;
        gameInputManager.OnInteractAlternateAction += GameInputManager_OnInteractAlternateAction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    /// <summary>
    /// 角色移动逻辑
    /// </summary>
    private void HandleMovement()
    {
        Vector2 inputVector = gameInputManager.GetMovementVectorNormalized();

        #region 设置移动

        // 不使用物理，不能直接使用 inputVector，3d的地面是x和z轴
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        #region 通过射线检测碰撞

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position,
            transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        #endregion

        #region 使角色在对角线上移动时碰到障碍物可以沿着障碍走

        if (!canMove)
        {
            // 不能移动（在对角线上发出射线有障碍物），尝试在x轴是否可以移动
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position,
                transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // 可以在x轴移动
                moveDir = moveDirX;
            }
            else
            {
                // 不能在x轴上移动，尝试在z轴是否可以移动
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position,
                    transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // 可以在z轴移动
                    moveDir = moveDirZ;
                }
                else
                {
                    // 不能在z轴上移动
                }
            }
        }

        #endregion

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        #endregion

        #region 设置判断是否是移动的布尔值

        isWalking = moveDir != Vector3.zero;

        #endregion

        #region 设置角色旋转

        // transform.eulerAngles 和 transform.rotation 表示 GameObject 的旋转信息，分别用欧拉角和四元数
        // transform.forward 表示 GameObject 在世界坐标系中的前方方向
        // Slerp 方法更适用于需要在球面上进行插值的情况，例如旋转物体。
        // 而 Lerp 方法更适用于需要在直线上进行插值的情况，例如移动物体。
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        #endregion
    }

    /// <summary>
    /// 设置动画播放是Idel或Walk
    /// </summary>
    /// <returns>判断是否是移动的布尔值</returns>
    public bool IsWalking()
    {
        return isWalking;
    }

    /// <summary>
    /// 角色交互逻辑
    /// </summary>
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInputManager.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance,
                countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // 有ClearCounter
                if (selectedCounter != baseCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    /// <summary>
    /// 事件处理器，处理与柜台交互（逻辑）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GameInputManager_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    /// <summary>
    /// 设置柜台的选择（视觉）
    /// </summary>
    /// <param name="selectedCounter"></param>
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    /// <summary>
    /// 事件处理器，处理与切割柜台切割动作交互（逻辑）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GameInputManager_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}