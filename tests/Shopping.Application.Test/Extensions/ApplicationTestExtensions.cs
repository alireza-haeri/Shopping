using Shopping.Application.Common;
using Xunit.Abstractions;

namespace Shopping.Application.Test.Extensions;

public static class ApplicationTestExtensions
{
    public static void WriteLineOperationResultErrors<TResult>(this ITestOutputHelper testOutputHelper,
        OperationResult<TResult> operationResult)
    {
        if (operationResult.ErrorMessages == null) return;
        foreach (var operationResultErrorMessage in operationResult.ErrorMessages)
        {
            testOutputHelper.WriteLine($"Property Name: {operationResultErrorMessage.Key} | Message: {operationResultErrorMessage.Value}");
        }
    }
}