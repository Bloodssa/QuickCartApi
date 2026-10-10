namespace QuickCartApi.Common
{
    // Use Service Result Rather Than Returning A Tupple in a Services
    public class ServiceResult
    {
        public ErrorKind Kind { get; init; }
        public string? Error { get; init; }
        public bool Success => Kind == ErrorKind.None; // default error false
    }

    // GENERIC SERVICE RESULT
    public class ServiceResult<T> : ServiceResult
    {
        public T? Value { get; init; }
    }
}
