using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }



    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("已经有一个玩家实例！");    
        }
        Instance = this;
    }



    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    /// <summary>
    /// 事件处理器，处理与柜台交互
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }




    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    /// <summary>
    /// 角色交互逻辑
    /// </summary>
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) 
            {
                // 有ClearCounter
                if (selectedCounter != clearCounter)
                {
                    SetSelectedCounter(clearCounter);
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
    /// 角色移动逻辑
    /// </summary>
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        #region 设置移动
        //不使用物理
        //不能直接使用inputVector,3d的地面是x和z轴
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
            //不能移动（在对角线上发出射线有障碍物）

            //尝试在x轴是否可以移动
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position,
                transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //可以在x轴移动
                moveDir = moveDirX;
            }
            else
            {
                //不能在x轴上移动

                //尝试在x轴是否可以移动
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position,
                    transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //可以在z轴移动
                    moveDir = moveDirZ;
                }
                else
                {
                    //不能在z轴上移动
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
        //transform.eulerAngles和transform.rotation表示GameObject的旋转信息，分别用欧拉角和四元数
        //transform.forward表示GameObject在世界坐标系中的前方方向
        //Slerp 方法更适用于需要在球面上进行插值的情况，例如旋转物体。
        //而 Lerp 方法更适用于需要在直线上进行插值的情况，例如移动物体。
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
    /// 设置柜台的选择
    /// </summary>
    /// <param name="selectedCounter"></param>
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
}