namespace LaJust.PowerMeter.Common
{
    public enum PageNames
    {
        EventPage,
        ConfigPage,
        HistoryPage,
        RosterPage
    }

    public static class RegionNames
    {
        #region Shell Region Names

        public const string PrimaryPageRegion = "PrimaryPageRegion";
        public const string SecondaryPageRegion = "SecondaryPageRegion";
        public const string ToolbarLeftRegion = "ToolbarLeftRegion";
        public const string ToolbarRightRegion = "ToolbarRightRegion";
        public const string OverlayRegion = "OverlayRegion";
        public const string ReceiverStateRegion = "ReceiverStateRegion";

        #endregion

        #region Event Page Region Names

        public const string ClockRegion = "ClockRegion";
        public const string GameNumberRegion = "GameNumberRegion";
        public const string GameRoundRegion = "GameRoundRegion";
        public const string MeterRegion = "MeterRegion";

        #endregion

        #region Config Page Region Names

        public const string ConfigGeneralOptionsRegion = "ConfigGeneralOptionsRegion";

        #endregion

        #region Roster Page Region Names

        public const string RosterNamesRegion = "RosterNamesRegion";

        #endregion

        #region History Graph Region Names

        public const string HistoryGraphMain = "HistoryGraphMain";
        public const string HistoryGraphSelectors = "HistoryGraphSelectors";

        #endregion
    }
}
