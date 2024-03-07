namespace Kidstarter.Api.Models
{
    public record ErrorResponse<TError>(TError Error, int ErrorCode);
}