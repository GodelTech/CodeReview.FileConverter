using System.Collections.Generic;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IFileListResolver
    {
        IEnumerable<string> ResolveFiles(OptionsBase options);
    }
}