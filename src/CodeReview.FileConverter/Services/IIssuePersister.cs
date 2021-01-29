using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;

namespace GodelTech.CodeReview.FileConverter.Services
{
    public interface IIssuePersister
    {
        Task SaveAsync(string filePath, IEnumerable<Issue> issues);
    }
}