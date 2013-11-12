using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SupportFragment = Android.Support.V4.App.Fragment;

using PullToRefresharp.Android.Views;

namespace Sample.Android.Fragments
{
    class ExpandableListFragment : SupportFragment
    {
        // LOOK HERE!
        // The easiest way to interact with your pull to refresh view is to 
        // hold a reference to it that's typed to IPullToRefresharpView.
        private IPullToRefresharpView ptr_view;

        private PullToRefresharp.Android.Widget.ExpandableListView ExpandableListView;

        List<String> items = MainActivity.FLAVORS.ConvertAll<string>(x => (string)x.Clone());

        public ExpandableListFragment() : base()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup root, Bundle data)
        {
            ViewWrapper view = (ViewWrapper)inflater.Inflate(Resource.Layout.expandable,root,false);
            ExpandableListView = view.FindViewById<PullToRefresharp.Android.Widget.ExpandableListView>(Resource.Id.expandablelist);
            return view;
        }

        public override void OnViewStateRestored (Bundle savedInstanceState)
        {
            base.OnViewStateRestored (savedInstanceState);

            while (items.Count < 100) {
                items.AddRange(items);
            }

            ExpandableListView.SetAdapter(new SampleExpandableAdapter(Activity,items));

            if (ptr_view == null && ExpandableListView is IPullToRefresharpView) {
                ptr_view = (IPullToRefresharpView)ExpandableListView;
                // LOOK HERE!
                // Hookup a handler to the RefreshActivated event
                ptr_view.RefreshActivated += ptr_view_RefreshActivated;
            }

        }

        private void ptr_view_RefreshActivated(object sender, EventArgs args)
        {
            // LOOK HERE!
            // Refresh your content when PullToRefresharp informs you that a refresh is needed
            View.PostDelayed(() => {
                if (ptr_view != null) {
                    // When you are done refreshing your content, let PullToRefresharp know you're done.
                    ptr_view.OnRefreshCompleted();
                }
            }, 4000);
        }

        public override void OnDestroyView()
        {
            if (ptr_view != null) {
                ptr_view.RefreshActivated -= ptr_view_RefreshActivated;
                ptr_view = null;
            }

            ExpandableListView = null;

            base.OnDestroyView();
        }

        public override void OnResume ()
        {
            base.OnResume ();
            ExpandableListView.ChildClick += listview_ChildClick;
        }

        public override void OnPause ()
        {
            base.OnPause ();
            ExpandableListView.ChildClick -= listview_ChildClick;
        }

        private void listview_ChildClick(object sender, ExpandableListView.ChildClickEventArgs args)
        {
            Toast.MakeText(Activity, "Group: "+args.GroupPosition + " Child: "+args.ChildPosition + " Clicked", ToastLength.Short).Show();
        }

        private class SampleExpandableAdapter : BaseExpandableListAdapter
        {
            private List<string> Items;
            private Context Context;
            public SampleExpandableAdapter(Context context,List<string> items)
            {
                this.Items = items;
                this.Context = context;
            }

            public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
            {
                return null;
            }

            public override long GetChildId(int groupPosition, int childPosition)
            {
                return 0;
            }

            public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                TextView tv;
                if (convertView == null)
                {
                    LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                    tv = (TextView)inflater.Inflate(Resource.Layout.expandable_item, parent, false);
                }
                else
                {
                    tv = (TextView)convertView;
                }
                tv.Text = Items[groupPosition * 10 + childPosition];
                return tv;
            }

            public override int GetChildrenCount(int groupPosition)
            {
                return 10;
            }

            public override Java.Lang.Object GetGroup(int groupPosition)
            {
                return null;
            }

            public override long GetGroupId(int groupPosition)
            {
                return groupPosition;
            }

            public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
            {
                TextView tv;
                if (convertView == null)
                {
                    LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                    tv = (TextView)inflater.Inflate(Resource.Layout.expandable_group, parent, false);
                }
                else
                {
                    tv = (TextView)convertView;
                }
                tv.Text = "Group " + groupPosition;
                return tv;
            }

            public override int GroupCount
            {
                get { return 10; }
            }

            public override bool HasStableIds
            {
                get { return false; }
            }

            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return true;
            }
        }
    }
}