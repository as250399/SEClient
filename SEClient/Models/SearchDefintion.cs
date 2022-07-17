using SEClient.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Models
{
    public class SearchDefintion
    {
        public SearchTypeEnum SearchType { get; set; }
        public string GroupFiledId { get; set; }
    }
}