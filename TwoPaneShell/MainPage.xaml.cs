using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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


        public MainPage()
        {
            this.InitializeComponent();

            Pane1DominantWidth = new GridLength(1, GridUnitType.Star);
            Pane1SharedWidth = new GridLength(0.5, GridUnitType.Star);
            Pane2DominantWidth = new GridLength(0, GridUnitType.Star);
            Pane2SharedWidth = new GridLength(0.5, GridUnitType.Star);

            // point the static instance variable to this instance of the Page.
            Current = this;

            // uncomment this if you have page-specific initialization
            //Loaded += MainPage_Loaded;

            SetBothPanesEqual();
        }

        /// <summary>
        /// Do any page-specific initialization here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // put your code here
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
