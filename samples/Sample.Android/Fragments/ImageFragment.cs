
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SupportFragment = Android.Support.V4.App.Fragment;

using PullToRefresharp.Android.Views;

namespace Sample.Android.Fragments
{
    public class ImageFragment : SupportFragment
    {
        // The easiest way to interact with your pull to refresh view is to 
        // hold a reference to it that's typed to IPullToRefresharpView.
        private IPullToRefresharpView ptr_view;

        private int drawable_res_id = 0;

        public ImageFragment() : base()
        {
        }

        public ImageFragment(int drawableResId) : base()
        {
            drawable_res_id = drawableResId;
        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            if (savedInstanceState != null) {
                drawable_res_id = savedInstanceState.GetInt("drawable_res_id", 0);
            }
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup parent, Bundle data)
        {
            var view = inflater.Inflate(Resource.Layout.image, parent, false);
            if (drawable_res_id > 0) {
                view.FindViewById<ImageView>(Resource.Id.image_view).SetImageResource(drawable_res_id);
            }

            // Find your pull to refresh view.
            ptr_view = view.FindViewById<ScrollView>(Resource.Id.scrollview) as IPullToRefresharpView;
            if (ptr_view != null) {
                // Hook into the RefreshActivated event so PullToRefresharp can notify you that a refresh is needed.
                ptr_view.RefreshActivated += ptr_view_RefreshActivated;
            }

            return view;
        }

        public override void OnDestroyView()
        {
            // Don't forget to unhook the event handler.
            if (ptr_view != null) {
                ptr_view.RefreshActivated -= ptr_view_RefreshActivated;
            }

            base.OnDestroyView();
        }

        private void ptr_view_RefreshActivated(object sender, EventArgs args)
        {
            // Refresh your content when PullToRefresharp informs you that a refresh is needed
            View.PostDelayed(() => {
                if (ptr_view != null) {
                    // When you are done refreshing your content, let PullToRefresharp know you're done.
                    ptr_view.OnRefreshCompleted();
                }
            }, 2000);
        }

        public override void OnSaveInstanceState (Bundle savedInstanceState)
        {
            base.OnSaveInstanceState (savedInstanceState);
            savedInstanceState.PutInt("drawable_res_id", drawable_res_id);
        }
    }
}

