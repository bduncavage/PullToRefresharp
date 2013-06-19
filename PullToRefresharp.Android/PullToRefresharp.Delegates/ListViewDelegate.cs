using System;
using Android.Views;
using Android.Widget;

using PullToRefresharp.Android.Views;

namespace PullToRefresharp.Android.Delegates
{
    public class ListViewDelegate : ViewDelegate<AbsListView>
    {
        public ListViewDelegate(AbsListView view) : base (view)
        {
        }

        public override bool IsAtTop {
            get {
                var child = view.GetChildAt(0);
                return view.FirstVisiblePosition == 0 && (child == null || child.Top == 0);
            }
        }
    }
}

