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
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Frameworks;

namespace FormForwarder_Tests
{
    public class FormForwarderTests
    {

        // Mocks
        private Mock<ISmtpClient> _mockSmtpClient;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<FormForwarderController>> _mockLogger;
        private Mock<HttpContext> _moqContext;
        private Mock<HttpRequest> _moqRequest;
        private FormCollection _formValues;
        private FormFeature _form;
        private FeatureCollection _features;
        private FormForwarderController _controller;
        private const string Email = "test@outlook.com";
        private const string SuccessPage = "success.html";
        private Dictionary<string, StringValues> Values = new Dictionary<string, StringValues>(){
                {"FirstName", "Captain"},
                {"LastName", "Falcon"},
                {"Email", Email},
                {"Message", "Test message"},
                {"_next", SuccessPage }
        };

        private void SetForm(Dictionary<string, StringValues>? values)
        {
            _formValues = new FormCollection(values);
            _form = new FormFeature(_formValues);

            _features = new FeatureCollection();
            _features.Set<IFormFeature>(_form);

            if (values == null)
            {
                _moqRequest.Setup(r => r.Form).Returns((IFormCollection)null);
            }
            else
            {
                _moqRequest.Setup(r => r.Form).Returns(_formValues);
            }

            var context = new DefaultHttpContext(_features);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = context;
        }

        [SetUp]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<FormForwarderController>>();
            _mockSmtpClient = new Mock<ISmtpClient>();
            _mockConfiguration = new Mock<IConfiguration>();

            _moqContext = new Mock<HttpContext>();
            _moqRequest = new Mock<HttpRequest>();
            _moqContext.Setup(x => x.Request).Returns(_moqRequest.Object);

            _mockConfiguration.Setup(s => s.GetSection("SMTP:Email").Value).Returns(Email);

            _controller = new FormForwarderController(_mockLogger.Object, _mockSmtpClient.Object, _mockConfiguration.Object);
        }

        [Test]
        public void MoqFormsTest()
        {
            SetForm(Values);

            var form = _moqContext.Object.Request.Form;

            Assert.IsNotNull(form);
            Assert.That(form["FirstName"], Is.EqualTo("Captain"));
            Assert.That(form["LastName"], Is.EqualTo("Falcon"));
            Assert.That(form["Email"], Is.EqualTo(Email));
            Assert.That(form["Message"], Is.EqualTo("Test message"));
            Assert.That(form["_next"], Is.EqualTo(SuccessPage));
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
            SetForm(Values);
            _mockSmtpClient.Setup(s => s.Send(It.IsAny<MailMessage>())).Throws<SmtpException>();

            var form = _moqContext.Object.Request.Form;
            Assert.IsNotNull(form);

            var response = _controller.Post(Email);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.NotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public void Post_FormDataNull_ReturnsBadRequest()
        {
            SetForm(null);

            var form = _moqContext.Object.Request.Form;
            Assert.IsNull(form);
        }

        [Test]
        public void Post_MessageSentAndRedirectPage_ReturnsRedirect()
        {
            SetForm(Values);

            var form = _moqContext.Object.Request.Form;
            Assert.IsNotNull(form);

            var response = _controller.Post(Email);
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<RedirectResult>(response);
            var objectResult = response as RedirectResult;
            Assert.NotNull(objectResult);
            Assert.IsInstanceOf<RedirectResult>(objectResult);
        }

        [Test]
        public void Post_MessageSent_ReturnsOK()
        {
            var emptyFormValues = new Dictionary<string, StringValues>();
            SetForm(emptyFormValues);

            var form = _moqContext.Object.Request.Form;
            Assert.IsNotNull(form);

            var response = _controller.Post(Email);
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<OkResult>(response);
            var objectResult = response as OkResult;
            Assert.NotNull(objectResult);
            Assert.IsInstanceOf<OkResult>(objectResult);
        }
    }
}