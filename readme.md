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




