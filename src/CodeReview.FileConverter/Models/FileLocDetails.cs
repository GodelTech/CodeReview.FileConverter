namespace GodelTech.CodeReview.FileConverter.Models
{
    public class FileLocDetails
    {
        public string Language { get; set; }

        public long Blank { get; set; }
        public long Code { get; set; }
        public long Commented { get; set; }
    }
}
