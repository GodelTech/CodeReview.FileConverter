using System.Threading.Tasks;
using ReviewItEasy.FileConverter.Options;

namespace ReviewItEasy.FileConverter.Commands
{
    public interface IConvertReSharperCommand
    {
        Task<int> ExecuteAsync(ReSharperOptions options);
    }
}