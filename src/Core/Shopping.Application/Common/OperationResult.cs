namespace Shopping.Application.Common;

public interface IOperationResult
{
    bool IsSuccess { get; set; }
    bool IsNotFount { get; set; }
    List<KeyValuePair<string, string>> ErrorMessages { get; set; }
}

public class OperationResult<TResult> : IOperationResult
{
    public TResult? Result { get; set; }

    public bool IsSuccess { get; set; }
    public bool IsNotFount { get; set; }
    public List<KeyValuePair<string, string>>? ErrorMessages { get; set; }

    public static OperationResult<TResult> SuccessResult(TResult result)
    {
        return new OperationResult<TResult>()
        {
            IsSuccess = true,
            Result = result
        };
    }

    public static OperationResult<TResult> FailureResult(string propertyName, string message)
    {
        return new OperationResult<TResult>()
        {
            ErrorMessages = [new KeyValuePair<string, string>(propertyName, message)]
        };
    }
    
    public static OperationResult<TResult> FailureResult(List<KeyValuePair<string,string>> errorMessages)
    {
        return new OperationResult<TResult>()
        {
            ErrorMessages = errorMessages
        };
    }

    public static OperationResult<TResult> NotFoundResult()
    {
        return new OperationResult<TResult>()
        {
            IsNotFount = true
        };
    }
}