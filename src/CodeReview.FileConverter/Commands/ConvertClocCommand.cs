using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.FileConverter.Models;
using GodelTech.CodeReview.FileConverter.Options;

namespace GodelTech.CodeReview.FileConverter.Commands
{
    public class ConvertClocCommand : IConvertClocCommand
    {
        public async Task<int> ExecuteAsync(ClocOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            return Constants.SuccessExitCode;
        }
    }
}
