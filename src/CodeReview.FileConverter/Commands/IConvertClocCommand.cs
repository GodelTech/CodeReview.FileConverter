using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public interface IConvertClocCommand
    {
        Task<int> ExecuteAsync(ClocOptions options);
    }
}