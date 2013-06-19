using System;
using Android.Views;

namespace PullToRefresharp.Android.Delegates
{
    public class ViewDelegate<T> where T : View
    {
        protected T view;

        public ViewDelegate(T view)
        {
            this.view = view;
        }

        public event EventHandler RefreshActivated;
        public event EventHandler RefreshCompleted;

        public virtual void SetTopMargin(int margin)
        {
            ((ViewGroup.MarginLayoutParams)view.LayoutParameters).TopMargin = margin;
        }

        public virtual bool IsAtTop {
            get {
                return view.ScrollY == 0;
            }
        }

        private bool ignore_touch_events;
        public bool IgnoreTouchEvents {
            get {
                return ignore_touch_events;
            }
            set {
                ignore_touch_events = value;
            }
        }

        public void OnRefreshCompleted()
        {
            var h = RefreshCompleted;
            if (h != null) {
                h(this, EventArgs.Empty);
            }
        }

        public void OnRefreshActivated()
        {
            var h = RefreshActivated;
            if (h != null) {
                h(this, EventArgs.Empty);
            }
        }
    }
}

