using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace FileUpload.Controllers
{
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IHostEnvironment env;

        public FileUploadController(IHostEnvironment env)
        {
            this.env = env;
        }

        [HttpPost("save-single")]
        public IActionResult SaveFile(IFormFile formFile)
        {
            // This PDF validation could be better but I'm keeping it simple for... simplicity ¯\_(ツ)_/¯
            if (formFile is null || formFile.ContentType != "application/pdf")
            {
                return BadRequest();
            }
;
            var rootPath = env.ContentRootPath;
            using 
            (
                var stream = new FileStream
                (
                    @$"{Path.Combine(
                        rootPath, "UploadedFiles", Guid.NewGuid().ToString()
                    )}.pdf", 
                    FileMode.CreateNew, FileAccess.Write
                )
            )
            {
                formFile.CopyToAsync(stream).Wait();
            }

            return Ok();
        }

        [HttpPost("post-multiple")]
        public IActionResult PostMultiple(IFormFile[] formFiles)
        {
            // Yeah I know but keep that word (simplicity) in mind ( ͡~ ͜ʖ ͡°)¯
            if (formFiles is null || formFiles.Any(x => x.ContentType != "application/pdf"))
            {
                return BadRequest();
            }

            var dir = env.ContentRootPath;
            foreach (var file in formFiles)
            {
                using (var stream = new FileStream(Path.Combine(dir, "MultipleUpload", $"{Guid.NewGuid().ToString()}.pdf"), FileMode.Create, FileAccess.Write))
                {
                    file.CopyToAsync(stream).Wait();
                }
            }

            return Ok();
        }
    }
}