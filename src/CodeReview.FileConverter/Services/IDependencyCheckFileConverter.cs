using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IDependencyCheckFileConverter
    {
        Task<IEnumerable<Issue>> Convert(string filePath);
    }
}
