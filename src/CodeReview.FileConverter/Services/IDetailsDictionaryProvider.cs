using System.Collections.Generic;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IDetailsDictionaryProvider
    {
        IReadOnlyDictionary<string, DiagnosticDetails> GetDetails(string folderPath);
    }
}