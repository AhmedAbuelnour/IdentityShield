using IdentityShield.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Polly;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;

namespace IdentityShield.Infrastructure.Implementations;

public class NotificationService(IHttpClientFactory factory, IConfiguration configuration) : INotificationService
{
    private readonly HttpClient _SMSClient = factory.CreateClient("SMSClient");
    private readonly IConfiguration _configuration = configuration;

    public Task<bool> SendEmailAsync<TUser>(TUser user, string mailSubject, string token, CancellationToken cancellationToken) where TUser : IdentityUser
    {
        return SendEmailAsync(user.UserName, user.Email, mailSubject, token, cancellationToken);
    }

    public async Task<bool> SendEmailAsync(string userName, string email, string mailSubject, string token, CancellationToken cancellationToken)
    {
        MailMessage emailMessage = new MailMessage()
        {
            From = new MailAddress(_configuration["MailFrom"], _configuration["DisplayName"], Encoding.UTF8),
            Subject = mailSubject,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Priority = MailPriority.High,
            Body = GenerateHtmlEmailBody(userName, token)
        };

        emailMessage.To.Add(email);

        SmtpClient client = new()
        {
            Credentials = new NetworkCredential(_configuration["MailFrom"], _configuration["Password"]),
            Port = 587,
            Host = _configuration["Host"],
            EnableSsl = true
        };

        return await Policy.Handle<Exception>().RetryAsync(5).ExecuteAsync(async () =>
        {
            await client.SendMailAsync(emailMessage, cancellationToken);

            return true;
        });
    }

    public Task<bool> SendSMSAsync<TUser>(TUser user, string token, CancellationToken cancellationToken) where TUser : IdentityUser
    {
        return SendSMSAsync(user.PhoneNumber, token, cancellationToken);
    }

    public Task<bool> SendSMSAsync(string phoneNumber, string token, CancellationToken cancellationToken)
    {
        return Policy.HandleResult(false).RetryAsync(5).ExecuteAsync(async () =>
        {
            HttpResponseMessage httpResponseMessage = await _SMSClient.PostAsJsonAsync("/api/OTP", new
            {
                Mobile = phoneNumber,
                OTP = token,
                UserName = _configuration["SMSSettings:UserName"],
                Password = _configuration["SMSSettings:Password"],
                Sender = _configuration["SMSSettings:Sender"],
                Template = _configuration["SMSSettings:Template"],
                Environment = 1
            }, cancellationToken);

            return httpResponseMessage.IsSuccessStatusCode;
        });
    }

    private string GenerateHtmlEmailBody(string username, string otp)
        => string.Format("""
            <body style="font: small/1.5 monospace; direction: rtl;">
              <table style="max-width:720px; width:100%; border-spacing:0; border-collapse:collapse; margin:auto;">
                <tr>
                  <td valign="top">
                    <table style="width:90%; margin:auto; min-height:230px; border:1px solid #f3f3f3; padding:0; background-color:#fff;">
                      <tr>
                        <td style="padding:10px;" valign="top" align="center">
                          <center>
                            <table style="width:100%; margin:auto; min-height:230px; border:0; padding-bottom:0; background-color:#fff;">
                              <tr>
                                <td style="padding:0;" valign="top" align="center">
                                  <table width="100%" style="border-spacing:0; border-collapse:collapse; margin:auto;">
                                    <tr>
                                      <td style="padding:0;" valign="top" align="center">
                                        <a>
                                          <img style="display:block; border-radius:4px; padding:0 0 10px 0;" width="85" height="85" src="https://www.selaheltelmeez.com/assets/images/landing/ic_on.png" alt="">
                                        </a>
                                        <p style="line-height:24px; padding:0 0 5px; font-size:24px; font-weight:bold; margin:0;">مرحباً {0}</p>
                                        <p style="margin:0; padding:0 0 5px; font-size:18px;">الخطوة الاخيرة لبدء رحلتك التعليمية<br>أدخل الرمز الآتي على منصة سلاح التلميذ</p>
                                      </td>
                                    </tr>
                                  </table>
                                  <table width="100%" style="border-spacing:0; border-collapse:collapse; margin:auto;">
                                    <tr>
                                      <td align="center">
                                        <a href="{1}" style="background-color:#4b4c4e; width:200px; font-size:19px; text-decoration:none; border:0; border-radius:3px; color:#fff; display:inline-block; line-height:26px; padding:10px; margin:5px 0 10px 0; text-align:center;" target="_blank">{1}</a>
                                      </td>
                                    </tr>
                                  </table>
                                  <div style="height:5px;"></div>
                                  <table width="100%" style="border-spacing:0; border-collapse:collapse; margin:auto;">
                                    <tr align="center">
                                      <td align="center">
                                        <p style="margin:0; padding:5px 0; font-size:18px;">ملاحظة: إذا لم تكن قد تقدّمت بهذا الطلب، فلا تحتاج إلى فعل أي شيء، ولن تتلقى منا أي رسائل أخرى. هذه رسالة آلية، يرجى عدم الرد على هذه الرسالة. لمزيد من الإستفسارات والملاحظات قم بمراجعة 'مركز المساعدة' عبر منصة سلاح التلميذ.</p>
                                      </td>
                                    </tr>
                                  </table>
                                </td>
                              </tr>
                            </table>
                          </center>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            """, username, otp);
}
