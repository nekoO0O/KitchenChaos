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
            Debug.LogError("�Ѿ���һ�����ʵ����");    
        }
        Instance = this;
    }



    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    /// <summary>
    /// �¼����������������̨����
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
    /// ��ɫ�����߼�
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
                // ��ClearCounter
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
    /// ��ɫ�ƶ��߼�
    /// </summary>
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        #region �����ƶ�
        //��ʹ������
        //����ֱ��ʹ��inputVector,3d�ĵ�����x��z��
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        #region ͨ�����߼����ײ
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position,
            transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        #endregion

        #region ʹ��ɫ�ڶԽ������ƶ�ʱ�����ϰ�����������ϰ���
        if (!canMove)
        {
            //�����ƶ����ڶԽ����Ϸ����������ϰ��

            //������x���Ƿ�����ƶ�
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position,
                transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //������x���ƶ�
                moveDir = moveDirX;
            }
            else
            {
                //������x�����ƶ�

                //������x���Ƿ�����ƶ�
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position,
                    transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //������z���ƶ�
                    moveDir = moveDirZ;
                }
                else
                {
                    //������z�����ƶ�
                }
            }
        }
        #endregion

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        #endregion

        #region �����ж��Ƿ����ƶ��Ĳ���ֵ
        isWalking = moveDir != Vector3.zero;
        #endregion

        #region ���ý�ɫ��ת
        //transform.eulerAngles��transform.rotation��ʾGameObject����ת��Ϣ���ֱ���ŷ���Ǻ���Ԫ��
        //transform.forward��ʾGameObject����������ϵ�е�ǰ������
        //Slerp ��������������Ҫ�������Ͻ��в�ֵ�������������ת���塣
        //�� Lerp ��������������Ҫ��ֱ���Ͻ��в�ֵ������������ƶ����塣
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        #endregion
    }




    /// <summary>
    /// ���ö���������Idel��Walk
    /// </summary>
    /// <returns>�ж��Ƿ����ƶ��Ĳ���ֵ</returns>
    public bool IsWalking()
    {
        return isWalking;
    }

    /// <summary>
    /// ���ù�̨��ѡ��
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