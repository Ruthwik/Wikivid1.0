using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Wikivid1._0
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Task<string> currentHttpTask;
        private HttpClient httpClient;

        public MainPage()
        {
            this.InitializeComponent();
            httpClient = new HttpClient();
        }

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
                foreach (JsonValue value in parsedResponse[1].GetArray())
                {
                    suggestions.AppendQuerySuggestion(value.GetString());
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
            string url="";
            if (string.IsNullOrEmpty(queryText))
            {
                tbError.Text = "Use the search control to submit a query";
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
                    tbError.Text = "Suggestions could not be retrieved -- please verify that the URL points to a valid service";
                  //  MainPage.Current.NotifyUser(@"Suggestions could not be retrieved -- please verify that the URL points to a valid service (for example http://contoso.com?q={searchTerms}", NotifyType.ErrorMessage);
                }
                catch (Exception es)
                {
                    tbError.Text = "Some Random Excpetion " + es.StackTrace;
                   // MainPage.Current.NotifyUser("Suggestions could not be displayed -- please verify that the service provides valid OpenSearch suggestions", NotifyType.ErrorMessage);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }
    }
}
