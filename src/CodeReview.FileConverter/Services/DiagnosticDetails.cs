using System;

namespace ReviewItEasy.FileConverter.Services
{
    public class DiagnosticDetails
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string HelpLinkUri { get; set; }
        public string[] CustomTags { get; set; } = Array.Empty<string>();
    }
}