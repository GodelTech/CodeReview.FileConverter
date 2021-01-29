using System.Collections.Generic;
using ReviewItEasy.FileConverter.Models;

namespace ReviewItEasy.FileConverter.Services
{
    public interface IReSharperFileConverter
    {
        IEnumerable<Issue> Convert(string filePath);
    }
}