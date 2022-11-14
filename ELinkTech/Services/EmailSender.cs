using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

//*******************************************************************
//Author(s): Soyeong
//Date: 14 / 11 / 2022
//Perpose:
//Version: 1.0.0
//CopyRight ELinkTech & SoftWe're 2022 (c)
//********************************************************************

namespace ELinkTech.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration Configuration;
    private readonly ILogger _logger;

    public EmailSender(IConfiguration configuration,
                       ILogger<EmailSender> logger)
    {
        Configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Configuration["SendGrid:SendGridKey"]))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(Configuration["SendGrid:SendGridKey"], subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(Configuration["SendGrid:SenderEmail"], Configuration["SendGrid:SenderName"]),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode 
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }
}