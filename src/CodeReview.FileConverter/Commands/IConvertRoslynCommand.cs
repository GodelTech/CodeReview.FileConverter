using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public interface IConvertRoslynCommand
    {
        Task<int> ExecuteAsync(RoslynOptions options);
    }
}