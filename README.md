# PullToRefresharp

## Pull To Refresh for Android in `C#`

Add pull-to-refresh functionality to your Android app quickly and easily.

PullToRefresharp is the only C# library that provides pull-to-refresh functionality to ListView, GridView, and ScrollView on Android. It is simple to integrate with, customizable and extensible.

See it in action here: [PullToRefresharp in Action](http://vimeo.com/68728191)

### Simple integration:

Add the support library:

Ensure that your project has a reference to Mono.Android.Support.v4
This allows PullToRefresharp to be compatible with Gingerbread (Android 2.3) and lower.

Update your layout:

```
<pulltorefresharp.android.views.ViewWrapper
    android:layout_width="fill_parent"
    android:layout_height="fill_parent">

    <pulltorefresharp.android.widget.GridView
        android:id="@id/myGridView" />

</pulltorefresharp.android.views.ViewWrapper>
```

Hook into the `RefreshActivated` event

```csharp
// Get a reference to the view, in this case we only care about methods in IPullToRefresharpView
// so we pull the view out as that type.
var myPullToRefresharpView = FindViewById<IPullToRefresharpView>(Resource.Id.myGridView);
myPullToRefresharpView.RefreshActivated += (o, e) { RefreshMyContent(); };

// when content refresh complete
myPullToRefresharpView.OnRefreshCompleted();

// -----------------------------------------

// Alternatively, you can pull the widget out of the view.
// This way you can access it as the widget and as an IPullToRefresharpView
var myGridView = FindViewById<PullToRefresharp.Android.Widget.GridView>(Resource.Id.myGridView);
myGridView.RefreshActivated += (o, e) { RefreshMyContent(); };

// when content refresh complete
myGridView.OnRefreshCompleted();
```

### Features

Out of the box PullToFrefresharp gives you a clean pull-to-refresh header UI and assets. You can change the colors or image assets via XML attributes, or include your own fully custom header.

If you want to add pull-to-refresh to a view type other than GridView, ListView, or ScrollView, you can! Simply create a subclass of the view and implement the `IPullToRefreshWrappedView` interface (there's almost nothing to it, all you need to do is write a little bit of glue code to proxy most calls to `PullToRefresharp.Delegates.ViewDelegate`).

### Fork it on GitHub!

[PullToRefresharp on GitHub](http://github.com/bduncavage/PullToRefresharp)

PullToRefresharp is free and open source software. If there's something you want to add, please do! The goal of PullToRefresharp is to provide a robust pull-to-refresh framework for all Xamarin.Android (and possiby Xamarin.iOS in the future) applications.

