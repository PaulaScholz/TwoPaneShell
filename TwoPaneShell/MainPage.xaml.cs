using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MUXC = Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwoPaneShell
{
    /// <summary>
    /// This page contains a TwoPaneView panel named "MainView", defined in the XAML.
    /// https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.xaml.controls.twopaneview?view=winui-2.3
    /// https://www.reflectionit.nl/blog/2019/xaml-twopaneview
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        /// <summary>
        /// This lets downlevel Pages or UserControls access MainPage public instance methods and 
        /// properties through this static instance variable, set in the MainPage constructor.
        /// </summary>
        public static MainPage Current = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public GridLength Pane1DominantWidth { get; set; }
        public GridLength Pane1SharedWidth { get; set; }
        public GridLength Pane2DominantWidth { get; set; }
        public GridLength Pane2SharedWidth { get; set; }

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

            MainView.ModeChanged += MainView_ModeChanged;

            //SetBothPanesEqual();
        }

        private void MainView_ModeChanged(Microsoft.UI.Xaml.Controls.TwoPaneView sender, object args)
        {
            Microsoft.UI.Xaml.Controls.TwoPaneViewMode theMode = sender.Mode;

            Debug.WriteLine(string.Format("MinWideModeWidth is {0}", MainView.MinWideModeWidth));
            Debug.WriteLine(string.Format("MinTallModeHeight is {0}", MainView.MinTallModeHeight));

            double minWideModeWidth = MainView.MinWideModeWidth;
            double minTallModeHeight = MainView.MinTallModeHeight;
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            if(windowWidth > minWideModeWidth)
            {
                Debug.WriteLine("Window width > minWideModeWidth, should be in Wide mode");
            }
            else if (windowWidth < minWideModeWidth && windowHeight > minTallModeHeight)
            {
                Debug.WriteLine("Window width < minWideModeWidth && windowHeight > minTallModeHeight, should be in Tall mode");
            }
            else if (windowWidth < minWideModeWidth && windowHeight < minTallModeHeight)
            {
                Debug.WriteLine("Window width < minWideModeWidth && windowHeight < minTallModeHeight, should be in Single Pane mode"); 
            }
            else
            {
                Debug.WriteLine("Can't compute mode");
            }

            switch (theMode)
            {
                case MUXC.TwoPaneViewMode.SinglePane:
                    //
                    Debug.WriteLine("MainView_ModeChanged TwoPaneView Mode is SinglePane");
                    
                    break;
                case MUXC.TwoPaneViewMode.Tall:
                    //
                    Debug.WriteLine("MainView_ModeChanged TwoPaneView Mode is Tall");

                    break;
                case MUXC.TwoPaneViewMode.Wide:
                    //
                    Debug.WriteLine("MainView_ModeChanged TwoPaneView Mode is Wide");

                    break;
                default:
                    //
                    break;
            }

            Debug.WriteLine(string.Format("TwoPaneView.Pane1Lenth = {0}, Pane2Length = {1}", MainView.Pane1Length, MainView.Pane2Length));
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            // this computes the current orientation and spanned status
            QueryOrientation();

            Debug.WriteLine("Width of Window is {0}",Window.Current.Bounds.Width);
            Debug.WriteLine("Height of Window is {0}", Window.Current.Bounds.Height);

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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
