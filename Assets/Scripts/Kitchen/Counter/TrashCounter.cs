using System;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed; // 当有物体丢弃时触发

    public new static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();

            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}