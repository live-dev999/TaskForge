namespace TaskForge.Application.Core
{
    public class Result<T>
    {
        public bool IsSeccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }

        public static Result<T> Success(T value) =>
            new Result<T> { IsSeccess = true, Value = value };

        public static Result<T> Failure(string error) =>
            new Result<T> { IsSeccess = false, Error = error };
    }
}
