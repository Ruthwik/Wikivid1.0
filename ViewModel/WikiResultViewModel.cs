using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikivid1._0.Model;

namespace Wikivid1._0.ViewModel
{
    public class WikiResultViewModel
    {
        //ObservableCollection<SubContent> _SubContent = new ObservableCollection<SubContent>();

        public ObservableCollection<SubContent> _SubContent
        {
            get;
            set;
        }

      
        public string html { get; set; }
        public WikiResultViewModel()
        {
            
            html = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>";

            _SubContent = new ObservableCollection<SubContent>();

            _SubContent.Add(new SubContent(){
                heading = "india",
                text = "this is lots of text",
                youtubePath = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>"
            });
            _SubContent.Add(new SubContent()
            {
                heading = "subcontinent",
                text = "this is lots of text again",
                youtubePath = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>"
            });
            _SubContent.Add(new SubContent()
            {
                heading = "history",
                text = "this is lots of textyet again",
                youtubePath = @"<iframe width=""640"" height=""390"" src=""http://www.youtube.com/embed/" + "ooDrCr-8ALI" + @"?rel=0"" frameborder=""0"" allowfullscreen></iframe>"
            });
        }
    }
}
