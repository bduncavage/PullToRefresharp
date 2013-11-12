using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Sample.Android.Fragments;

using SupportFragmentActivity = Android.Support.V4.App.FragmentActivity;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;

using Android.Content;
using System.Collections.Generic;

namespace Sample.Android
{
    [Activity (Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : SupportFragmentActivity
    {
        public static readonly List<String> FLAVORS = new List<string>() {
            "Donut", "Eclair", "Froyo", "Gingerbread", "Honeycomb", "Ice Cream Sandwich", "Jelly Bean"
        };

        private NavFragment nav;
        private SampleListFragment list_fragment;
        private GridFragment grid_fragment;
        private ImageFragment image_fragment;
        private ExpandableListFragment expandablelist_fragment;

        #region lifecycle

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.main_layout);

            var fm = SupportFragmentManager;
            var transaction = fm.BeginTransaction();
            nav = new NavFragment();
            transaction.Replace(Resource.Id.fragment_container, nav);

            transaction.Commit();

            nav.NavigationItemActivated += nav_NavigationItemSelected;
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            nav.NavigationItemActivated -= nav_NavigationItemSelected;
        }

        #endregion

        #region private methods

        private void nav_NavigationItemSelected(object sender, NavigationEventArgs args)
        {
            var containerResId = Resource.Id.fragment_container;

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.SetTransition((int)SupportFragmentTransaction.TransitFragmentOpen);

            if (args.NavItemPosition == NavigationDestination.List) {
                list_fragment = list_fragment ?? new SampleListFragment();
                list_fragment.FastScrollEnabled = false;
                transaction.Replace(containerResId, list_fragment);
            } else if (args.NavItemPosition == NavigationDestination.Grid) {
                grid_fragment = grid_fragment ?? new GridFragment();
                transaction.Replace(containerResId, grid_fragment);
            } else if (args.NavItemPosition == NavigationDestination.ScrollView) {
                image_fragment = new ImageFragment(Resource.Drawable.android_flavors);
                transaction.Replace(containerResId, image_fragment);
            } else if (args.NavItemPosition == NavigationDestination.ListFastScroll) {
                list_fragment = list_fragment ?? new SampleListFragment();
                list_fragment.FastScrollEnabled = true;
                transaction.Replace(containerResId, list_fragment);
            } else if (args.NavItemPosition == NavigationDestination.ExpandableList) {
                expandablelist_fragment = expandablelist_fragment ?? new ExpandableListFragment();
                transaction.Replace(containerResId, expandablelist_fragment);
            }

            transaction.AddToBackStack(null);
            transaction.Commit();
        }

        #endregion
    }
}


