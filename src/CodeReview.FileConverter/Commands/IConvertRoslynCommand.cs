using System.Threading.Tasks;
using ReviewItEasy.FileConverter.Options;

namespace ReviewItEasy.FileConverter.Commands
{
    public interface IConvertRoslynCommand
    {
        Task<int> ExecuteAsync(RoslynOptions options);
    }
}