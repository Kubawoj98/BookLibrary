using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Controllers;
using BookLibrary.Data;
using BookLibrary.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookLibrary.Tests
{
    [TestFixture]
    public class AuthControllerTests : IDisposable
    {
        private Mock<ApplicationDbContext> _contextMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BookLibraryTest")
                .Options;

            _contextMock = new Mock<ApplicationDbContext>(options);
            _controller = new AuthController(_contextMock.Object);
        }

        [TearDown]
        public void Dispose()
        {
            _contextMock.Object.Dispose();
        }

        [Test]
        public async Task Register_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("Email", "Required");

            var model = new RegisterViewModel
            {
                Email = "",
                Password = "Password123"
            };

            var result = await _controller.Register(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }
    }
}
