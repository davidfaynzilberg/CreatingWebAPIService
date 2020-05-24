using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreatingWebAPIService.Models
{
    public class RequestModel
    {
        public string SiteApiKey { get; set; }
        public string Ip { get; set; }
        public List<Country> WhiteList { get; set; }
    }
}