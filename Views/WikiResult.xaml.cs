using Wikivid1._0.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Wikivid1._0.ViewModel;
using Wikivid1._0.Model;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Wikivid1._0.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class WikiResult : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        string query = "India";
       // string html;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public WikiResult()
        {
            string html;
            this.InitializeComponent();
            
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            
            //html = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";
           // WebView v = (WebView)FindChildControl<WebView>(lw);
            //this.MyWebview.NavigateToString(html);
        }

        public IEnumerable<ListViewItem> GetListViewItemsFromList(ListView lv)
        {
            return FindChildrenOfType<ListViewItem>(lv);
        }

        public IEnumerable<T> FindChildrenOfType<T>( DependencyObject ob)
            where T : class
        {
            foreach (var child in GetChildren(ob))
            {
                T castedChild = child as T;
                if (castedChild != null)
                {
                    yield return castedChild;
                }
                else
                {
                    foreach (var internalChild in FindChildrenOfType<T>(child))
                    {
                        yield return internalChild;
                    }
                }
            }
        }

        public IEnumerable<DependencyObject> GetChildren(DependencyObject ob)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(ob);

            for (int i = 0; i < childCount; i++)
            {
                yield return VisualTreeHelper.GetChild(ob, i);
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            SearchQuery sq = (SearchQuery)e.Parameter;

           // WikiFetch c = new WikiFetch();
           // c.fetch("http://en.wikipedia.org/wiki/" + sq.Query.Replace(" ","_"));

            this.DataContext = new WikiResultViewModel(sq.Query.Replace(" ", "_"));
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void pageTitle_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void WebView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            
            //WebView v = (WebView)sender;
            //if (v.Tag != null)
            //{
            //    string b = v.Tag.ToString();
            //    v.NavigateToString(b);
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            StackPanel sp = (StackPanel)b.Parent;
            TextBlock myTB = (from child in sp.Children
                              where child is TextBlock && ((TextBlock)child).Tag.Equals("2")
                              select (TextBlock)child).FirstOrDefault();
            myTB.MaxLines = int.MaxValue;

        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            double Lines = tb.ActualHeight / tb.FontSize;
            StackPanel sp = (StackPanel)tb.Parent;
            Button myTB = (from child in sp.Children
                           where child is Button
                           select (Button)child).FirstOrDefault();
            if (Lines < 19)
            {
                myTB.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            TextBlock tB = (TextBlock)sender;
            if (tB.Text != null)
            {
                ProgressRing.Visibility = Visibility.Collapsed;
            }
        }

        private void SMallTextBlock_DataContextChanged_1(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            TextBlock tB = (TextBlock)sender;
            string text = tB.Text;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text;
            TextBox tb = (TextBox)sender;
            if (tb.Text != null)
            {
                text = tb.Text;
                StackPanel sp = (StackPanel)tb.Parent;
                Grid gr = (Grid)sp.Parent;
                WebView myTB = (from child in gr.Children
                                where child is WebView
                                select (WebView)child).FirstOrDefault();
                myTB.NavigateToString(text);
            }
        }

    }
}
