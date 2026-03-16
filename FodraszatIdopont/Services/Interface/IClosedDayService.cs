using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IClosedDayService
    {
        Task<Results<ClosedDay>> AddClosedDay(DateOnly closedday);
        Task<Results<DateOnly>> RemoveClosedDay(DateOnly closedday);
        Task<Results<List<ClosedDay>>> GetClosedDays();
        Task<Results<bool>> IsClosedDay(DateOnly closedday);
    }
}
