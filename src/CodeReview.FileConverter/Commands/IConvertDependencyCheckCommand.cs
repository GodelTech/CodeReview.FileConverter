using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public interface IConvertDependencyCheckCommand
    {
        Task<int> ExecuteAsync(DependencyCheckOptions options);
    }
}
