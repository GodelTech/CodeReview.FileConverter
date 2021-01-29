using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewItEasy.FileConverter.Models;

namespace ReviewItEasy.FileConverter.Services
{
    public interface IIssuePersister
    {
        Task SaveAsync(string filePath, IEnumerable<Issue> issues);
    }
}