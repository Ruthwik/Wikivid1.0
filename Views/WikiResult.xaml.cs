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
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;
using System.Net.Http;
using Windows.Data.Json;
using Windows.Storage.Streams;

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
            httpClient = new HttpClient();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            DispatcherTimerSetup();
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
                dispatcherTimer.Stop();
                ProgressText.Visibility = Visibility.Collapsed;
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

        #region DISPATCHER

        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;
        int timesTicked = 1;
        int timesToTick = 10;

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            startTime = DateTimeOffset.Now;
            lastTime = startTime;
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            timesTicked++;
            Random R = new Random();
            ;
            ProgressText.Text = funnyText[R.Next(0, 19)];
            
        }

        string []funnyText = {"Loading Please Wait..",
                                 "Please Wait More.. More Loading",
                                 "Feeling Hungry? Please order food",
                                 "Getting Data From Internet",
                                 "Loading Brains",
                                 "Applying Advanced Algorithms which no one understands",
                                 "Gone to EAT, Be right Back!",
                                 "Please Wait.. About to Load",
                                 "Be ready to experience something cool",
                                 "Filling RocketFuel.. Please Wait",
                                 "Loading More Data",
                             "Just a few seconds more",
                             "Better late than never",
                             "Word Hard. Dream Big",
                             "The Machine is Learning",
                             "Getting Videos...",
                             "Getting some nice Text",
                             "It's something wait the worth!",
    "Java Developers wear glasses because they can't C#","Applying some Data Mining","","",""};

        #endregion

        #region SEARCHBOX
        private HttpClient httpClient;
        private Task<string> currentHttpTask;
        /// <summary>
        /// Completes the retreval of suggestions from the web service
        /// </summary>
        /// <param name="str">User query string</param>
        /// <param name="suggestions">Suggestions list to append new suggestions</param>
        /// <returns></returns>
        private async Task GetSuggestionsAsync(string str, SearchSuggestionCollection suggestions)
        {
            // Cancel the previous suggestion request if it is not finished.
            if (currentHttpTask != null)
            {
                currentHttpTask.AsAsyncOperation<string>().Cancel();
            }

            // Get the suggestions from an open search service.
            currentHttpTask = httpClient.GetStringAsync(str);
            string response = await currentHttpTask;
            JsonArray parsedResponse = JsonArray.Parse(response);
            if (parsedResponse.Count > 1)
            {
                suggestions.AppendSearchSeparator("Exact Match");
                foreach (JsonValue value in parsedResponse[1].GetArray())
                {
                    //suggestions.AppendQuerySuggestion(value.GetString());
                    var imageUri = new Uri("ms-appx:///test.png");
                    var imageRef = RandomAccessStreamReference.CreateFromUri(imageUri);
                    suggestions.AppendResultSuggestion(value.GetString(), "", value.GetString(), imageRef, "");
                    //suggestions.AppendResultSuggestion(value.GetString(), "Details", "baz", imageRef, "Result");
                }
            }
        }

        /// <summary>
        /// Populates SearchBox with Suggestions when user enters text
        /// </summary>
        /// <param name="sender">Xaml SearchBox</param>
        /// <param name="e">Event when user changes text in SearchBox</param>
        private async void SearchBoxEventsSuggestionsRequested(Object sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            var queryText = e.QueryText;
            string url = "";
            if (string.IsNullOrEmpty(queryText))
            {
                //tbError.Text = "Use the search control to submit a query";
                //MainPage.Current.NotifyUser("Use the search control to submit a query", NotifyType.StatusMessage);
            }
            else
            {
                // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
                var request = e.Request;
                var deferral = request.GetDeferral();

                try
                {
                    url = @"http://en.wikipedia.org/w/api.php?action=opensearch&search=" + queryText + @"&limit=10&namespace=0&format=json";
                    // Use the web service Url entered in the UrlTextBox that supports OpenSearch Suggestions in order to see suggestions come from the web service.
                    // See http://www.opensearch.org/Specifications/OpenSearch/Extensions/Suggestions/1.0 for details on OpenSearch Suggestions format.
                    // Replace "{searchTerms}" of the Url with the query string.
                    Task task = GetSuggestionsAsync(url, request.SearchSuggestionCollection);
                    await task;

                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        if (request.SearchSuggestionCollection.Size > 0)
                        {

                            //MainPage.Current.NotifyUser("Suggestions provided for query: " + queryText, NotifyType.StatusMessage);
                        }
                        else
                        {
                            // MainPage.Current.NotifyUser("No suggestions provided for query: " + queryText, NotifyType.StatusMessage);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // We have canceled the task.
                }
                catch (FormatException)
                {
                   // tbError.Text = "Suggestions could not be retrieved -- please verify that the URL points to a valid service";
                    //  MainPage.Current.NotifyUser(@"Suggestions could not be retrieved -- please verify that the URL points to a valid service (for example http://contoso.com?q={searchTerms}", NotifyType.ErrorMessage);
                }
                catch (Exception es)
                {
                   // tbError.Text = "Some Random Excpetion " + es.StackTrace;
                    // MainPage.Current.NotifyUser("Suggestions could not be displayed -- please verify that the service provides valid OpenSearch suggestions", NotifyType.ErrorMessage);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private void SearchBoxSuggestions_ResultSuggestionChosen(SearchBox sender, SearchBoxResultSuggestionChosenEventArgs args)
        {
            string chosen = args.Tag.ToString();
            SearchQuery sq = new SearchQuery()
            {
                Query = chosen,
                ExaxtMatch = 1
            };
            this.Frame.Navigate(typeof(Views.WikiResult), sq);
        }

        private void SearchBoxSuggestions_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            string chosen = args.QueryText;
            SearchQuery sq = new SearchQuery()
            {
                Query = chosen,
                ExaxtMatch = 0
            };
            this.Frame.Navigate(typeof(Views.WikiResult), sq);
        }

        #endregion
    }
}
