using NPOI.HSSF.UserModel;
using ScratchCard.File;
using ScratchCard.Model;

namespace ScratchCard.Tests;

public class UnitTest1
{
    [Fact]
    public void GenerateExcelFile_WritesNonEmptyFile_AndSheetsExist()
    {
        Card card = new Card { Length = 3, Width = 2, AwardTypes = 1 };

        Award award = new Award { Length = card.Length, Width = card.Width, Name = "A" };
        award.Number = 1;
        award.AwardPositions.Clear();
        award.AwardPositions.Add(new AwardPosition { PositionX = 1, PositionY = 0 });
        card.Awards.Add(award);

        string filePath = Path.Combine(Path.GetTempPath(), $"scratchcard_{Guid.NewGuid():N}.xls");
        FileUtil.GenerateExcelFile(card, filePath);

        FileInfo fileInfo = new FileInfo(filePath);
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length > 0);

        using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        HSSFWorkbook workbook = new HSSFWorkbook(fs);
        Assert.NotNull(workbook.GetSheet("Question"));
        Assert.NotNull(workbook.GetSheet("Answer"));
    }

    [Fact]
    public void GenerateExcelFile_WritesAwardName_ToExpectedCells()
    {
        Card card = new Card { Length = 4, Width = 3, AwardTypes = 2 };

        Award award1 = new Award { Length = card.Length, Width = card.Width, Name = "一等奖" };
        award1.Number = 1;
        award1.AwardPositions.Clear();
        award1.AwardPositions.Add(new AwardPosition { PositionX = 2, PositionY = 1 });

        Award award2 = new Award { Length = card.Length, Width = card.Width, Name = "谢谢参与" };
        award2.Number = 1;
        award2.AwardPositions.Clear();
        award2.AwardPositions.Add(new AwardPosition { PositionX = 0, PositionY = 0 });

        card.Awards.Add(award1);
        card.Awards.Add(award2);

        string filePath = Path.Combine(Path.GetTempPath(), $"scratchcard_{Guid.NewGuid():N}.xls");
        FileUtil.GenerateExcelFile(card, filePath);

        using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        HSSFWorkbook workbook = new HSSFWorkbook(fs);
        var question = workbook.GetSheet("Question");
        var answer = workbook.GetSheet("Answer");

        Assert.Equal("谢谢参与", question.GetRow(0).GetCell(0).StringCellValue);
        Assert.Equal("谢谢参与", answer.GetRow(0).GetCell(0).StringCellValue);

        Assert.Equal("一等奖", question.GetRow(1).GetCell(2).StringCellValue);
        Assert.Equal("一等奖", answer.GetRow(1).GetCell(2).StringCellValue);
    }
}