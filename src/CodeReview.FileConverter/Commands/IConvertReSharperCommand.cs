using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public interface IConvertReSharperCommand
    {
        Task<int> ExecuteAsync(ReSharperOptions options);
    }
}