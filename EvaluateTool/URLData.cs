using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace TestProject.EvaluateTool
{
    public class URLData
    {
        [ScriptIgnore]
        public string Host { get; set;}

        public string Link { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        [ScriptIgnore]
        public bool IsRead { get; set; } = false;

        public string Error { get; set; }
    }
}