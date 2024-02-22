using System;
using System.Collections;
using System.Collections.Generic;

public interface IHasProgress
{
    // UI事件
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
}