namespace LaJust.PowerMeter.Modules.Roster.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using LaJust.PowerMeter.Modules.Roster.Models;
    using System.Collections.ObjectModel;

    public class RosterService
    {
        private ObservableCollection<Competitor> _competitors = new ObservableCollection<Competitor>();

        public ObservableCollection<Competitor> Competitors
        {
            get { return _competitors; }
        }

        public RosterService()
        {
            LoadCompetitors();
        }

        private string GetDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LaJust");
        }

        public void LoadCompetitors()
        {
            if (File.Exists(Path.Combine(GetDataDirectory(), "competitors.txt")))
            {
                _competitors.Clear();

                using (StreamReader reader = new StreamReader(Path.Combine(GetDataDirectory(), "competitors.txt")))
                {
                    while (!reader.EndOfStream)
                    {
                        _competitors.Add(new Competitor() { Name = reader.ReadLine() });
                    }
                }
            }
        }

        public void SaveCompetitors()
        {
            Directory.CreateDirectory(GetDataDirectory());
            using (StreamWriter writer = new StreamWriter(Path.Combine(GetDataDirectory(), "competitors.txt")))
            {
                foreach (Competitor competitor in _competitors)
                {
                    writer.WriteLine(competitor.Name);
                }
            }
        }
    }
}
