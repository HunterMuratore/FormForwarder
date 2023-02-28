using Microsoft.AspNetCore.HttpLogging;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var smtpHost = builder.Configuration.GetSection("SMTP:Host").Value;
var smtpPort = int.Parse(builder.Configuration.GetSection("SMTP:Port").Value);
var smtpEmail = builder.Configuration.GetSection("SMTP:Email").Value;
var smtpPassword = builder.Configuration.GetSection("SMTP:Password").Value;
var port = System.Environment.GetEnvironmentVariable("PORT");

var smtpClient = new SmtpClient(smtpHost, smtpPort)
{
    Credentials = new NetworkCredential(smtpEmail, smtpPassword),
    EnableSsl = true
};
builder.Services.AddSingleton(smtpClient);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

if (string.IsNullOrEmpty(port)) {
    app.Run("http://0.0.0.0:7070");
} else {
    app.Run($"http://0.0.0.0:{port}");
}