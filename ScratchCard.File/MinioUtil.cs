using MimeKit;
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

        //文件下载路径
        private static readonly string downloadFilePath = "path/to/your/downloaded/file";

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="objectName">文件唯一标识</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public async Task UploadFileAsync(string bucketName, FileInfo fileInfo)
        {
            CheckBucket(bucketName);

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                                              .WithBucket(bucketName)
                                              .WithObject(fileInfo.Name)
                                              .WithFileName(fileInfo.FullName)
                                              );

            Console.WriteLine($"文件 '{fileInfo.Name}' 上传到 bucket '{bucketName}' 中。");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="objectName">文件唯一标识</param>
        /// <returns></returns>
        public async Task DownloadFile(string bucketName, string objectName)
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
        private void CheckBucket(string bucketName)
        {
            bool found = BucketExists(bucketName);
            if (!found)
            {
                throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
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
        public bool BucketExists(string bucketName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                BucketExistsArgs args = new BucketExistsArgs()
                                                .WithBucket(bucketName);

                Task<bool> bucketExistTask = _minioClient.BucketExistsAsync(args);
                Task.WaitAll(bucketExistTask);
                return bucketExistTask.Result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
