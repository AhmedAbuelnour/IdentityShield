using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Options;
using IdentityShield.Infrastructure.Clients;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Polly;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace IdentityShield.Infrastructure.Implementations;

public class NotificationService(VodafoneSMSClient vodafoneSMSClient, IOptions<EmailOptions> emailOptionsAccessor) : INotificationService
{
    private readonly EmailOptions emailOptions = emailOptionsAccessor.Value;
    public async Task<bool> SendEmailAsync<TUser>(TUser user, string mailSubject, string token, CancellationToken cancellationToken) where TUser : IdentityUser
    {

        MailMessage emailMessage = new()
        {
            From = new MailAddress(emailOptions.MailFrom, emailOptions.DisplayName, Encoding.UTF8),
            Subject = mailSubject,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Priority = MailPriority.High,
            Body = GenerateHtmlEmailBody(user.UserName, token)
        };

        emailMessage.To.Add(user.NormalizedEmail);

        SmtpClient client = new()
        {
            Credentials = new NetworkCredential(emailOptions.MailFrom, emailOptions.Password),
            Port = 587,
            Host = emailOptions.Host,
            EnableSsl = true
        };


        return await Policy.Handle<Exception>().RetryAsync(5).ExecuteAsync(async () =>
        {
            await client.SendMailAsync(emailMessage, cancellationToken);

            return true;
        });
    }


    public async Task<bool> SendSMSAsync<TUser>(TUser user, string token, CancellationToken cancellationToken) where TUser : IdentityUser
    {
        return await vodafoneSMSClient.SendSmsAsync(user.PhoneNumber, $"Your OTP is: {token}", cancellationToken);
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
