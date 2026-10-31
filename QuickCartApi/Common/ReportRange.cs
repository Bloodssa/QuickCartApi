namespace QuickCartApi.Common
{
    public readonly record struct ReportRange(DateOnly From, DateOnly To)
    {
        private static readonly TimeSpan PhOffset = TimeSpan.FromHours(8); // Philippines: UTC+8, no DST

        public DateTime FromUtc => From.ToDateTime(TimeOnly.MinValue) - PhOffset;
        public DateTime ToUtcExclusive => To.AddDays(1).ToDateTime(TimeOnly.MinValue) - PhOffset;
        public int Days => To.DayNumber - From.DayNumber + 1;
    }
}
