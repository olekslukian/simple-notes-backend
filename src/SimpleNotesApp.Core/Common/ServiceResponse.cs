
using Microsoft.AspNetCore.Mvc;

namespace SimpleNotesApp.Core.Common;

public sealed class ServiceResponse<T>
{
  private readonly T? _data;
  private readonly string? _errorMessage;
  public bool IsSuccess { get; }

  private ServiceResponse(T? data)
  {
    _data = data;
    IsSuccess = true;
  }

  private ServiceResponse(string? errorMessage)
  {
    _errorMessage = errorMessage;
    IsSuccess = false;
  }

  public static ServiceResponse<T> Success(T data) => new(data);
  public static ServiceResponse<T> Failure(string? errorMessage) => new(errorMessage);

  public IActionResult When(Func<T, IActionResult> onSuccess, Func<string, IActionResult> onFailure) => IsSuccess && _data is not null ? onSuccess(_data) : onFailure(_errorMessage ?? "Something went wrong");
}
