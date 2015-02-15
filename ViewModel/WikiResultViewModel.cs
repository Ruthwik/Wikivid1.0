using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wikivid1._0.Common;




public class LinkItem
{
    public string Href;
    public string Text;

    public override string ToString()
    {
        return "https://www.youtube.com" + Href;
    }
    public string TextString()
    {
        return Text + "\n";
    }
}



namespace Wikivid1._0.ViewModel
{
    public class JsonYoutube
    {
        public JsonYoutube(string json)
        {
            //json.Replace("{[", "{");
          //  json.Replace("]}", "}");
            JObject jObject = JObject.Parse(json);
            JToken jItems = jObject["items"][0];
            JToken js = jItems["id"];
            vidId = (string)js["videoId"];
       
        }

        public string vidId { get; set; }
    }
    public class JsonResult
    {
        public JsonResult(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jParse = jObject["parse"];
            title = (string)jParse["title"];
            JToken jText = jParse["text"];
            html = (string)jText["*"];
        }

        public string title { get; set; }

        public string html { get; set; }
    }

    public class DispItem : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public string Title
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        private string _Link1;
        public string Link1
        {
            get
            {
                return this._Link1;
            }
            set
            {
                this._Link1 = value;
                NotifyPropertyChanged("Link1");
            }
        }
        public string Link2
        {
            get;
            set;
        }
        public string Link3
        {
            get;
            set;
        }
    }

    public class WikiResultViewModel
    {
        public static ObservableCollection<DispItem> di { get; set; }

        public WikiResultViewModel(string url)
        
        {
            Debug.WriteLine("initiating sequence for " + url);
            di = new ObservableCollection<DispItem>();
            //di.Add(new DispItem() { Title = "", Text = "" });
           fetchRest(url);
            //fetchRestYoutube(url,0);
        }

        public async void fetchRest(string url)
        {
            JsonResult jR;
            url = @"http://en.wikipedia.org/w/api.php?format=json&action=parse&prop=text&page="+url;
            string responseText = "";
            int respCode = 0;
            try
            {

                // used to build entire input
                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                // prepare the web page we will be asking for
                System.Net.Http.HttpClient searchClient;
                searchClient = new System.Net.Http.HttpClient();
                //searchClient.MaxResponseContentBufferSize = 256000;
                System.Net.Http.HttpResponseMessage response = await searchClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                jR = new JsonResult(responseText);
            }
            catch (System.Threading.Tasks.TaskCanceledException tcex)
            {
                Debug.WriteLine("TASK CANCEL EXCEPTION" + tcex);
                throw tcex;
            }
            catch (System.Net.Http.HttpRequestException httpex)
            {
                Debug.WriteLine("HTTP EXCEPTION" + httpex);
                throw httpex;
            }
            catch (WebException e)
            {
                string text = string.Empty;
                string outRespType = string.Empty;
                if (e.Response != null)
                {
                    using (WebResponse response = e.Response)
                    {
                        outRespType = response.ContentType;
                        HttpWebResponse exceptionResponse = (HttpWebResponse)response;
                        respCode = (int)exceptionResponse.StatusCode;

                        using (System.IO.Stream data = response.GetResponseStream())
                        {
                            text = new System.IO.StreamReader(data).ReadToEnd();
                        };
                    };
                }
                throw e;
            }

            catch (Exception ex)
            {
                throw ex;
            }
            //string heading, heading_text;
            //List<string> sub_heading, sub_text;
            //heading = findheading(responseText);
            //heading_text = findheading_text(responseText);
            //Debug.WriteLine(heading);
            //heading =ReadLines(responseText).Skip(x-1).Take(x).First();

            //Debug.WriteLine(""+responseText);
            if (jR.html != null && jR.title != null) FindWiki(jR.title, jR.html);
            else Debug.WriteLine("NULL IN JSON RESPONSE");
            populateYouTube();
            //di[1].Link1 = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";
            //Find(responseText);
        }

        private void populateYouTube()
        {
            
            for (int i = di.Count-1; i >=0;i-- )
            {

                fetchRestYoutube(di[0].Title + ' ' + di[i].Title, i);
                //d.Link1 = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";
            }
            //fetchRestYoutube(di[0].Title, 0);

        }

        string a = "", b = "", c = "";
        public async void fetchRestYoutube(string url,int i)
        {
            url = @"https://www.googleapis.com/youtube/v3/search?type=video&part=snippet&q="+ url.Replace("_","%20") + @"&maxResults=1&videoEmbeddable=true&key=AIzaSyCRwECOq2X6FXqSfboGYfBPUNNKLah_SQY";
            string responseText = "";
            JsonYoutube jYT;
            int respCode = 0;
            try
            {
                // prepare the web page we will be asking for
                System.Net.Http.HttpClient searchClient;
                searchClient = new System.Net.Http.HttpClient();
                //searchClient.MaxResponseContentBufferSize = 256000;
                System.Net.Http.HttpResponseMessage response = await searchClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                jYT = new JsonYoutube(responseText);
            }
            catch (System.Threading.Tasks.TaskCanceledException tcex)
            {
                Debug.WriteLine("TASK CANCEL EXCEPTION" + tcex);
                throw tcex;
            }
            catch (System.Net.Http.HttpRequestException httpex)
            {
                Debug.WriteLine("HTTP EXCEPTION" + httpex);
                throw httpex;
            }
            catch (WebException e)
            {
                string text = string.Empty;
                string outRespType = string.Empty;
                if (e.Response != null)
                {
                    using (WebResponse response = e.Response)
                    {
                        outRespType = response.ContentType;
                        HttpWebResponse exceptionResponse = (HttpWebResponse)response;
                        respCode = (int)exceptionResponse.StatusCode;

                        using (System.IO.Stream data = response.GetResponseStream())
                        {
                            text = new System.IO.StreamReader(data).ReadToEnd();
                        };
                    };
                }
                throw e;
            }

            catch (Exception ex)
            {
                throw ex;
            }
            di[i].Link1 = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + jYT.vidId + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";
        }

        public List<LinkItem> Find(string file)
        {
            //a = ""; b = ""; c = "";
            List<LinkItem> list = new List<LinkItem>();
            int count = 0;
            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(<a href=\""/watch.*?\"">)",
                RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                LinkItem i = new LinkItem();

                // 3.
                // Get href attribute.
                //Match m2 = Regex.Match(value, @"href=\""(/watch.*?)\""",
                //RegexOptions.Singleline);
                //if (m2.Success)
                //{
                //    i.Href = m2.Groups[1].Value; count++;
                //}

                // 4.
                // Remove inner tags from text.
                count++;
                string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                RegexOptions.Singleline);
                i.Text = t;
                //if(count==1) a = t;
                //if (count == 2) b = t;
                //if (count == 3) c = t;
                Debug.WriteLine(i.TextString());
                //list.Add(i);
                if (count > 2) break;
            }
            return list;
        }
        public ObservableCollection<DispItem> FindWiki(string title,string file)
        {
            //Debug.WriteLine(heading + '\n' + heading_text + '\n');
            List<LinkItem> list = new List<LinkItem>();
            DispItem d = new DispItem();
            // 1.
            // Find all matches in file.
            //MatchCollection m1 = Regex.Matches(file, @"(<h1 id=\""firstHeading\"".*?>.*?</h1>)",
            //    RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            //foreach (Match m in m1)
            //{
            //string value = m.Groups[1].Value;
            LinkItem i = new LinkItem();

            // 3.
            // Get href attribute.
            /*Match m2 = Regex.Match(file, @"(<h1 id=\""firstHeading\"".*?>.*?</h1>)",
            RegexOptions.Singleline);
            if (m2.Success)
            {
                i.Href = m2.Groups[1].Value;
            }

            // 4.
            // Remove inner tags from text.
            string t = Regex.Replace(i.Href, @"\s*<.*?>\s*", "",
            RegexOptions.Singleline);
            */
            d.Title = title;
            //Debug.WriteLine(d.Title);
            //list.Add(i);
            //}

            // 5.
            // Find all para matches in file.
            //MatchCollection m1 = Regex.Matches(file, @"(<p>.*?</p>.*?class=\""toc\"">)",
            //RegexOptions.Singleline);

            // 6.
            // Loop over each match.
            //foreach (Match m in m1)
            //{
            //string value = m.Groups[1].Value;
            //LinkItem i = new LinkItem();

            // 7.
            // Get href attribute.
            Match m2 = Regex.Match(file, @"(<p>.*?</p>)",
            RegexOptions.Singleline);
            if (m2.Success)
            {
                i.Href = m2.Groups[1].Value;
            }

            // 8.
            // Remove inner tags from text.
            string t = Regex.Replace(i.Href, @" <.*?>\s*", " ", RegexOptions.Singleline);
            t = Regex.Replace(t, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
            t = Regex.Replace(t, @"\[.*?\]", "", RegexOptions.Singleline);
            d.Text = t;
           // Debug.WriteLine(d.Text);
            //list.Add(i);

            //Call for youtube link here
            //fetchYouTube("https://www.youtube.com/results?search_query="+d.Title.Replace(' ','+'));
            //d.Link1 = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>"; 
            //d.Link2 = b;
            //d.Link3 = c;
            //
            di.Add(d);
            //}

            // 9.
            // Find all h2 para matches in file.
            //m1 = Regex.Matches(file, @"(<h2><span class=\""mw-headline\"".*?>.*?</span></h2>.*?<p>.*?</p>)",
            //    RegexOptions.Singleline);
            MatchCollection m1 = Regex.Matches(file, @"(<h2><span class=\""mw-headline\"".*?>.*?</p>)",
                RegexOptions.Singleline);

            // 10.
            // Loop over each match.
            int counter = 0;
            foreach (Match m in m1)
            {
                if (counter == 5)
                    break;
                else
                    counter++;
                string value = m.Groups[1].Value;
                //LinkItem i = new LinkItem();
               d = new DispItem();

                // 11.
                // Get href attribute.
                m2 = Regex.Match(value, @"(<h2>.*?</h2>)",
               RegexOptions.Singleline);
                if (m2.Success)
                {
                    i.Href = m2.Groups[1].Value;
                }

                // 12.
                // Remove inner tags from text.
                t = Regex.Replace(i.Href, @" <.*?>\s*", " ", RegexOptions.Singleline);
                t = Regex.Replace(t, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
                t = Regex.Replace(t, @"\[.*?\]", "", RegexOptions.Singleline);
                d.Title = t;
                //Debug.WriteLine(d.Title);
                if (d.Title.Equals("See also")) break;
                //list.Add(i);

                value = m.Groups[1].Value;
                LinkItem j = new LinkItem();
                m2 = Regex.Match(value, @"(<p>.*?</p>)",
                RegexOptions.Singleline);
                if (m2.Success)
                {
                    j.Href = m2.Groups[1].Value;
                }

                // 13.
                // Remove inner tags from text.
                t = Regex.Replace(j.Href, @" <.*?>\s*", " ", RegexOptions.Singleline);
                t = Regex.Replace(t, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
                t = Regex.Replace(t, @"\[.*?\]", "", RegexOptions.Singleline);
                d.Text = t;
               // Debug.WriteLine(d.Text);
                //call for youtube links here
                //fetchYouTube("https://www.youtube.com/results?search_query=" + di[0].Title.Replace(' ', '+') + '+' + d.Title.Replace(' ', '+'));
              //  d.Link1 = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>"; 
                //d.Link2 = b;
                //d.Link3 = c;
                di.Add(d);
            }
            foreach (DispItem das in di)
            {
                Debug.WriteLine("--" + das.Title + "\n");
            }
            return di;
        }

    }
}
