namespace QuickCartApi.Enums
{
    public enum OrderStatus
    {
        Pending, // 0
        Confirmed, // 1
        Processing,// 2
        Completed, // 3
        Cancelled // 4
    }

    public static class OrderStatusExtensions
    {
        private static readonly Dictionary<OrderStatus, OrderStatus[]> Transitions = new()
        {
            [OrderStatus.Pending] = [OrderStatus.Confirmed, OrderStatus.Cancelled],
            [OrderStatus.Confirmed] = [OrderStatus.Processing, OrderStatus.Cancelled],
            [OrderStatus.Processing] = [OrderStatus.Completed, OrderStatus.Cancelled],
            [OrderStatus.Completed] = [],
            [OrderStatus.Cancelled] = []
        };

        public static bool CanTransitionTo(this OrderStatus from, OrderStatus to) =>
            Transitions[from].Contains(to);
    }
}
