using FormForwarder.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ISmtpClient _smtpClient;
        private readonly string _email;

        public FormForwarderController(ILogger<FormForwarderController> logger, ISmtpClient smtpClient, IConfiguration configuration)
        {
            _logger = logger;
            _smtpClient = smtpClient;
            _email = configuration.GetSection("SMTP:Email").Value;
        }

        [HttpPost("{email}")]
        public IActionResult Post(string email)
        {

            _logger.LogInformation($"Form forward request received for '{email}'.");
            var builder = new StringBuilder();
            var emailSubject = "";
            var successPage = "";
            var message = "";

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("Must enter an email address.");
                return BadRequest("Must enter an email address.");
            }

            var emailMatch = Regex.Match(email, EMAIL_VALIDATE_REGEX, RegexOptions.IgnoreCase);                 // Verify email is in proper email form
            if (!emailMatch.Success) {
                _logger.LogError("Must enter an email address in valid email address form (your@email.com).");
                return BadRequest("Must enter an email address in valid email address form (your@email.com).");
            }

            var form = HttpContext?.Request?.Form;
            if (form == null)
            {
                return BadRequest("Must submit form data.");
            }

            foreach (var key in form.Keys)
            {
                var val = form[key];
                if (key.ToLower() == "subject")
                {
                    emailSubject = val;
                }
                else if (string.IsNullOrEmpty(val.ToString()))
                {
                    builder.AppendLine("The user did not provide a " + key + "<br>");
                }
                else if (key.ToLower() == "_next") {
                    successPage = val;
                }
                else if (key.ToLower() == "message") {
                    message = val.ToString();  
                }
                else
                {
                    builder.AppendLine("<b>" + key + ":</b> " + val.ToString() + "<br>");
                }
            }

            if (string.IsNullOrEmpty(emailSubject))
            {
                emailSubject = EMAIL_SUBJECT;
            }

            // Provide a default message if user does not send a message
            if (message == "")
            {
                builder.AppendLine("<b>Message:</b><br>");
                builder.AppendLine("The user did provide provide a message.");
            } else
            {
                builder.AppendLine("<b>Message:</b><br>");
                builder.AppendLine(message);
            }

            _logger.LogInformation($"Sent to: {email}");
            _logger.LogInformation(builder.ToString());

            try
            { 
                var mailMessage = new MailMessage(_email, email, emailSubject, builder.ToString())
                {
                    IsBodyHtml = true,              // Needed to use formatting in body of email
                    BodyEncoding = Encoding.UTF8
                };
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Problem();
            }

            _logger.LogInformation("Returned OK");

            // Redirects to location received in form submitted
            if (!string.IsNullOrEmpty(successPage)) {
                return Redirect(successPage);
            } else {
                return Ok();
            }
        }
    }
}