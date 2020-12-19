using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServerlessApp.Models;

namespace ePaymentsApp
{
    public static class SendEmail
    {
        [FunctionName("SendEmail")]
        public static void Run([BlobTrigger("licenses/{name}.lic")]CloudBlockBlob licenseBlob, 
            string name, 
            [Table("payments", "stripe", "{name}")] Payment payment,
            [SendGrid] out SendGridMessage message,
            ILogger log)
        {
            log.LogInformation($"SendEmail Blob trigger function processing blob: {name}");
            var apiKey = Environment.GetEnvironmentVariable("AzureWebJobsSendGridApiKey");
            var client = new SendGridClient(apiKey);
            message = new SendGridMessage();
            message.AddTo(System.Environment.GetEnvironmentVariable("EmailRecipient", EnvironmentVariableTarget.Process));
            message.AddContent("text/html", $"Download your license <a href='{licenseBlob.Uri.AbsoluteUri}' alt='license link'>here</a>");
            message.SetFrom(new EmailAddress("anand.vaibhav@hotmail.com"));
            message.SetSubject("Your payment has been completed");
            client.SendEmailAsync(message).GetAwaiter().GetResult();
            
        }
    }
}
