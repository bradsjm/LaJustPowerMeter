namespace LaJust.PowerMeter.Modules.Roster.Models
{
    using System;
    using LaJust.PowerMeter.Common.BaseClasses;

    public class Competitor : PropertyNotifier
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
    }

}
