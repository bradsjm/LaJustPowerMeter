// <copyright file="GameSelectorService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Graphing.Entities;

    /// <summary>
    /// Retrieves Game Data
    /// </summary>
    public class GameHistoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameSelectorService"/> class.
        /// </summary>
        public GameHistoryService()
        {
        }

        /// <summary>
        /// Gets the available dates by parsing the filenames of each of data files (.dsx) files in the data directory.
        /// </summary>
        /// <returns>ReadOnlyCollection of available dates</returns>
        public ReadOnlyCollection<DateTime> GetAvailableDates()
        {
            List<DateTime> availableDates = new List<DateTime>();
            string dataDirectory = GetDataDirectory();

            if (Directory.Exists(dataDirectory))
            {
                foreach (string file in Directory.GetFiles(dataDirectory, "*.dsx", SearchOption.TopDirectoryOnly))
                {
                    DateTime parsedDate;
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (DateTime.TryParseExact(fileName, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        availableDates.Add(parsedDate);
                    }
                }
            }
            return availableDates.AsReadOnly();
        }

        /// <summary>
        /// Gets the data set for a particular date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public DataLogModel GetDataForDate(DateTime date)
        {
            DataLogModel data = new DataLogModel();
            string fileName = Path.Combine(GetDataDirectory(), date.ToString("yyyy-MM-dd") + ".dsx");
            if (File.Exists(fileName))
            {
                data.ReadXml(fileName, System.Data.XmlReadMode.IgnoreSchema);
                foreach (var row in data.Impacts.Where(row => row.Timestamp.Date != date).ToList()) data.Impacts.RemoveImpactsRow(row);
                foreach (var row in data.Scores.Where(row => row.Timestamp.Date != date).ToList()) data.Scores.RemoveScoresRow(row);
                data.AcceptChanges();
            }
            return data;
        }

        /// <summary>
        /// Gets the data directory.
        /// </summary>
        /// <returns></returns>
        private string GetDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LaJust");
        }
    }
}
