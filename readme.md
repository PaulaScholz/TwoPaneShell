# TwoPaneShell

This is `TwoPaneShell`, the UWP shell application used to host two-pane samples for dual-screen Windows devices.  `TwoPaneShell` consists of a MainPage hosting a [TwoPaneView](https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.xaml.controls.twopaneview?view=winui-2.3) control and two Panes, which are placeholders for your own sample controls and content.

![TwoPaneShell](/docimages/TwoPaneShell.png)

A good introduction to `TwoPaneView` by [Fons Sonnemans](https://www.reflectionit.nl/blog/authors/fons-sonnemans) is available on [ReflectionIt](https://www.reflectionit.nl/blog/2019/xaml-twopaneview).

The relative width of each pane is controlled by two `TwoPaneView` properties, the `Pane1Length` and `Pane2Length`, which are of type [GridLength](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.GridLength).  These properties use [Star sizing](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.columndefinition.width) to support the dynamic layout concepts needed by `TwoPaneView` to respond to changes in screen size and position on dual-screen devices.

In this sample, we have four public properties and three methods of `MainPage` used to set these `TwoPaneView` pane length properties, depending on whether the user has spanned the `MainPage` across screen boundaries.

These reflect three states:

    * Pane1 Dominant - Pane 1 is the only visible pane of `MainView`
    * Pane2 Dominant - Pane 2 is the only visible pane of `MainView`
    * Both Panes Equal - Both panes equally share the the content space of `MainView`.  This is primarily used when Pane1 and Pane2 are displayed on separate screens.

We also have three helper functions used to set these states.  These are:

```csharp
        /// <summary>
        /// Make Pane 1 of the TwoPaneView the dominant visible pane. We are doing
        /// this explicitly, but we could also bind the MainView.Pane1Length
        /// and MainView.Pane2Length properties in the XAML if we desired.
        /// </summary>
        public void SetPane1Dominant()
        {
            MainView.Pane1Length = Pane1DominantWidth;
            MainView.Pane2Length = Pane2DominantWidth;
        }

        /// <summary>
        /// Make Pane 2 of the TwoPaneView the dominant visible pane.
        /// </summary>
        public void SetPane2Dominant()
        {
            MainView.Pane1Length = Pane2DominantWidth;
            MainView.Pane2Length = Pane1DominantWidth;
        }

        /// <summary>
        /// Make both panes of equal GridLength, and thus both equally visible.
        /// </summary>
        public void SetBothPanesEqual()
        {
            MainView.Pane1Length = Pane1SharedWidth;
            MainView.Pane2Length = Pane2SharedWidth;
        }
```
These methods may be accessed though the `MainPage.Current` static instance variable in any downlevel page.

Alternatively, the `Pane1Length` and `Pane2Length` properties of `MainView` may be set by binding instead of explicitly through these methods.  There are four public properties defined for this purpose.

```csharp
        public GridLength Pane1DominantWidth { get; set; }
        public GridLength Pane1SharedWidth { get; set; }
        public GridLength Pane2DominantWidth { get; set; }
        public GridLength Pane2SharedWidth { get; set; }
```

## Application Spanning and Screen Orientation
Experiments with TwoPaneView and `ApplicationView.Orientation` values from the `Windows.UI.ViewManagement` namespaces show that these values are not consistently reliable during the course of the TwoPaneView application's lifetime. 

The `Windows.Graphics.Display.CurrentOrientation` and `NativeOrientation` values also display unreliable results after application spanning or screen rotation in a TwoPaneView application.

We have written our own functions to determine the spanned status and application orientation that rely on the initial and current width and height of the TwoPaneView.  Because the TwoPaneView always starts in an unspanned state, we store the `ActualWidth` and `ActualHeight` of TwoPaneView as private page properties and overload the `SizeChanged` event handler to compute the actual values, like this:

```csharp
        /// <summary>
        /// The beginning width of TwoPaneView element, MainView
        /// </summary>
        private double BeginningWidth { get; set; }

        /// <summary>
        /// The beginning height of TwoPaneView element, MainView
        /// </summary>
        private double BeginningHeight { get; set; }

        /// <summary>
        /// The current ApplicationViewOrientation of the MainView (Portrait or Landscape)
        /// </summary>
        public ApplicationViewOrientation CurrentOrientation { get; set; }

        /// <summary>
        /// True if the application is spanned across two screens.
        /// </summary>
        public bool ApplicationIsSpanned { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            Pane1DominantWidth = new GridLength(1, GridUnitType.Star);
            Pane1SharedWidth = new GridLength(0.5, GridUnitType.Star);
            Pane2DominantWidth = new GridLength(0, GridUnitType.Star);
            Pane2SharedWidth = new GridLength(0.5, GridUnitType.Star);

            // point the static instance variable to this instance of the Page.
            Current = this;

            // We get the beginning height/width of the TwoPaneView in Loaded
            Loaded += MainPage_Loaded;            

            // This is called when the screen orientation changes
            SizeChanged += MainPage_SizeChanged;

            SetBothPanesEqual();
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            // this computes the current orientation and spanned status
            QueryOrientation();

            Debug.WriteLine("Actual Width of MainView is {0}", MainView.ActualWidth);
            Debug.WriteLine("Actual Height of MainView is {0}", MainView.ActualHeight);

            Debug.WriteLine(string.Format("CurrentOrientation is {0}", CurrentOrientation.ToString()));
            Debug.WriteLine(string.Format("ApplicationIsSpanned = {0}", ApplicationIsSpanned.ToString()));

            Debug.WriteLine("------------------------------------------------");
        }

        /// <summary>
        /// Do any page-specific initialization here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the initial size of the TwoPaneView element
            BeginningWidth = MainView.ActualWidth;
            BeginningHeight = MainView.ActualHeight;
        }

        /// <summary>
        /// We compute the orientation and spanned status of the TwoPaneView here.
        /// Results update the properties "ApplicationIsSpanned" and "CurrentOrientation"
        /// </summary>
        private void QueryOrientation()
        {
            // based on initial conditions and current conditions, compute orientation
            double currentWidth = MainView.ActualWidth;
            double currentHeight = MainView.ActualHeight;

            // this runs before MainPage_Loaded, when BeginningWidth & BeginningHeight are zero
            if(BeginningWidth !=0 )
            {                
                if(currentWidth == BeginningWidth && currentHeight == BeginningHeight)
                {
                    // if width and height are equal to the beginning width & height, we're not spanned
                    ApplicationIsSpanned = false;
                }
                else if (currentWidth >= BeginningWidth && currentHeight >= BeginningHeight)
                {
                    // it is possible that currentWidth AND currentHeight might be equal to 
                    // their beginning values but that is caught by the clause above.  If only
                    // one of them is equal, then this clause is executed instead.
                    ApplicationIsSpanned = true;
                }
                else
                {
                    ApplicationIsSpanned = false;
                }
            }
            else
            {
                // Application always starts as not spanned
                ApplicationIsSpanned = false;
            }

            // compute our orientation because we can't rely on DisplayInformation or ApplicationView values
            if(currentWidth > currentHeight && !ApplicationIsSpanned)
            {
                CurrentOrientation = ApplicationViewOrientation.Landscape;
            }
            else if (currentWidth < currentHeight && !ApplicationIsSpanned)
            {
                CurrentOrientation = ApplicationViewOrientation.Portrait;
            }
            else if (ApplicationIsSpanned && currentWidth > currentHeight)
            {
                CurrentOrientation = ApplicationViewOrientation.Portrait;
            }
            else
            {
                CurrentOrientation = ApplicationViewOrientation.Landscape;
            }
        }
```






