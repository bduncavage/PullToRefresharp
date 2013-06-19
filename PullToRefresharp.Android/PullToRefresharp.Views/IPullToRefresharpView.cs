//
// IPullToRefresharpView.cs
//
// Author:
//   Brett Duncavage <brett.duncavage@rd.io>
//
// Copyright 2013 Brett Duncavage
//
using System;

namespace PullToRefresharp.Android.Views
{
    public interface IPullToRefresharpView
    {
        /// <summary>
        /// Enables/Disables pull to refresh for this view.
        /// </summary>
        /// <value><c>true</c> if pull to refresh enabled; otherwise, <c>false</c>.</value>
        bool PullToRefreshEnabled { get; set; }

        /// <summary>
        /// Returns the current state of the pull to refresh.
        /// </summary>
        /// <value>The refresh state.</value>
        PullToRefresharpRefreshState RefreshState { get; }

        /// <summary>
        /// Occurs when refresh activated. Hook into this event so you can trigger a refresh
        /// of the content for the view.
        /// </summary>
        event EventHandler RefreshActivated;

        /// <summary>
        /// Raises the refresh completed event. Call this when your refresh is complete
        /// and you wish to hide the pull to refresh header.
        /// </summary>
        void OnRefreshCompleted();
    }
}

