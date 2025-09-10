
using Microsoft.AspNetCore.Mvc;

namespace SimpleNotesApp.Core.Common;

public sealed class ServiceResponse<T>
{
  private readonly T? _data;
  private readonly List<Error> _errors;
  public bool IsSuccess { get; }

  private ServiceResponse(T? data)
  {
    _data = data;
    _errors = [];
    IsSuccess = true;
  }

  private ServiceResponse(List<Error> errors)
  {
    _errors = errors;
    IsSuccess = false;
  }

  private ServiceResponse(Error error)
  {
    _errors = [error];
    IsSuccess = false;
  }

  public static ServiceResponse<T> Success(T data) => new(data);

  public static ServiceResponse<T> Failure(Error error) => new(error);
  public IActionResult When(Func<T, IActionResult> onSuccess, Func<List<Error>, IActionResult> onFailure) => IsSuccess && _data is not null ? onSuccess(_data) : onFailure(_errors);
}
