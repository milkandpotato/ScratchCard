﻿using System;
using System.Runtime.InteropServices;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace ScratchCard
{
    public class CheckUtils
    {
        public CheckUtils()
        {
        }

        //获取整数类型
        public static int GetNumber(string text)
        {
            int number;
            bool checkResult = false;
            do
            {
                Console.WriteLine(text);
                string str = Console.ReadLine();
                checkResult = int.TryParse(str, out number);
                if (checkResult)
                {
                    number = int.Parse(str);
                }
                else
                {
                    Console.WriteLine("请输入纯数字");
                }
            } while (!checkResult);

            return number;
        }

        //获取文件路径
        public static string getFilePath()
        {
            string filePath = "";
            string fileName = "ScratchCard.xls";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = $"C:\\Users\\{Environment.UserName}\\Downloads\\{fileName}";
                Console.WriteLine("Windows");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Linux");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = $"/Users/{Environment.UserName}/Downloads/{fileName}";
            }

            return filePath;
        }

        //获取excel文本框
        public static ICellStyle GetCellStyle(IWorkbook workbook)
        {
            //创建单元格样式
            ICellStyle cellStyle = workbook.CreateCellStyle();
            //设置为文本格式，也可以为 text，即 dataFormat.GetFormat("text");
            cellStyle.DataFormat = DateFormat.DEFAULT;
            cellStyle.BorderBottom = BorderStyle.Thin;    //下边框线
            cellStyle.BorderLeft = BorderStyle.Thin;      //左边框线
            cellStyle.BorderRight = BorderStyle.Thin;     //右边框线
            cellStyle.BorderTop = BorderStyle.Thin;       //上边框线

            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;

            return cellStyle;
        }

    }
}
