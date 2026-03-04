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
        public class CreateServiceTests 
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
            public async Task CreateService_ShouldFail_WhenNameIsEmpty()
            {
                var result = await _service.CreateService(
                    new Service
                    {
                        Name = "",
                        DurationInMinute = 30,
                        Price = 3000
                    });

                Assert.That(result.Success, Is.False);

                _mockServiceRepo.Verify(c => c.Create(It.IsAny<Service>()), Times.Never);
            }

            [Test]
            public async Task CreateService_ShouldFail_WhenNameAlreadyExists()
            {
                _mockServiceRepo.Setup(s => s.ExistsByName("teszt")).ReturnsAsync(true);

                var result = await _service.CreateService(
                    new Service
                    {
                        Name = "teszt",
                        DurationInMinute = 30,
                        Price = 3000
                    });

                Assert.That(result.Success, Is.False);

                _mockServiceRepo.Verify(e => e.ExistsByName("teszt"), Times.Once);
                _mockServiceRepo.Verify(e => e.Create(It.IsAny<Service>()), Times.Never);
            }

            [Test]
            public async Task CreateService_ShouldCreateService_WhenNameIsUnique()
            {
                _mockServiceRepo.Setup(s => s.ExistsByName("teszt")).ReturnsAsync(false);

                var result = await _service.CreateService(
                    new Service
                    {
                        Name = "teszt",
                        DurationInMinute = 30,
                        Price = 3000
                    });

                Assert.That(result.Success, Is.True);

                _mockServiceRepo.Verify(e => e.ExistsByName("teszt"), Times.Once);
                _mockServiceRepo.Verify(e => e.Create(It.IsAny<Service>()), Times.Once);
            }

            [Test]
            public void CreateService_ShouldThrowException_WhenRepositoryFails()
            {
                _mockServiceRepo.Setup(s => s.ExistsByName("teszt")).ReturnsAsync(false);

                _mockServiceRepo
                    .Setup(s => s.Create(It.IsAny<Service>()))
                    .ThrowsAsync(new Exception("DB hiba"));

                Assert.ThrowsAsync<Exception>(async () =>
                    await _service.CreateService(new Service
                    {
                        Name = "teszt",
                        DurationInMinute = 30,
                        Price = 3000
                    })
                );
            }
        }   

        public class PromoteToHairdresserTests
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
            public async Task PromoteToHairdresser_ShouldFail_WhenUserDoesNotExist()
            {
                _mockUserRepo.Setup(u => u.GetUserByEamil("teszt")).ReturnsAsync((User?)null);

                var result = await _service.PromoteToHairdresser("teszt");

                Assert.That(result.Success, Is.False);

                _mockUserRepo.Verify(u => u.Update(It.IsAny<User>()), Times.Never);
            }

            [Test]
            public async Task PromoteToHairdresser_ShouldFail_WhenUserAlreadyHairdresser()
            {
                _mockUserRepo.Setup(u => u.GetUserByEamil("teszt")).ReturnsAsync(
                    new User { 
                        Role = FodraszatIdopont.Models.Enums.UserRole.Hairdresser,
                        Email = "teszt@gmail.com"});

                var result = await _service.PromoteToHairdresser("teszt");

                Assert.That(result.Success, Is.False);

                _mockUserRepo.Verify(u => u.Update(It.IsAny<User>()), Times.Never);
            }

            [Test]
            public async Task ShouldPromoteUser_WhenUserIsValid()
            {
                _mockUserRepo.Setup(u => u.GetUserByEamil("teszt")).ReturnsAsync(new User { Role = FodraszatIdopont.Models.Enums.UserRole.User});

                var result = await _service.PromoteToHairdresser("teszt");

                Assert.That(result.Success, Is.True);

                _mockUserRepo.Verify(u => u.Update(It.IsAny<User>()), Times.Once);
            }
        }

    }
}

