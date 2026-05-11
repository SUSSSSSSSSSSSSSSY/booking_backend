namespace Booking.Contracts.Common;

public class PaginationRequest
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public int Page { get; set; } = DefaultPage;

    public int PageSize { get; set; } = DefaultPageSize;

    public int Skip => (Page - 1) * PageSize;

    public void Normalize()
    {
        if (Page < 1)
        {
            Page = DefaultPage;
        }

        if (PageSize < 1)
        {
            PageSize = DefaultPageSize;
        }

        if (PageSize > MaxPageSize)
        {
            PageSize = MaxPageSize;
        }
    }
}