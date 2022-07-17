using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Contract
{
    public class Profile
    {
        public string ProfileName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string TagIndex { get; set; } = string.Empty;
    }
}