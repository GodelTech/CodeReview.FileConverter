using System.Collections.Generic;
using GodelTech.CodeReview.FileConverter.Models;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IRoslynIssueConverter
    {
        IEnumerable<Issue> Convert(string filePath, string srcFolderPrefix, IReadOnlyDictionary<string, DiagnosticDetails> diagnosticDetailsMap);
    }
}