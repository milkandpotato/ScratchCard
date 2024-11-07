using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace ScratchCard
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Card card = new Card();
            int totalAwardNumber = 0;

            //获取长度
            card.Length = CheckUtils.GetNumber("请输入您的刮刮卡的长度");
            //获取宽度
            card.Width = CheckUtils.GetNumber("请输入您的刮刮卡的宽度");
            //获取奖项类型数量
            int awardType = CheckUtils.GetNumber("请输入您奖项类型的数量");
            //总格子数
            int totalCardCellNumber = card.Length * card.Width;

            for (int i = 0; i < awardType; i++)
            {
                string awardStr = "";
                do
                {
                    Console.WriteLine($"请输入奖项_{i}");
                    //获取奖项内容
                    awardStr = Console.ReadLine();
                    //判断该奖项是否存在
                    if (card.Awards.ContainsKey(awardStr))
                    {
                        Console.WriteLine("该奖项已存在，不可重复添加");
                    }
                    else
                    {
                        int awardNumber;
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
                                totalAwardNumber += awardNumber;
                                card.Awards.Add(awardStr, awardNumber);
                            }
                        } while (totalAwardNumber + awardNumber > totalCardCellNumber);
                    }
                } while (!card.Awards.ContainsKey(awardStr));
            }
            //获取生成的excel路径
            string filePath = CheckUtils.getFilePath();
            Console.WriteLine($"当前系统生成路径为:{filePath}");

            //创建一个新的excel文件，如果当前路径已经存在，则进行覆盖操作
            IWorkbook wb = new HSSFWorkbook();
            try
            {
                //设定要使用的Sheet为第0个Sheet
                ISheet TempSheet = wb.CreateSheet();
                //遍历宽度
                for (int i = 0; i < card.Width; i++)
                {
                    IRow row = TempSheet.CreateRow(i);
                    //遍历长度
                    for (int j = 0; j < card.Length; j++)
                    {
                        //设置边框
                        ICellStyle cellStyle = CheckUtils.GetCellStyle(wb);
                        row.CreateCell(j).CellStyle = cellStyle;
                    }
                }

                //设置数据
                foreach(string str in card.Awards.Keys)
                {
                    Random random = new Random();
                    
                    //获取奖品数量
                    int awardNumber;
                    card.Awards.TryGetValue(str, out awardNumber);

                    for(int i = 0; i < awardNumber; i++)
                    {
                        int awardX = random.Next(card.Length);
                        int awardY = random.Next(card.Width);

                        //设置奖品值的位置
                        TempSheet.GetRow(awardY).GetCell(awardX).SetCellValue(str);
                    }
                }

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    wb.Write(fs);//向打开的这个xls文件中写入数据
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
