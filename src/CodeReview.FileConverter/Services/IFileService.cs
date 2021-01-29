using System.IO;
using System.Threading.Tasks;

namespace ReviewItEasy.FileConverter.Services
{
    public interface IFileService
    {
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        FileStream Create(string path);
        bool Exists(string path);
        FileStream OpenRead(string filePath);
    }
}