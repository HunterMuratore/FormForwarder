using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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

        [HttpGet]
        public IActionResult Get() {
            return Ok();
        }
        
        [HttpPost("{email}")]
        public IActionResult Post(string email)
        {
            var builder = new StringBuilder();
            var EMAIL_SUBJECT = "";
            var EMAIL_RECIP = "";

            if (!string.IsNullOrEmpty(email))
            {
                foreach (var key in HttpContext.Request.Form.Keys)
                {
                    var val = HttpContext.Request.Form[key];
                    if (key == "subject")
                    {
                        EMAIL_SUBJECT = val;
                    }
                    else if (key == "send-to")
                    {
                        EMAIL_RECIP = val;
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
                _smtpClient.Send(_email, EMAIL_RECIP, EMAIL_SUBJECT, builder.ToString());
                return Ok();
            } 
            else
            {
                return BadRequest();
            }
        }
    }
}