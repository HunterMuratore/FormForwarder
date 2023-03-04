using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace FormForwarder.Controllers
{
    [ApiController]
    [Route("")]
    public class FormForwarderController : ControllerBase
    {
        private const string EMAIL_VALIDATE_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const string EMAIL_SUBJECT = "Form Submitted";

        private readonly ILogger<FormForwarderController> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly string _email;

        public FormForwarderController(ILogger<FormForwarderController> logger, SmtpClient smtpClient, IConfiguration configuration)
        {
            _logger = logger;
            _smtpClient = smtpClient;
            _email = configuration.GetSection("SMTP:Email").Value;
        }

        [HttpPost("{email}")]
        public IActionResult Post(string email)
        {

            _logger.LogInformation(email);
            var builder = new StringBuilder();
            var emailSubject = "";
            var successPage = "";

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("Must enter an email address.");
                return BadRequest("Must enter an email address.");
            }

            var emailMatch = Regex.Match(email, EMAIL_VALIDATE_REGEX, RegexOptions.IgnoreCase);
            if (!emailMatch.Success) {
                _logger.LogError("Must enter an email address in valid email address form (your@email.com).");
                return BadRequest("Must enter an email address in valid email address form (your@email.com).");
            }

            foreach (var key in HttpContext.Request.Form.Keys)
            {
                var val = HttpContext.Request.Form[key];
                if (key == "subject")
                {
                    emailSubject = val;
                    _logger.LogInformation(val);
                }
                else if (string.IsNullOrEmpty(val.ToString()))
                {
                    builder.AppendLine("The user did not provide a " + key);
                    builder.AppendLine();
                }
                else if (key == "_next") {
                    successPage = val;
                }
                else
                {
                    builder.AppendLine(key + ":");
                    builder.AppendLine();
                    builder.AppendLine(val.ToString());
                    builder.AppendLine();
                    _logger.LogInformation(val);
                }
            }

            if (string.IsNullOrEmpty(emailSubject))
            {
                emailSubject = EMAIL_SUBJECT;
                _logger.LogInformation(emailSubject);
            }

            try
            {
                _smtpClient.Send(_email, email, emailSubject, builder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Problem();
            }

            _logger.LogInformation("Returned OK");
            if (!string.IsNullOrEmpty(successPage)) {
                return Redirect(successPage);
            } else {
                return Ok();
            }
        }
    }
}