/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.DataGraph.Services
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using CarlosAg.ExcelXmlWriter;
    using LaJust.PowerMeter.Modules.DataGraph.Models;
    using System.Data;

    public class ExcelService
    {
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
                impacts.First().Name,
                impacts.First().GameNumber,
                impacts.First().RoundNumber);
            workbook.Save(Path.Combine(GetDataDirectory(), filename));
        }

        private string GetDataDirectory()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LaJust");
            Directory.CreateDirectory(dir);
            return dir;
        }

    }
}
