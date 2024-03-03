namespace AbbeyMortageAssessment.Services.Url
{
    using AbbeyMortageAssessment.Services.Common;
    using Microsoft.AspNetCore.Http;

    public interface IUrlService : IService
    {
        string GenerateReturnUrl(string path, HttpContext httpContext);
    }
}
