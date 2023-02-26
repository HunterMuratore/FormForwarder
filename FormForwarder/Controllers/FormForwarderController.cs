using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text;

namespace FormForwarder.Controllers
{
    [ApiController]
    [Route("")]
    public class FormForwarderController : ControllerBase
    {
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
            var builder = new StringBuilder();
            var emailSubject = "";

            if (!string.IsNullOrEmpty(email))
            {
                foreach (var key in HttpContext.Request.Form.Keys)
                {
                    var val = HttpContext.Request.Form[key];
                    if (key == "subject")
                    {
                        emailSubject = val;
                    }
                    else if (string.IsNullOrEmpty(val.ToString()))
                    {
                        builder.AppendLine("The user did not provide a " + key);
                        builder.AppendLine();
                    }
                    else
                    {
                        builder.AppendLine(key + ":");
                        builder.AppendLine();
                        builder.AppendLine(val.ToString());
                        builder.AppendLine();
                    }
                }

                if (emailSubject == string.Empty)
                {
                    emailSubject = EMAIL_SUBJECT;
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
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}