using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using System;
using Android.Content.Res;
using Android.Support.V4.View;
using Android.OS;

namespace PullToRefresharp.Android.Views
{
    public enum PullToRefresharpRefreshState
    {
        PullToRefresh,
        ReleaseToRefresh,
        Refreshing,
    }

    public class ViewWrapper : FrameLayout, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private const string TAG = "PullToRefresharpWrapper";

        private PullToRefresharpRefreshState refresh_state = PullToRefresharpRefreshState.PullToRefresh;

        private int header_measured_height;
        private int current_scroll_y;
        private int last_touch_y = -1;
        private bool send_down_event;
        private bool did_steal_event_stream;

        // Custom attributes
        private int header_res_id;
        private int content_view_res_id;
        private int pulldown_progress_indicator_res_id;
        private int pull_to_refresh_string_id;
        private int release_to_refresh_string_id;
        private int refreshing_string_id;
        private float pulldown_tension_factor;
        private int header_background_res_id;
        private ColorStateList header_text_color;
        private int pulldown_icon_drawable_res_id;
        private int fastscroll_thumb_width;

        private Scroller scroller;
        private IPullDownProgressIndicator pulldown_progress_indicator;
        private ViewGroup inner_header;
        private TextView refresh_text;

        #region Public API

        public float PullDownTensionFactor {
            get { return pulldown_tension_factor; }
            set {
                if (value < 0 || value > 1) {
                    throw new ArgumentOutOfRangeException("Tension factor must be 0 < x <= 1");
                }
                pulldown_tension_factor = value;
            }
        }

        public int SnapbackDuration;
        public bool IsPullEnabled;
        public PullToRefresharpRefreshState State {
			get {
				return refresh_state;
			}
		}

        public IPullToRefresharpWrappedView ContentView {
            get;
            private set;
        }

        public ViewGroup Header {
            get;
            private set;
        }

        #endregion

        #region Constructors
        
        public ViewWrapper(Context context) : this(context, null, 0)
        {
        }
        
        public ViewWrapper(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }
        
        public ViewWrapper(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize(attrs, defStyle);
        }
        
        #endregion

        #region Private methods

        private void Initialize(IAttributeSet attrs, int defStyle)
        {
            InitalizeAttributes(attrs, defStyle);

            scroller = new Scroller(Context);

            // I'm not using ViewTreeObserver.GlobalLayout because it gets called too many
            // times, even after removing the handler, seems like a bug maybe.
            this.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

        private void InitalizeAttributes(IAttributeSet attrs, int defStyle) 
        {
            TypedArray a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.PullToRefresharpWrapper, defStyle, 0);

            header_res_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_headerId, 0);
            content_view_res_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_contentViewId, 0);
            pulldown_progress_indicator_res_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_pullDownProgressIndicatorId, 0);

            pull_to_refresh_string_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_pullToRefreshText,
                                            Resource.String.ptrsharp_pull_to_refresh);
            release_to_refresh_string_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_releaseToRefreshText,
                                            Resource.String.ptrsharp_release_to_refresh);
            refreshing_string_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_refreshingText,
                                            Resource.String.ptrsharp_refreshing);

            header_background_res_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_ptrHeaderBackground, 0);
            header_text_color = a.GetColorStateList(Resource.Styleable.PullToRefresharpWrapper_headerTextColor);

            pulldown_icon_drawable_res_id = a.GetResourceId(Resource.Styleable.PullToRefresharpWrapper_headerIconDrawable, 0);

            pulldown_tension_factor = a.GetFloat(Resource.Styleable.PullToRefresharpWrapper_pullDownTension, 0.5f);
            SnapbackDuration = a.GetInt(Resource.Styleable.PullToRefresharpWrapper_snapbackDuration, 400);
            IsPullEnabled = a.GetBoolean(Resource.Styleable.PullToRefresharpWrapper_pullEnabled, true);

            fastscroll_thumb_width = a.GetDimensionPixelSize(Resource.Styleable.PullToRefresharpWrapper_fastScrollThumbWidth, -1);
            if (fastscroll_thumb_width < 0) {
                fastscroll_thumb_width = Resources.GetDimensionPixelSize(Resource.Dimension.fastscroll_thumb_width);
            }

            // Enforce the constraint that both or none of the attributes headerId and viewId are set
            if (header_res_id > 0 && content_view_res_id == 0 || content_view_res_id > 0 && header_res_id == 0) {
                throw new ArgumentException("Both headerId and contentViewId must be either set or not set, setting just one is not supported");
            }

            a.Recycle();
        }

        private void UpdateHeaderTopMargin(int margin)
        {
            // TODO: Not doing this the "short hand" way because it doesn't seem to
            // work with GridView, which doesn't make sense, need to investigate.
            var layout_params = (FrameLayout.LayoutParams)Header.LayoutParameters;
            layout_params.TopMargin = margin;

            // Since the Xamarin Component store wants component libraries built with
            // Froyo (for greatest compatibility), I cannot access Build.VERSION_CODES.GingerbreadMr1
            // Its value is 10, but I'm not happy about having to do this. But I also don't want
            // to apply this hack unnecessarily.
            if ((int)Build.VERSION.SdkInt <= 10) {
                // A nasty hack for fixing margins in <= GingerbreadMr1
                layout_params.Gravity = GravityFlags.Top;
            }
            Header.LayoutParameters = layout_params;
        }

        private void UpdateInnerHeaderTopMargin(int margin)
        {
            if (inner_header != null) {
                var lp = (ViewGroup.MarginLayoutParams)inner_header.LayoutParameters;
                lp.TopMargin = margin;
                inner_header.LayoutParameters = lp;
                // also move the container up to offset the new height
                UpdateHeaderTopMargin(-(header_measured_height + margin));
            }
        }

        private void UpdateRefreshState(PullToRefresharpRefreshState state)
        {
            if (refresh_state == state) {
                return;
            }

            refresh_state = state;

            if (pulldown_progress_indicator != null) {
                pulldown_progress_indicator.SetRefreshState(state);
            }

            switch(refresh_state) {
                case PullToRefresharpRefreshState.Refreshing:
                    refresh_text.Text = Resources.GetString(refreshing_string_id);
                    ContentView.OnRefreshActivated();
                    break;

                case PullToRefresharpRefreshState.PullToRefresh:
                    refresh_text.Text = Resources.GetString(pull_to_refresh_string_id);
                    break;

                case PullToRefresharpRefreshState.ReleaseToRefresh:
                    refresh_text.Text = Resources.GetString(release_to_refresh_string_id);
                    break;
            }
        }

        #endregion

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (current_scroll_y < 0) {
                // enforce the offsets
                if (Header != null) {
                    Header.OffsetTopAndBottom(-current_scroll_y);
                }
                if (ContentView != null) {
                    ((View)ContentView).OffsetTopAndBottom(-current_scroll_y);
                }
            }
        }

        #region Touch Handling

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            //Log.Debug("PTR", "Can intercept: {0}", ev.ActionMasked);

            if (!IsPullEnabled) {
                return base.OnInterceptTouchEvent(ev);
            }

			if (null == ContentView) {
				return base.OnInterceptTouchEvent (ev);
			}

            if (ev.ActionMasked == MotionEventActions.Down && ContentView.IsAtTop
                && refresh_state != PullToRefresharpRefreshState.Refreshing) {
                did_steal_event_stream = false;
                send_down_event = false;

                last_touch_y = (int)ev.RawY;
                if (current_scroll_y < 0) {
                    // tell the wrapped view to ignore this event
                    // we can't steal it outright TODO: explain why
                    ContentView.IgnoreTouchEvents = true;
                }
            } else if (ev.ActionMasked == MotionEventActions.Up && current_scroll_y < 0 
                       && refresh_state != PullToRefresharpRefreshState.Refreshing) {
                // Same as above, tell the view to ignore touch events so it
                // doesn't register a click at the wrong time.
                ContentView.IgnoreTouchEvents = true;
            } else if (ev.ActionMasked == MotionEventActions.Move) {
                ContentView.IgnoreTouchEvents = refresh_state == PullToRefresharpRefreshState.Refreshing && current_scroll_y < 0;
                if (ContentView.IgnoreTouchEvents) {
                    send_down_event = true;
                    did_steal_event_stream = true;
                    return true;
                }
                return false;
            }

            return base.OnInterceptTouchEvent(ev);
        }

        private bool should_cancel_before_up;
        private bool should_send_down_before_up;
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!IsPullEnabled) {
                return base.OnTouchEvent(e);
            }

			if (null == ContentView) {
				return base.OnTouchEvent(e);
			}

            if (ContentView is AbsListView && ((AbsListView)ContentView).FastScrollEnabled) {
                // An adimittedly crude way to determine if touch is in fast scroll, but
                // should accomplish the goal of not displaying ptr header.
                // This is crude because there is not a definitive way to determine
                // a) if the fast scroller is visible or
                // b) the width of the scroller
                if (Resources.DisplayMetrics.WidthPixels - e.RawX < fastscroll_thumb_width) {
                    return false; // let the list view handle this
                }
            }

            switch(e.ActionMasked) {
                case MotionEventActions.Down:
                    last_touch_y = ContentView.IsAtTop ? (int)e.RawY : -1;
                    if (!ContentView.IsAtTop) {
                        send_down_event = false; // because this event will reach the ContentView
                    } else {
                        should_send_down_before_up = true;
                    }
                    return ContentView.IsAtTop;

                case MotionEventActions.Move:
                    if (did_steal_event_stream && current_scroll_y >= 0) {
                        if (send_down_event) {
                            did_steal_event_stream = false;
                            send_down_event = false;
                            ContentView.IgnoreTouchEvents = false;
                            SendFakeEvent(e, MotionEventActions.Down);
                        }
                        return true;
                    }

                    if (last_touch_y == -1) {
                        last_touch_y = (int)e.RawY;
                        return true;
                    }

                    var y_delta = last_touch_y - (int)e.RawY;
                    last_touch_y = (int)e.RawY;

                    bool isMovingUp = y_delta > 0;
                    if (isMovingUp && current_scroll_y >= 0 || !ContentView.IsAtTop) {
                        should_send_down_before_up = false;
                        ContentView.ForceHandleTouchEvent(e);
                        return true;
                    }

                    if (ContentView.IsAtTop) {
                        var new_scroll_to = isMovingUp ? (int)y_delta : (int)(y_delta * PullDownTensionFactor);
                        // see if this will fully hide the header
                        if (current_scroll_y < 0 && current_scroll_y + new_scroll_to > 0) {
                            // only scroll enough to hide the header
                            new_scroll_to = -current_scroll_y;
                        }

                        current_scroll_y += new_scroll_to;

                        Header.OffsetTopAndBottom(-new_scroll_to);
                        ((View)ContentView).OffsetTopAndBottom(-new_scroll_to);
                        ViewCompat.PostInvalidateOnAnimation(this);

                        if (current_scroll_y == 0) {
                            ContentView.IgnoreTouchEvents = false;
                            return true;
                        } else {
                            should_cancel_before_up = true;
                            should_send_down_before_up = false;
                        }

                        if (Math.Abs(current_scroll_y) >= header_measured_height) {
                            if (refresh_state != PullToRefresharpRefreshState.Refreshing) {
                                SetPullDownIconProgress(1);
                                UpdateRefreshState(PullToRefresharpRefreshState.ReleaseToRefresh);
                            }
                        } else {
                            // Don't update anything if we are refreshing.
                            if (refresh_state != PullToRefresharpRefreshState.Refreshing) {
                                SetPullDownIconProgress((float)Math.Abs(current_scroll_y) / (float)header_measured_height);
                                UpdateRefreshState(PullToRefresharpRefreshState.PullToRefresh);
                            }
                        }

                        return true;
                    }
                    return false;

                case MotionEventActions.Up:
                    return HandleUpEvent(e);
            }

            return base.OnTouchEvent(e);
        }

        private bool HandleUpEvent(MotionEvent e)
        {
            last_touch_y = -1;

            if (refresh_state == PullToRefresharpRefreshState.ReleaseToRefresh) {
                UpdateRefreshState(PullToRefresharpRefreshState.Refreshing);
            }

            if (current_scroll_y < 0) {
                Post(StartSnapback);
                ContentView.IgnoreTouchEvents = false;
            }

            if (did_steal_event_stream && current_scroll_y >= 0) {
                did_steal_event_stream = false;
                ContentView.ForceHandleTouchEvent(e);
            }

            if (should_cancel_before_up) {
                // This means we revealed the ptr header and should cancel the event
                // so the content view doesn't register a click on up.
                should_cancel_before_up = false;
                SendFakeEvent(e, MotionEventActions.Cancel);
            } else if (should_send_down_before_up) {
                should_send_down_before_up = false;
                SendFakeEvent(e, MotionEventActions.Down);
            }

            return false;
        }

        private void SendFakeEvent(MotionEvent e, MotionEventActions forcedAction)
        {
            var fake_event = MotionEvent.ObtainNoHistory(e);
            fake_event.Action = forcedAction;
            ContentView.ForceHandleTouchEvent(fake_event);
            fake_event.Recycle();
        }

        private void SetPullDownIconProgress(float progress)
        {
            if (pulldown_progress_indicator != null) {
                pulldown_progress_indicator.SetProgress(progress);
            }
        }

        #endregion

        #region Snapback Handling

        public override void ComputeScroll()
        {
            IncrementSnapback();
            base.ComputeScroll();
        }

        private void StartSnapback()
        {
            var y_delta = -current_scroll_y;
            if (refresh_state == PullToRefresharpRefreshState.Refreshing) {
                // snap back just to the height of the header
                y_delta = Math.Abs(current_scroll_y) - header_measured_height;
            }
            scroller.StartScroll(0, current_scroll_y, 0, y_delta, SnapbackDuration);
            ViewCompat.PostInvalidateOnAnimation(this);
        }

        private void IncrementSnapback()
        {
            if (scroller.IsFinished) {
                return;
            }

            var more = scroller.ComputeScrollOffset();
            Header.OffsetTopAndBottom(current_scroll_y - scroller.CurrY);
            ((View)ContentView).OffsetTopAndBottom(current_scroll_y - scroller.CurrY);
            current_scroll_y = scroller.CurrY;

            if (more) {
                ViewCompat.PostInvalidateOnAnimation(this);
            }
        }

        private void InterruptSnapback()
        {
            // reset our current scroll position
            scroller.ComputeScrollOffset();
            current_scroll_y = scroller.CurrY;
            scroller.AbortAnimation();
        }

        private void ptrView_RefreshCompleted(object sender, EventArgs args)
        {
            UpdateRefreshState(PullToRefresharpRefreshState.PullToRefresh);
            Post(StartSnapback);
        }

        #endregion

        #region IOnGlobalLayoutListener

        public void OnGlobalLayout()
        {
            // the wrapper must have 1 or 2 children, any more or less
            // is an error
            if (ChildCount == 0 || ChildCount > 2) {
                throw new ArgumentOutOfRangeException(String.Format("Wrapper must have 1 or 2 children, found {0}",
                                                                    ChildCount));
            }

            if (ChildCount == 1) {
                // inject our header
                var header = LayoutInflater.FromContext(Context)
                    .Inflate(Resource.Layout.ptrsharp_header, this, false);

                // set any custom icon drawable here so the next layout pass
                // will have already accounted for it's dimensions
                if (pulldown_icon_drawable_res_id > 0) {
                    var icon = header.FindViewById<ImageView>(Resource.Id.icon);
                    icon.SetImageResource(pulldown_icon_drawable_res_id);
                }

                // set the header height to be as tall as the display
                var lp = new ViewGroup.LayoutParams(header.LayoutParameters);
                lp.Height = Context.Resources.DisplayMetrics.HeightPixels;
                header.LayoutParameters = lp;

                AddView(header, 0);
                // return now and this will get called again and we can
                // proceed as usual
                return;
            }

            // find our children and glean the measured size of the header
            if (header_measured_height == 0) {
                if (header_res_id > 0) {
                    Header = FindViewById<ViewGroup>(header_res_id);
                } else {
                    Header = (ViewGroup)GetChildAt(0);
                }

                if (header_background_res_id > 0) {
                    Header.SetBackgroundResource(header_background_res_id);
                }

                if (header_text_color != null) {
                    var text_view = Header.FindViewById<TextView>(Resource.Id.text);
                    if (text_view != null) {
                        text_view.SetTextColor(header_text_color);
                    }
                }

                // Header should not be null at this point, if it is something is totally wrong
                // so I'm not checking for null.
                // TODO: Possibly add an attribute to define the id of the inner header
                inner_header = (ViewGroup)Header.GetChildAt(0);

                if (content_view_res_id > 0) {
                    ContentView = (IPullToRefresharpWrappedView)FindViewById<View>(content_view_res_id);
                } else {
                    ContentView = (IPullToRefresharpWrappedView)GetChildAt(1);
                }

                if (pulldown_progress_indicator_res_id > 0) {
                    pulldown_progress_indicator = FindPullDownProgressIndicator(pulldown_progress_indicator_res_id);
                } else {
                    pulldown_progress_indicator = FindPullDownProgressIndicator(Resource.Id.pullDownProgressIndicator);
                }

                ContentView.RefreshCompleted += ptrView_RefreshCompleted;

                header_measured_height = inner_header.Height;
                UpdateHeaderTopMargin(-Header.Height);

                // set initial text
                refresh_text = FindViewById<TextView>(Resource.Id.text);
                refresh_text.Text = Resources.GetString(pull_to_refresh_string_id);
            }

            if (header_measured_height > 0) {
                Header.ViewTreeObserver.RemoveGlobalOnLayoutListener(this);
            }
        }

        private IPullDownProgressIndicator FindPullDownProgressIndicator(int resId)
        {
            var view = FindViewById<View>(resId);
            return view is IPullDownProgressIndicator ? view as IPullDownProgressIndicator : null;
        }

        #endregion
    }
}

