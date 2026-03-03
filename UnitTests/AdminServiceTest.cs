using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    internal class AdminServiceTest
    {
        [SetUp]
        public void Setup()
        {
            var _mockRepo = new Mock<IServiceRepository>();
            var _service = new AdminService(_mockRepo.Object);
        }
    }
}
