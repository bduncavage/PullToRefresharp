using System;
using Android.Views;

namespace PullToRefresharp.Android.Views
{
    public interface IPullToRefresharpWrappedView : IPullToRefresharpView
    {
        event EventHandler RefreshCompleted;

        void OnRefreshActivated();
        void SetTopMargin(int margin);
        void ForceHandleTouchEvent(MotionEvent e);

        bool IsAtTop { get; }
        bool IgnoreTouchEvents { get; set; }
    }
}

