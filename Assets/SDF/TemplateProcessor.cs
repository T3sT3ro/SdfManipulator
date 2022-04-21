using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SDF {
    public class TemplateProcessor {
        private KeyValuePair<string, string>[] parameters;
        private Type                           outputType;

        private static Regex paramRegex  = new Regex(@"_IN\s+(?<type>\w)\s+(?<param>\w)");
        private static Regex outputRegex = new Regex(@"_OUT\s+(?<type>\w)");

        public TemplateProcessor(string template) {
            // parameters = paramRegex.Matches(template).Select(match => KeyValuePair.Create(
            //     match.Groups["param"],
            //     match.Groups["type"])
            // ).ToArray();
            
        }
    }
}
