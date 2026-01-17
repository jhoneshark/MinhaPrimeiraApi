using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Domain.Interface;

public interface IApiLogService {
    Task SaveLogAsync(ApiResponseLog log);
}