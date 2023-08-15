using Abp.Authorization;
using Abp.UI;
using CRM.DomainServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Images
{
    [AbpAuthorize]
    public class ImageAppService : CRMAppServiceBase
    {
        private IHostingEnvironment _hostingEnvironment;

        public ImageAppService(IHostingEnvironment environment, UploadImageService imageService)
        {
            _hostingEnvironment = environment;
        }
        [HttpPost]
        public async Task<string> UploadImage(IFormFile file)
        {
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "image");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (file != null && file.Length > 0)
            {
                string fileName = file.FileName;
                string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                if (FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "jpg" || FileExtension == "gif")
                {

                    string imagePath = DateTimeOffset.Now.ToUnixTimeMilliseconds()+ "_" + AbpSession.UserId.Value + fileName;
                    using (var stream = System.IO.File.Create(Path.Combine(_hostingEnvironment.WebRootPath, "image", imagePath)))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return "/image/" + imagePath;
                }
                else
                {
                    throw new UserFriendlyException(String.Format("File can not upload!"));
                }
            }
            else
            {
                throw new UserFriendlyException(String.Format("No file upload!"));
            }
        }

    }
}
