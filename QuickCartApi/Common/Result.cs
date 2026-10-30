namespace QuickCartApi.Common
{
    public class Result
    {
        public static ServiceResult Ok() => new();
        public static ServiceResult<T> Ok<T>(T value) => new() { Value = value };
        public static ServiceResult Fail(ErrorKind kind, string error) => new() { Kind = kind, Error = error };
        public static ServiceResult<T> Fail<T>(ErrorKind kind, string error) => new() { Kind = kind, Error = error };
        public static ServiceResult<T> Fail<T>(ErrorKind kind,string error, Dictionary<string, string[]> errors) => new()
        {
            Kind = kind,
            Error = error,
            Errors = errors
        };
    }
}
