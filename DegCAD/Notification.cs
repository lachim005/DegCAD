using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public record Notification
    {
        public string GUID { get; set; }
        public string Title {get; set;}
        public string Body {get; set;}
        public string ButtonTitle {get; set;}
        public string ButtonLink {get; set;}
        public bool Seen {get; set;}

        public Notification(string guid, string title, string body, string buttonTitle, string buttonLink, bool seen)
        {
            GUID = guid;
            Title = title;
            Body = body;
            ButtonTitle = buttonTitle;
            ButtonLink = buttonLink;
            Seen = seen;
        }
    }

    
}
