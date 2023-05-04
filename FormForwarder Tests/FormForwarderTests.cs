using FormForwarder.Controllers;

namespace FormForwarder_Tests
{
    public class FormForwarderTests
    {
        private FormForwarderController _postRequest;

        [SetUp]
        public void Setup()
        {
            _postRequest = new FormForwarderController();
        }

        [Test]
        public async void Post_NullEmail_ReturnsBadRequestEmailNull()
        {
            var email = "";

            var response = _postRequest(email);

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
            Assert.AreEqual("Must enter an email address.", response.Message);
        }

        [TestCase("huntergmail.com")]
        [TestCase("hunter@gmailcom")]
        [TestCase("@gmail.com")]
        [TestCase("hunter@")]
        [TestCase("hunter@.com")]
        public async void Post_InvalidEmail_ReturnsBadRequestEmailInvalid(string email)
        {
            var response = _postRequest(email);

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
            Assert.AreEqual("Must enter an email address in valid email address form (your@email.com).", response.Message);
        }
    }
}