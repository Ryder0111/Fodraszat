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

        public async Task<Results<ClosedDay>> AddClosedDay(DateOnly closedday)
        {
            if(closedday == DateOnly.MinValue)
                return Results<ClosedDay>.Fail("Adja meg a dátumot!");

            if(await _repo.ExistsByDate(closedday))
                return Results<ClosedDay>.Fail("Ezen a napon már zárva vagyunk!");

            if (closedday < DateOnly.FromDateTime(DateTime.Now.AddMonths(2)))
                return Results<ClosedDay>.Fail("A következő 2 hónapban még lehetnek foglalások, ezért erre az időszakra nem állítható be zárás.");

            ClosedDay closedDay = new ClosedDay
            {
                Date = closedday,
            };
            await _repo.Add(closedDay);
            return Results<ClosedDay>.Ok(closedDay);
        }

        public async Task<Results<List<ClosedDay>>> GetClosedDays()
        {
            var closeddays = await _repo.GettAll();

            if (!closeddays.Any())
                return Results<List<ClosedDay>>.Fail("Nincsenek napok mikor zárva vagyunk!");

            return Results<List<ClosedDay>>.Ok(closeddays);
        }

        public async Task<Results<bool>> IsClosedDay(DateOnly closedday)
        {
            if (closedday == DateOnly.MinValue)
                return Results<bool>.Fail("Adja meg a dátumot!");

            if (await _repo.ExistsByDate(closedday))
                return Results<bool>.Ok(true);

            return Results<bool>.Ok(false);
        }

        public async Task<Results<DateOnly>> RemoveClosedDay(DateOnly closedday)
        {   
            if (!await _repo.ExistsByDate(closedday))
                return Results<DateOnly>.Fail("Nincs zárt nap ezen a dátumon.");
            
            await _repo.DeleteByDate(closedday);
            return Results<DateOnly>.Ok(closedday);
        }
    }
}
