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
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<IServiceRepository> _mockServiceRepo;
        private AdminService _service;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockServiceRepo = new Mock<IServiceRepository>();

            _service = new AdminService(
                _mockUserRepo.Object,
                _mockServiceRepo.Object
            );
        }

        [Test]
        public async Task Hibas_Input_Name_error()
        {
            var ujszolgaltatas = new Service
            {
                Name = "",
                DurationInMinute = 30,
                Price = 3000
            };

            var result = await _service.CreateService( ujszolgaltatas );

            Assert.That( result.Success, Is.False);

            _mockServiceRepo.Verify(c => c.Create(It.IsAny<Service>()), Times.Never);
        }

        [Test]
        public async Task Existing_Name_error()
        {
            var ujszolgaltatas = new Service
            {
                Name = "teszt",
                DurationInMinute = 30,
                Price = 3000
            };

            _mockServiceRepo.Create(ujszolgaltatas);
        }

    }
}

