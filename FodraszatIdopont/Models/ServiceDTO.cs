using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.ViewModels;

namespace FodraszatIdopont.Models
{
    public class ServiceDTO
    {
        public List<Service> Services { get; set; } = new();
        public ServiceViewModel NewService { get; set; } = new();
    }
}
