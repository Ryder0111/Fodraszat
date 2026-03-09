using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.Services
{
    public class ClosedDayService : IClosedDayService
    {
        private readonly IClosedDayRepository _repo;

        public ClosedDayService(IClosedDayRepository repo)
        {
            _repo = repo;
        }

        public Task<Results<ClosedDay>> AddClosedDay(ClosedDay closedday)
        {
            throw new NotImplementedException();
        }

        public Task<Results<List<ClosedDay>>> GetClosedDays()
        {
            throw new NotImplementedException();
        }

        public Task<Results<bool>> IsClosedDay(DateOnly closedday)
        {
            throw new NotImplementedException();
        }

        public Task<Results<ClosedDay>> RemoveClosedDay(int id)
        {
            throw new NotImplementedException();
        }
    }
}
