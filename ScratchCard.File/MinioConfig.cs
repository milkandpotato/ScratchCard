using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScratchCard.File
{

    public class MinioConfig
    {
        private readonly IConfigurationSection _configurationSection;

        public MinioConfig(IConfiguration configuration)
        {
            _configurationSection = configuration.GetSection("MinioSettings");

            endpoint = _configurationSection.GetSection("endpoint").Value;
            accessKey = _configurationSection.GetSection("accessKey").Value;
            secretKey = _configurationSection.GetSection("secretKey").Value;
            secure = Boolean.Parse(_configurationSection.GetSection("secure").Value);
        }

        // MinIO 服务器地址
        private string endpoint;

        // 访问密钥
        private string accessKey;

        // 密钥
        private string secretKey;

        //bucket
        private string bucket = "public";

        //是否使用https
        private bool secure;

        //文件下载路径
        private string downloadFilePath = "path/to/your/downloaded/file";

        public string Endpoint { get => endpoint; set => endpoint = value; }

        public string AccessKey { get => accessKey; set => accessKey = value; }

        public string SecretKey { get => secretKey; set => secretKey = value; }

        public string Bucket { get => bucket; set => bucket = value; }

        public bool Secure { get => secure; set => secure = value; }

        public string DownloadFilePath { get => downloadFilePath; set => downloadFilePath = value; }
    }
}
