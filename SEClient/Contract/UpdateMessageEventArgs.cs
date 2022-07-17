using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Contract
{
    public class UpdateMessageEventArgs : EventArgs
    {
        public string RabbitMessage { get; set; }
    }
}