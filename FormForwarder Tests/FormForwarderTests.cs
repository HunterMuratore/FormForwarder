using FormForwarder.Controllers;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Moq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using System.Net;
using FormForwarder.Interfaces;
using FormForwarder.Services;

namespace FormForwarder_Tests
{
    public class FormForwarderTests
    {
        // Mocks
        private Mock<ISmtpClient> _mockSmtpClient;
        private Mock<IConfiguration> _mockConfiguration;

        private FormForwarderController _controller;
        private const string Email = "test@outlook.com";

        [SetUp]
        public void Init()
        {
            var mockLogger = new Mock<ILogger<FormForwarderController>>();
            _mockSmtpClient = new Mock<ISmtpClient>();

            _mockConfiguration = new Mock<IConfiguration>();            
            _mockConfiguration.Setup(s => s.GetSection("SMTP:Email").Value).Returns(Email);

            _controller = new FormForwarderController(mockLogger.Object, _mockSmtpClient.Object, _mockConfiguration.Object);
        }       

        [Test]
        public void Post_EmptyEmail_ReturnsBadRequest()
        {
            var email = "";

            var response = _controller.Post(email);

            Assert.NotNull(response);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void Post_NullEmail_ReturnsBadRequest()
        {
            var response = _controller.Post(null);

            Assert.NotNull(response);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [TestCase("huntergmail.com")]
        [TestCase("hunter@gmailcom")]
        [TestCase("@gmail.com")]
        [TestCase("hunter@")]
        [TestCase("hunter@.com")]
        [TestCase("hunter")]
        [TestCase("@.com")]
        [TestCase(".com")]
        [TestCase("@")]
        public void Post_InvalidEmail_ReturnsBadRequest(string email)
        {
            var response = _controller.Post(email);

            Assert.NotNull(response);
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void Post_SMTPException_ReturnsProblem()
        {
            _mockSmtpClient.Setup(s => s.Send(It.IsAny<MailMessage>())).Throws<SmtpException>();

            var response = _controller.Post(Email);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.NotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(((int)HttpStatusCode.InternalServerError)));
        }
    }
}