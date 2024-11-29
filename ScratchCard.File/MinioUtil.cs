using Minio;
using Minio.DataModel.Args;

namespace ScratchCard.File
{
    public class MinioUtil
    {
        private readonly IMinioClient _minioClient;

        public MinioUtil(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="objectName">文件唯一标识</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public async Task UploadFileAsync(string bucketName, string fileName, string filePath)
        {
            await CheckBucket(bucketName);

            string objectName = GenerateFileName(fileName);

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                                              .WithBucket(bucketName)
                                              .WithObject(objectName)
                                              .WithFileName(filePath));

            Console.WriteLine($"文件 '{fileName}' 上传到 bucket '{bucketName}' 中。");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public async Task UploadFileAsync(string bucketName, string fileName, long fileSize, Stream fileStream)
        {
            await CheckBucket(bucketName);

            string objectName = GenerateFileName(fileName);

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                                              .WithBucket(bucketName)
                                              .WithObject(objectName)
                                              .WithObjectSize(fileSize)
                                              .WithStreamData(fileStream));

            Console.WriteLine($"文件 '{fileName}' 上传到 bucket '{bucketName}' 中。");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="objectName">文件唯一标识</param>
        /// <param name="downloadFilePath">文件下载路径</param>
        /// <returns></returns>
        public async Task DownloadFile(string bucketName, string objectName, string downloadFilePath)
        {
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                                              .WithBucket(bucketName)
                                              .WithObject(objectName)
                                              .WithFile(downloadFilePath));
            Console.WriteLine($"文件 '{objectName}' 从 bucket '{bucketName}' 下载到 '{downloadFilePath}'。");
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="objectName">文件唯一标识</param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string bucketName, string objectName)
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                                                .WithBucket(bucketName)
                                                .WithObject(objectName));
            Console.WriteLine($"文件 '{objectName}' 从 bucket '{bucketName}' 中删除。");
        }

        /// <summary>
        /// 校验是否存在,如果不存在则报错
        /// <example>
        /// 调用示例
        /// <code>
        /// bool exists = MinioHelper.BucketExists(minio, buckName);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="minio"></param>
        /// <param name="bucketName"></param>
        /// <exception cref="Exception"></exception>
        private async Task CheckBucket(string bucketName)
        {
            bool found = await BucketExists(bucketName);
            //如果不存在则创建bucket
            if (!found)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs()
                    .WithBucket(bucketName));
            }
        }

        /// <summary>
        /// 检查存储桶是否存在
        /// </summary>
        /// <example>
        /// <code>
        /// var data = MinioHelper.ListBuckets(minio);
        /// </code>
        /// </example>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        private async Task<bool> BucketExists(string bucketName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                BucketExistsArgs args = new BucketExistsArgs()
                                                .WithBucket(bucketName);

                bool bucketExistTask = await _minioClient.BucketExistsAsync(args);
                return bucketExistTask;
            }
            catch (Minio.Exceptions.AccessDeniedException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 生成唯一文件名
        /// </summary>
        /// <param name="fileFullName">文件名称("xxxxx.pdf")</param>
        /// <returns></returns>
        private string GenerateFileName(string fileFullName)
        {
            //文件名
            string filename = Path.GetFileNameWithoutExtension(fileFullName);

            //扩展名
            string extension = Path.GetExtension(fileFullName);

            return $"{filename}_{DateTime.Now.Ticks}{extension}";
        }
    }
}
