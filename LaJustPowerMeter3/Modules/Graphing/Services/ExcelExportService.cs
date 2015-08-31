// <copyright file="GameDataLoggerService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using CarlosAg.ExcelXmlWriter;
    using Graphing.Entities;

    /// <summary>
    /// Excel Exporting Service
    /// </summary>
    public class ExcelExportService
    {
        /// <summary>
        /// Exports the specified impacts to Excel worksheet.
        /// </summary>
        /// <param name="impacts">The impacts.</param>
        public void Export(EnumerableRowCollection<DataLogModel.ImpactsRow> impacts)
        {
            Workbook workbook = new Workbook();
            workbook.Properties.Title = "LaJust Sports Export";
            workbook.Properties.Created = DateTime.Now;

            Worksheet impactSheet = workbook.Worksheets.Add("Impacts");
            WorksheetRow row = impactSheet.Table.Rows.Add();
            row.Cells.Add(new WorksheetCell("Sequence", DataType.String));
            row.Cells.Add(new WorksheetCell("Time", DataType.String));
            row.Cells.Add(new WorksheetCell("Device ID", DataType.String));
            row.Cells.Add(new WorksheetCell("Game Number", DataType.String));
            row.Cells.Add(new WorksheetCell("Round Number", DataType.String));
            row.Cells.Add(new WorksheetCell("Seconds", DataType.String));
            row.Cells.Add(new WorksheetCell("Competitor", DataType.String));
            row.Cells.Add(new WorksheetCell("Data Source", DataType.String));
            row.Cells.Add(new WorksheetCell("Required Impact", DataType.String));
            row.Cells.Add(new WorksheetCell("Actual Impact", DataType.String));
            row.Cells.Add(new WorksheetCell("Panel", DataType.String));
            
            foreach (var impactRow in impacts)
            {
                row = impactSheet.Table.Rows.Add();
                row.Cells.Add(new WorksheetCell(impactRow.ID.ToString(), DataType.Number));
                row.Cells.Add(new WorksheetCell(impactRow.Timestamp.ToString("G"), DataType.String));
                row.Cells.Add(new WorksheetCell(impactRow.SensorId.ToString(), DataType.String));
                row.Cells.Add(new WorksheetCell(impactRow.GameNumber.ToString(), DataType.Number));
                row.Cells.Add(new WorksheetCell(impactRow.RoundNumber.ToString(), DataType.Number));
                row.Cells.Add(new WorksheetCell(impactRow.ElapsedTime.TotalSeconds.ToString(), DataType.Number));
                row.Cells.Add(new WorksheetCell(impactRow.Name.ToString(), DataType.String));
                row.Cells.Add(new WorksheetCell(impactRow.DataSource.ToString(), DataType.String));
                row.Cells.Add(new WorksheetCell(impactRow.RequiredLevel.ToString(), DataType.Number));
                row.Cells.Add(new WorksheetCell(impactRow.ImpactLevel.ToString(), DataType.Number));
                row.Cells.Add(new WorksheetCell(impactRow.SensorPanel.ToString(), DataType.String));
            }

            string filename = string.Format("{0} {1} G{2:D3}-{3}R.xls", 
                impacts.First().Timestamp.ToString("yyyy-MM-dd"), 
                impacts.First().Name.Trim(),
                impacts.First().GameNumber,
                impacts.First().RoundNumber);

            workbook.Save(Path.Combine(GetDataDirectory(), filename));
        }

        /// <summary>
        /// Gets the data directory.
        /// </summary>
        /// <returns></returns>
        private string GetDataDirectory()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LaJust");
            Directory.CreateDirectory(dir);
            return dir;
        }

    }
}
