using Microsoft.AspNetCore.Mvc;
using Minio;
using ScratchCard.File;

namespace ScratchCard.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly MinioUtil _minioUtil;

        public FileController(IMinioClient minioClient)
        {
            _minioUtil = new(minioClient);
        }

        /// <summary>
        /// 上传文件到Minio
        /// </summary>
        /// <param name="bucketName">bucket</param>
        /// <param name="formFile">文件</param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadFile")]
        public async Task UploadFile(string bucketName, IFormFile formFile)
        {
            await _minioUtil.UploadFileAsync(bucketName, formFile.FileName, formFile.Length, formFile.OpenReadStream());
        }

        /// <summary>
        /// 从minio中删除文件
        /// </summary>
        /// <param name="bucketName">bucket</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteFile")]
        public async Task DeleteFile(string bucketName, string fileName)
        {
            await _minioUtil.DeleteFileAsync(bucketName, fileName);
        }
    }
}
