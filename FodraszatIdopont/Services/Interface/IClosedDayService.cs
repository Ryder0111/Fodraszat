using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IClosedDayService
    {
        Task<Results<ClosedDay>> AddClosedDay(ClosedDay closedday);
        Task<Results<ClosedDay>> RemoveClosedDay(int id);
        Task<Results<List<ClosedDay>>> GetClosedDays();
        Task<Results<bool>> IsClosedDay(DateOnly closedday);
    }
}
