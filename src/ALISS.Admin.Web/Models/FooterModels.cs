using System.Collections.Generic;

namespace ALISS.Admin.Web.Models
{
    public class FooterModel
    {
        public string Copyright { get; set; }
        public List<FooterColumn> Columns { get; set; }

        public FooterModel()
        {
            Columns = new List<FooterColumn>();
        }
    }

    public class FooterColumn
    {
        public string Title { get; set; }
        public List<FooterLink> Links { get; set; }

        public FooterColumn()
        {
            Links = new List<FooterLink>();
        }
    }

    public class FooterLink
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }
}