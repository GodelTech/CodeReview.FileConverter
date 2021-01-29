using System.Collections.Generic;
using ReviewItEasy.FileConverter.Models;

namespace ReviewItEasy.FileConverter.Services
{
    public interface IRoslynIssueConverter
    {
        IEnumerable<Issue> Convert(string filePath, string srcFolderPrefix, IReadOnlyDictionary<string, DiagnosticDetails> diagnosticDetailsMap);
    }
}