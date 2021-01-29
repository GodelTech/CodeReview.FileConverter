using System.Collections.Generic;
using ReviewItEasy.FileConverter.Options;

namespace ReviewItEasy.FileConverter.Services
{
    public interface IFileListResolver
    {
        IEnumerable<string> ResolveFiles(OptionsBase options);
    }
}