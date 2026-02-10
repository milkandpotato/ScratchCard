using Minio;
using ScratchCard.File;
using ScratchCard.Model;

namespace ScratchCard
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //初始化minio
            IMinioClient client = new MinioClient()
                .WithEndpoint("47.99.75.217:9000")
                .WithCredentials("HDkFf2iLw5vO69M0rat7", "VA4d2m6sm4qqlQFRL9R3vhTg3nZtS2NfwLxSHD0C")
                .WithSSL(false)
                .Build();

            //初始化卡片
            Card card = new Card();
            //初始化随机数
            Random random = new Random();

            MinioUtil minioUtil = new MinioUtil(client);
            int totalAwardNumber = 0;

            //获取长度
            card.Length = CheckUtils.GetNumber("请输入您的刮刮卡的长度");
            //获取宽度
            card.Width = CheckUtils.GetNumber("请输入您的刮刮卡的宽度");
            //总格子数
            int totalCardCellNumber = card.Length * card.Width;
            do
            {
                //获取奖项类型数量
                card.AwardTypes = CheckUtils.GetNumber("请输入您奖项类型的数量");
                if (card.AwardTypes > totalCardCellNumber)
                {
                    Console.WriteLine($"奖项数量不可大于总格子数!总格子数为:{totalCardCellNumber}");
                }
            } while (card.AwardTypes > totalCardCellNumber);


            #region 生成数据
            for (int i = 0; i < card.AwardTypes; i++)
            {
                Award award = new Award();
                award.Length = card.Length;
                award.Width = card.Width;

                int awardNumber;

                do
                {
                    Console.WriteLine($"请输入奖项_{i + 1}");
                    //获取奖项内容
                    string? awardStr = Console.ReadLine();
                    //判断输入是否为空字符串
                    if (String.IsNullOrWhiteSpace(awardStr))
                    {
                        Console.WriteLine("不可输入空字符串");
                        continue;
                    }


                    //判断该奖项是否存在
                    bool hasAward = card.Awards.Where(award => award.Name.Equals(awardStr)).Any();
                    if (hasAward)
                    {
                        Console.WriteLine("该奖项已存在，不可重复添加");
                        continue;
                    }
                    else
                    {
                        award.Name = awardStr;
                    }
                }
                while (String.IsNullOrEmpty(award.Name));

                while (true)
                {
                    //获取奖项数量
                    awardNumber = CheckUtils.GetNumber("请输入该奖项的数量");

                    //判断添加的奖品数量是否大于总的格子数量
                    if (totalAwardNumber + awardNumber > totalCardCellNumber)
                    {
                        Console.WriteLine($"奖品数量不可超过总格子数！总格子数：{totalCardCellNumber},期望奖品数量:{totalAwardNumber + awardNumber}");
                        continue;
                    }

                    award.Number = awardNumber;
                    totalAwardNumber += awardNumber;
                    break;
                }

                card.Awards.Add(award);
            }
            #endregion

            AssignUniquePositions(card, random);

            //获取生成的excel路径
            string filePath = FileUtil.GetFilePath();
            Console.WriteLine($"当前系统生成路径为:{filePath}");

            //生成xls文件
            FileUtil.GenerateExcelFile(card, filePath);

            //上传minio
            Task task = minioUtil.UploadFileAsync(Environment.UserName, filePath);
            task.Wait();
        }

        private static void AssignUniquePositions(Card card, Random random)
        {
            int totalCells = card.Length * card.Width;
            int totalAwards = card.Awards.Sum(a => a.Number);
            if (totalAwards > totalCells)
            {
                throw new InvalidOperationException($"奖品数量不可超过总格子数！总格子数：{totalCells},奖品数量：{totalAwards}");
            }

            var allCells = new List<(int X, int Y)>(totalCells);
            for (int y = 0; y < card.Width; y++)
            {
                for (int x = 0; x < card.Length; x++)
                {
                    allCells.Add((x, y));
                }
            }

            for (int i = allCells.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (allCells[i], allCells[j]) = (allCells[j], allCells[i]);
            }

            int index = 0;
            foreach (Award award in card.Awards)
            {
                award.AwardPositions.Clear();
                for (int k = 0; k < award.Number; k++)
                {
                    var (x, y) = allCells[index++];
                    award.AwardPositions.Add(new AwardPosition { PositionX = x, PositionY = y });
                }
            }
        }
    }
}
