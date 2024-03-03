namespace AbbeyMortageAssessment.Services.Stream
{
    using AbbeyMortageAssessment.Services.Common;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading.Tasks;

    public interface IStreamService : IService
    {
        Task<MemoryStream> CopyFileToMemoryStreamAsync(IFormFile file);
    }
}
