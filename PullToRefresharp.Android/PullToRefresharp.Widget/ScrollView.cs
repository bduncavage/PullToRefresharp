using System;
using global::Android.Widget;
using global::Android.Content;
using global::Android.Util;
using global::Android.Views;

using AndroidScrollView = global::Android.Widget.ScrollView;

using PullToRefresharp.Android.Views;
using PullToRefresharp.Android.Delegates;

namespace PullToRefresharp.Android.Widget
{
    public class ScrollView : AndroidScrollView, IPullToRefresharpWrappedView
    {
        private ViewDelegate<ScrollView> ptr_delegate;

        #region Constructors

        public ScrollView(Context context) : this(context, null, 0)
        {
        }

        public ScrollView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public ScrollView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            ptr_delegate = new ViewDelegate<ScrollView>(this);
        }

        #endregion

        #region Touch Handling

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (Parent is ViewWrapper) {
                return (Parent as ViewWrapper).OnTouchEvent(e) || IgnoreTouchEvents || base.OnTouchEvent(e);
            } else {
                return base.OnTouchEvent(e);
            }
        }

        #endregion

        #region IPullToRefresharpWrappedView implementation

        public void ForceHandleTouchEvent(MotionEvent e)
        {
            base.OnTouchEvent(e);
        }

        public event EventHandler RefreshActivated {
            add { ptr_delegate.RefreshActivated += value; }
            remove { ptr_delegate.RefreshActivated -= value; }
        }

        public event EventHandler RefreshCompleted {
            add { ptr_delegate.RefreshCompleted += value; }
            remove { ptr_delegate.RefreshCompleted -= value; }
        }

        public void SetTopMargin(int margin)
        {
            ptr_delegate.SetTopMargin(margin);
        }

        public PullToRefresharpRefreshState RefreshState {
            get {
                return (Parent as ViewWrapper).State;
            }
        }

        public bool PullToRefreshEnabled {
            get {
                return (Parent as ViewWrapper).IsPullEnabled;
            }
            set {
                (Parent as ViewWrapper).IsPullEnabled = value;
            }
        }

        public bool IsAtTop {
            get {
                return ptr_delegate.IsAtTop;
            }
        }

        public bool IgnoreTouchEvents {
            get {
                return ptr_delegate.IgnoreTouchEvents;
            }
            set {
                ptr_delegate.IgnoreTouchEvents = value;
            }
        }

        public void OnRefreshCompleted()
        {
            ptr_delegate.OnRefreshCompleted();
        }

        public void OnRefreshActivated()
        {
            ptr_delegate.OnRefreshActivated();
        }

        #endregion
    }
}

