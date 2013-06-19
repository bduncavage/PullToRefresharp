
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
using Android.Views.Animations;

namespace Sample.Android.Fragments
{
    public class GridFragment : SupportFragment
    {
        private GridView grid_view;
        // LOOK HERE!
        // The easiest way to interact with your pull to refresh view is to 
        // hold a reference to it that's typed to IPullToRefresharpView.
        private IPullToRefresharpView ptr_grid_view;

        private GridAdapter adapter;

        private List<string> data_list = MainActivity.FLAVORS.ConvertAll<string>(x => (string)x.Clone());

        public GridFragment() : base()
        {
        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            // fill up the list with a bunch of junk
            while (data_list.Count () < 100) {
                data_list.AddRange(data_list);
            }
            base.OnCreate (savedInstanceState);
            adapter = new GridAdapter(this, Activity, Resource.Layout.grid_item, data_list);
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup parent, Bundle data)
        {
            var view = inflater.Inflate(Resource.Layout.grid, null, false);
            grid_view = view.FindViewById<GridView>(Resource.Id.gridview);
            if (grid_view is IPullToRefresharpView) {
                ptr_grid_view = (IPullToRefresharpView)grid_view;
            }

            grid_view.Adapter = adapter;
            grid_view.ItemClick += grid_view_ItemClick;

            // LOOK HERE!
            // Hookup a handler to the RefreshActivated event
            ptr_grid_view.RefreshActivated += grid_view_RefreshActivated;

            return view;
        }

        public override void OnDestroyView()
        {
            grid_view.ItemClick -= grid_view_ItemClick;
            grid_view = null;
            ptr_grid_view = null;

            base.OnDestroyView();
        }

        private void grid_view_RefreshActivated(object sender, EventArgs args)
        {
            // LOOK HERE!
            // Refresh your content when PullToRefresharp informs you that a refresh is needed
            grid_view.PostDelayed(() => {
                // When you are done refreshing your content, let PullToRefresharp know you're done.
                ptr_grid_view.OnRefreshCompleted();
            }, 2000);
        }

        private void grid_view_ItemClick(object sender, AdapterView.ItemClickEventArgs args)
        {
            Toast.MakeText(Activity, args.Position + " Clicked", ToastLength.Short).Show();
        }

        #region Adapter

        private class GridAdapter : ArrayAdapter
        {
            private GridFragment fragment;

            public override int Count {
                get {
                    return fragment.data_list.Count();
                }
            }
            public GridAdapter(GridFragment fragment, Context context, int textViewResId, List<string> objects) : base(context, textViewResId, objects)
            {
                this.fragment = fragment;
            }

            public override Java.Lang.Object GetItem (int position)
            {
                if (position == fragment.data_list.Count() - 1) {
                    fragment.data_list.AddRange(fragment.data_list);
                    NotifyDataSetChanged();
                }
                return fragment.data_list[position];
            }
            public override View GetView (int position, View convertView, ViewGroup parent)
            {
                return base.GetView (position, convertView, parent);
            }
        }

        #endregion
    }
}

