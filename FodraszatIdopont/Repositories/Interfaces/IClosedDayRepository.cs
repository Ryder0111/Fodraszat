using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Repositories.Interfaces
{
    public interface IClosedDayRepository
    {
        Task<ClosedDay> Add(DateOnly date);
        Task<ClosedDay> Remove(ClosedDay closedday);
        Task<bool> ExistsByDate(DateOnly date);
        Task<List<ClosedDay>> GettAll();

    }
}
