using System;
using Android.Content;
using Android.Util;
using Android.Graphics;

namespace PullToRefresharp.Android.Views
{
    public class RotatingPullDownIcon : RotatableImageView, IPullToRefresharpPullDownIcon
    {
        #region Constructors

        public RotatingPullDownIcon(Context context) : this(context, null, 0)
        {
        }

        public RotatingPullDownIcon(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public RotatingPullDownIcon(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        #endregion

        #region IPullToRefresharpPullDownIcon implementation

        public void SetProgress(float progress)
        {
            if (RotationPivotPoint == null) {
                RotationPivotPoint = new Point(MeasuredWidth / 2, MeasuredHeight / 2);
            }

            RotationDegress = progress * 180.0f;
        }

        #endregion
    }
}

