using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RLD.CPCore.Helpers
{
	public static class xlsx
	{
		public static List<string> GetAllLines(string filePath)
		{
			StringBuilder sb = new StringBuilder();
			List<string> lines = new List<string>();
			using (ExcelPackage package = new ExcelPackage(filePath))
			{
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
				ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

				//获取表格的列数和行数
				int rowCount = worksheet.Dimension.Rows;
				int colCount = worksheet.Dimension.Columns;
				for (int i = 1; i <= rowCount; i++)
				{
					for (int j = 1; j <= 4; j++)
					{
						// 具体的获取数据
						if (worksheet.Cells[i, j].Value != null)
						{
							sb.Append(worksheet.Cells[i, j].Value.ToString());
							sb.Append('	');
						}
					}
					lines.Add(sb.ToString());
					sb.Clear();
				}
			}
			return lines;
		}
		public static void WriteLines(string filePath, IList<string> lines)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using (ExcelPackage package = new ExcelPackage(filePath))
			{
				package.Workbook.Worksheets.Add("Sheet1");

				for (int j = 0; j < lines.Count; j++)
				{
					string line = lines[j];
					string[] contents = line.Split('	');
					if (contents.Length > 0)
					{
						for (int i = 1; i <= contents.Length; i++)
						{
							package.Workbook.Worksheets[0].Cells[j + 1, i].Value = contents[i - 1];
						}
					}
				}
				package.Save();
			}
		}
	}
}
