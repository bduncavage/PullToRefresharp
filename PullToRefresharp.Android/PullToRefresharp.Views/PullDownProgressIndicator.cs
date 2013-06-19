using System;
using Android.Widget;
using Android.Content;
using Android.Util;

namespace PullToRefresharp.Android.Views
{
    public class PullDownProgressIndicator : ViewSwitcher, IPullDownProgressIndicator
    {
        private IPullToRefresharpPullDownIcon icon;

        #region Constructors

        public PullDownProgressIndicator(Context context) : base(context)
        {
        }

        public PullDownProgressIndicator(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        #endregion

        #region IPullDownProgressIndicator implementation

        public void SetProgress(float progress)
        {
            icon = icon ?? (IPullToRefresharpPullDownIcon)GetChildAt(0);
            if (icon != null) {
                icon.SetProgress(progress);
            }
        }

        public void SetRefreshState(PullToRefresharpRefreshState state)
        {
            DisplayedChild = state == PullToRefresharpRefreshState.Refreshing ? 1 : 0;
        }

        #endregion
    }
}

