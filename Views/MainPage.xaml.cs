using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wikivid1._0.Model;
using Windows.ApplicationModel.Search;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
        private IAsyncOperation<XmlDocument> currentXmlRequestOp;

        public MainPage()
        {
            this.InitializeComponent();
            httpClient = new HttpClient();
        }

        private void AddSuggestionFromNode(IXmlNode node, SearchSuggestionCollection suggestions)
        {
            string text = "";
            string description = "";
            string url = "";
            string imageUrl = "";
            string imageAlt = "";

            foreach (IXmlNode subNode in node.ChildNodes)
            {
                if (subNode.NodeType != NodeType.ElementNode)
                {
                    continue;
                }
                if (subNode.NodeName.Equals("Text", StringComparison.CurrentCultureIgnoreCase))
                {
                    text = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Description", StringComparison.CurrentCultureIgnoreCase))
                {
                    description = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Url", StringComparison.CurrentCultureIgnoreCase))
                {
                    url = subNode.InnerText;
                }
                else if (subNode.NodeName.Equals("Image", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (subNode.Attributes.GetNamedItem("source") != null)
                    {
                        imageUrl = subNode.Attributes.GetNamedItem("source").InnerText;
                    }
                    if (subNode.Attributes.GetNamedItem("alt") != null)
                    {
                        imageAlt = subNode.Attributes.GetNamedItem("alt").InnerText;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                // No proper suggestion item exists
            }
            else if (string.IsNullOrWhiteSpace(url))
            {
                suggestions.AppendQuerySuggestion(text);
            }
            else
            {
                // The following image should not be used in your application for Result Suggestions.  Replace the image with one that is tailored to your content
                Uri uri = string.IsNullOrWhiteSpace(imageUrl) ? new Uri("ms-appx:///Assets/SDK_ResultSuggestionImage.png") : new Uri(imageUrl);
                RandomAccessStreamReference imageSource = RandomAccessStreamReference.CreateFromUri(uri);
                suggestions.AppendResultSuggestion(text, description, text, imageSource, imageAlt);
            }
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
            if (currentXmlRequestOp != null)
            {
                currentXmlRequestOp.Cancel();
            }

            // Get the suggestion from a web service.
            currentXmlRequestOp = XmlDocument.LoadFromUriAsync(new Uri(str));
            XmlDocument doc = await currentXmlRequestOp;
            currentXmlRequestOp = null;
            XmlNodeList nodes = doc.GetElementsByTagName("Section");
            if (nodes.Count > 0)
            {
                IXmlNode section = nodes[0];
                foreach (IXmlNode node in section.ChildNodes)
                {
                    if (node.NodeType != NodeType.ElementNode)
                    {
                        continue;
                    }
                    if (node.NodeName.Equals("Separator", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string title = null;
                        IXmlNode titleAttr = node.Attributes.GetNamedItem("title");
                        if (titleAttr != null)
                        {
                            title = titleAttr.NodeValue.ToString();
                        }
                        suggestions.AppendSearchSeparator(String.IsNullOrWhiteSpace(title) ? "Suggestions" : title);
                    }
                    else
                    {
                        AddSuggestionFromNode(node, suggestions);
                    }
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
                     url = @"http://en.wikipedia.org/w/api.php?action=opensearch&search=" + queryText + @"&limit=10&namespace=0&format=xml";
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
    }
}
