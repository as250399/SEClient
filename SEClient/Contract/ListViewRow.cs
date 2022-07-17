using SEClient.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Contract
{
    public class ListViewRow
    {
        public SearchTypeEnum SearchType { get; set; }
        public string TransactionId { get; set; }
        public string BusinessUnit { get; set; }
        public string DateTime { get; set; }
        public string POS { get; set; }
        public string Tlog { get; set; }
        public string TotalHits { get; set; }
        public string GroupValue { get; set; }
        public string GroupFiledId { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public TotalTransactions Totaltransactions { get; set; }
    }
}