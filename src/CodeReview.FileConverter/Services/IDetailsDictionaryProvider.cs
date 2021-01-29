using System.Collections.Generic;

namespace ReviewItEasy.FileConverter.Services
{
    public interface IDetailsDictionaryProvider
    {
        IReadOnlyDictionary<string, DiagnosticDetails> GetDetails(string folderPath);
    }
}