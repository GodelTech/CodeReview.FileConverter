using System.Collections.Generic;
using GodelTech.CodeReview.FileConverter.Models;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IReSharperFileConverter
    {
        IEnumerable<Issue> Convert(string filePath);
    }
}