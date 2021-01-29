using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ReviewItEasy.FileConverter.Models
{
    public class Issue
    {
        public long Id { get; set; }
        public string RuleId { get; set; } // S123
        
        [JsonConverter(typeof(StringEnumConverter))]
        public IssueLevel Level { get; set; } // Error, Warning, Info, None
        public string Title { get; set; } // Methods and properties should be named in PascalCase 
        public string Message { get; set; } // Add a new line at the end of the file 'IAzureDevOpsClient.cs'. 
        public string Description { get; set; } // Shared naming conventions allow teams to collaborate efficiently. This rule checks whether or not method and property names are PascalCased. To reduce noise, two consecutive upper case characters are allowed unless they form the whole name. So, MyXMethod is compliant, but XM on its own is not. 
        public string DetailsUrl { get; set; } // https://rules.sonarsource.com/csharp/RSPEC-100
        public string Category { get; set; } // Minor Code Smell 
        public string[] Tags { get; set; } // C#, MainSourceScope, TestSourceScope 
        public IssueLocation[] Locations { get; set; }
    }
}
