using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Repositories.Interfaces
{
    public interface IClosedDayRepository
    {
        Task<ClosedDay> Add(ClosedDay date);
        Task<DateOnly> DeleteByDate(DateOnly closedday);
        Task<bool> ExistsByDate(DateOnly date);
        Task<List<ClosedDay>> GettAll();

    }
}
