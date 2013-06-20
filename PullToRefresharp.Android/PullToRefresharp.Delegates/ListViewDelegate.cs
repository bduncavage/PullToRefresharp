using System;
using Android.Views;
using Android.Widget;
using Android.OS;

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
                var is_child_at_top = view.FirstVisiblePosition == 0 && (child == null || child.Top == 0);

                if (child != null && !is_child_at_top && view.FirstVisiblePosition == 0) {
                    // If you build with a sane Android SDK version this is not necessary.
                    // But older Android versions count the View's ListPaddingTop as part of Top.
                    // we have to take into account padding/margin

                    // make sure we offset for the padding.
                    is_child_at_top = child.Top - view.ListPaddingTop == 0;
                }
                return is_child_at_top;
            }
        }
    }
}

