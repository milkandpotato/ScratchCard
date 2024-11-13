using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ScratchCard.Model;

namespace ScratchCard
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Card card = new Card();
            Random random = new Random();
            int totalAwardNumber = 0;

            //获取长度
            card.Length = CheckUtils.GetNumber("请输入您的刮刮卡的长度");
            //获取宽度
            card.Width = CheckUtils.GetNumber("请输入您的刮刮卡的宽度");
            //获取奖项类型数量
            int awardType = CheckUtils.GetNumber("请输入您奖项类型的数量");
            //总格子数
            int totalCardCellNumber = card.Length * card.Width;

            #region 生成数据
            for (int i = 0; i < awardType; i++)
            {
                Award award = new Award();
                award.Length = card.Length;
                award.Width = card.Width;

                int awardNumber;

                do
                {
                    Console.WriteLine($"请输入奖项_{i + 1}");
                    //获取奖项内容
                    string awardStr = Console.ReadLine();
                    //判断输入是否为空字符串
                    if (String.IsNullOrEmpty(awardStr))
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

                do
                {
                    //获取奖项数量
                    awardNumber = CheckUtils.GetNumber("请输入该奖项的数量");

                    //判断添加的奖品数量是否大于总的格子数量

                    if (totalAwardNumber + awardNumber > totalCardCellNumber)
                    {
                        Console.WriteLine($"奖品数量不可超过总格子数！总格子数：{totalCardCellNumber},当前奖品数量:{totalAwardNumber}");
                    }
                    else
                    {
                        award.Number = awardNumber;
                        totalAwardNumber += awardNumber;
                    }
                } while (totalAwardNumber + awardNumber > totalCardCellNumber);

                card.Awards.Add(award);
            }
            #endregion

            //获取生成的excel路径
            string filePath = CheckUtils.getFilePath();
            Console.WriteLine($"当前系统生成路径为:{filePath}");

            //创建一个新的excel文件，如果当前路径已经存在，则进行覆盖操作
            IWorkbook wb = new HSSFWorkbook();
            try
            {
                //生成sheet页
                ISheet TempSheet = wb.CreateSheet("ScratchCard");
                //获取答案文本样式
                ICellStyle answerStyle = CheckUtils.GetCellStyle(wb);
                //获取刮刮卡文本样式
                ICellStyle cardStyle = CheckUtils.GetCardCellStyle(wb);

                //遍历宽度
                for (int i = 0; i < card.Width; i++)
                {
                    IRow row = TempSheet.CreateRow(i);
                    //遍历长度
                    for (int j = 0; j < card.Length; j++)
                    {
                        //刮刮卡单元格
                        ICell cardCell = row.CreateCell(j);
                        //答案单元格
                        ICell answerCell = row.CreateCell(j + card.Length + 4);

                        //设置刮刮卡文本样式
                        cardCell.CellStyle = cardStyle;
                        //设置答案文本样式
                        answerCell.CellStyle = answerStyle;
                    }
                }

                //设置数据
                foreach (Award award in card.Awards)
                {
                    List<AwardPosition> positions = award.AwardPositions;
                    foreach (AwardPosition position in positions)
                    {
                        //设置刮刮卡的值的位置
                        TempSheet.GetRow(position.PositionY).GetCell(position.PositionX).SetCellValue(award.Name);
                        //设置答案值的位置
                        TempSheet.GetRow(position.PositionY).GetCell(position.PositionX + card.Length + 3).SetCellValue(award.Name);
                    }
                }

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    //向打开的这个xls文件中写入数据
                    wb.Write(fs);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
