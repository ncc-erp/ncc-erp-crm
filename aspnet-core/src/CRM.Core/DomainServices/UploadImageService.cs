using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CRM.DomainServices
{
    public class UploadImageService : BaseDomainService, IUploadImage
    {
        private IHostingEnvironment _hostingEnvironment;

        public UploadImageService(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        public async Task<string> UploadImage(IFormFile file)
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "Image");
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(uploads, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                using (var fileSubStream = File.OpenRead(filePath))
                {
                    return $"/Image/{file.FileName}";
                }
            }
            return null;
        }
    }
}
