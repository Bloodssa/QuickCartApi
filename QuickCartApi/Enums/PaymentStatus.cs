namespace QuickCartApi.Enums
{
    public enum PaymentStatus
    {
        Pending, // 0
        Paid, // 1
        Failed // 2
    }

    public static class PaymentStatusExtensions
    {
        private static readonly Dictionary<PaymentStatus, PaymentStatus[]> Transitions = new()
        {
            [PaymentStatus.Pending] = [PaymentStatus.Paid, PaymentStatus.Failed],
            [PaymentStatus.Failed] = [PaymentStatus.Pending, PaymentStatus.Paid],   // a failed payment can be retried
            [PaymentStatus.Paid] = []
        };

        public static bool CanTransitionTo(this PaymentStatus from, PaymentStatus to) =>
            Transitions[from].Contains(to);
    }
}
