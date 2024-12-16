using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Runtime.InteropServices;
using ScratchCard.Model;
using NPOI.Util;

namespace ScratchCard.File
{
    public class FileUtil
    {

        /// <summary>
        /// 获取生成的文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetFilePath()
        {
            string filePath = "";
            string fileName = $"ScratchCard_{DateTime.Now.Ticks}.xls";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = $"C:\\Users\\{Environment.UserName}\\Downloads\\{fileName}";
                Console.WriteLine("Windows");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = $"/home/scratchcard/{fileName}";
                Console.WriteLine("Linux");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = $"/Users/{Environment.UserName}/Downloads/{fileName}";
            }

            return filePath;
        }

        /// <summary>
        /// 获取刮刮卡文件
        /// </summary>
        /// <returns></returns>
        public static FileInfo GetFile()
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

            FileInfo fileInfo = new FileInfo(filePath);

            return fileInfo;
        }

        /// <summary>
        /// 获取excel文本框
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static ICellStyle GetCellStyle(IWorkbook workbook)
        {
            //创建单元格样式
            ICellStyle cellStyle = workbook.CreateCellStyle();
            //设置为文本格式，也可以为 text，即 dataFormat.GetFormat("text");
            cellStyle.DataFormat = DateFormat.DEFAULT;

            //下边框线
            cellStyle.BorderBottom = BorderStyle.Thin;
            //左边框线
            cellStyle.BorderLeft = BorderStyle.Thin;
            //右边框线
            cellStyle.BorderRight = BorderStyle.Thin;
            //上边框线
            cellStyle.BorderTop = BorderStyle.Thin;

            //下边框线颜色
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            //左边框线颜色
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            //右边框线颜色
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            //上边框线颜色
            cellStyle.TopBorderColor = HSSFColor.Black.Index;

            //文本对齐方式为填充
            cellStyle.Alignment = HorizontalAlignment.Fill;

            return cellStyle;
        }

        /// <summary>
        /// 获取刮刮卡的文本框样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static ICellStyle GetCardCellStyle(IWorkbook workbook)
        {
            ICellStyle cellStyle = GetCellStyle(workbook);

            cellStyle.FillPattern = FillPattern.SolidForeground;
            //字体颜色
            IFont font = workbook.CreateFont();
            font.Color = IndexedColors.Grey50Percent.Index;
            cellStyle.SetFont(font);
            //单元格填充颜色
            cellStyle.FillForegroundColor = IndexedColors.Grey50Percent.Index;

            return cellStyle;
        }

        /// <summary>
        /// 生成刮刮卡的Excel文件
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static FileStream GenerateExcelFile(Card card)
        {
            string filePath = GetFilePath();

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);

            //创建一个新的excel文件，如果当前路径已经存在，则进行覆盖操作
            IWorkbook wb = new HSSFWorkbook();
            try
            {
                //生成sheet页
                ISheet TempSheet = wb.CreateSheet("ScratchCard");
                //获取答案文本样式
                ICellStyle answerStyle = GetCellStyle(wb);
                //获取刮刮卡文本样式
                ICellStyle cardStyle = GetCardCellStyle(wb);

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
                        //答案值的位置
                        int answerPositionX = position.PositionX + card.Length + 4;

                        //设置刮刮卡的值的位置
                        TempSheet.GetRow(position.PositionY).GetCell(position.PositionX).SetCellValue(award.Name);
                        //设置答案值的位置
                        TempSheet.GetRow(position.PositionY).GetCell(answerPositionX).SetCellValue(award.Name);
                    }
                }

                fs.Close();
                fs.Dispose();

                return fs;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
