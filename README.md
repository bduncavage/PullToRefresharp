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
// Get a reference to the view
var myGridView = FindViewById<PullToRefresharp.Android.Widget.GridView>(Resource.Id.myGridView);
myGridView.RefreshActivated += (o, e) { RefreshMyContent(); };

// when content refresh complete
myGridView.OnRefreshCompleted();
```

### Features

Out of the box PullToFrefresharp gives you a clean pull-to-refresh header UI and assets. You can change the colors, image assets, or strings via XML attributes, or include your own fully custom header.

If you want to add pull-to-refresh to a view type other than GridView, ListView, or ScrollView, you can! Simply create a subclass of the view and implement the `IPullToRefreshWrappedView` interface (there's almost nothing to it, all you need to do is write a little bit of glue code to proxy most calls to `PullToRefresharp.Delegates.ViewDelegate`).

Overriding the stock header is accomplished by adding your header layout to the `ViewWrapper`, i.e.

```
<pulltorefresharp.android.views.ViewWrapper>
    <LinearLayout>
        <!-- your custom header -->
    </LinearLayout>
    <pulltorefresharp.android.widget.GridView />
</pulltorefresharp.android.views.ViewWrapper>
```

Use [ptrsharp_header.xml](https://github.com/bduncavage/PullToRefresharp/blob/master/PullToRefresharp.Android/Resources/layout/ptrsharp_header.xml) as a reference.


### Fork it on GitHub!

[PullToRefresharp on GitHub](http://github.com/bduncavage/PullToRefresharp)

PullToRefresharp is free and open source software. If there's something you want to add, please do! The goal of PullToRefresharp is to provide a robust pull-to-refresh framework for all Xamarin.Android (and possiby Xamarin.iOS in the future) applications.

### License

This software is distributed under the MIT software license.

The MIT License (MIT)

Copyright (c) <year> <copyright holders>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
