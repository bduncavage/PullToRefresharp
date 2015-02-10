## PullToRefresharp

Integrating PullToRefresharp into your existing application requires only a small amount of work. Modifying your layout to use the PullToRefresharp view and widget, then hooking into the RefreshActivated event, and finally calling OnRefreshComplete when you're done refreshing your content is all that you need to do.

Let's get started!

### Modifying your layout

The first thing you need to replace is any ListView, GridView, or ScrollView you wish to enable pull-to-refresh on with the PullToRefresharp equivalent.

```
<ListView />
```

Would become:

```
<pulltorefresharp.widget.ListView />
```

The next, and last, thing you need to do to your layout is to wrap the view with a `ViewWrapper`. This is the bit that will inject the pull-to-refresh header and manage event handling (it's how PullToRefresharp can be used with more than just ListView).

```
<pulltorefresharp.views.ViewWrapper
    android:layout_width="fill_parent"
    android:layout_height="fill_parent">

    <pulltorefresharp.widget.ListView />

</pulltorefresharp.views.ViewWrapper>
```

You are now done integrating PullToRefresharp into your layout!

### Hooking up Events

You'll want to know when a refresh has been activated. This is easy, just hook up a handler to your GridView, ListView, or ScrollView that has been PullToRefresharp-enabled.

```
var gridView = FindViewById<IPullToRefresharpView>(Resource.Id.myGridView);
gridView.RefreshActivated += (o, e) => { RefreshContent(); };
```

As you can see, your GridView implements the `IPullToRefresharpView` interface. This interface exposes some useful methods and properties (such as enabling/disabling pull-to-refresh). Right now, we only care about the RefreshActivated event, this is raised when the user has pulled beyond the threshold and released.

Now you will want to inform your IPullToRefreshView when you are done refreshing. This is done thusly:

```
gridView.OnRefreshCompleted();
```

This causes the header to snapback up in an animated fashion.

### Congratulations!

You're done! There is no more you need to do to start using PullToRefresharp. Also look at the source for `IPullToRefreshView` and `attrs.xml` to discover more advanced options (http://github.com/bduncavage/PullToRefresharp).
