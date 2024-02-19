using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public event EventHandler OnInteractAction;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    /// <summary>
    /// 通过输入获取移动向量，并标准化
    /// </summary>
    /// <returns>返回移动向量</returns>
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // 将输入的向量标准化（同时按下W和D，会输出（1，1），我希望角色的速度不会变，应为（0.71，0.71））
        inputVector = inputVector.normalized;

        return inputVector;
    }

    /// <summary>
    /// 玩家交互事件
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
}