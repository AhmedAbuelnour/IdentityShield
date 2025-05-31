using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using IdentityShield.Domain.Options;
using Microsoft.Extensions.Options;

namespace IdentityShield.Infrastructure.Clients
{
    public class VodafoneSMSClient(HttpClient httpClient, IOptions<VodafoneSMSOptions> options)
    {
        private readonly VodafoneSMSOptions _options = options.Value;

        public async Task<bool> SendSmsAsync(string receiverMsisdn, string smsText, CancellationToken cancellationToken)
        {
            string secureHash = GenerateSecureHash(receiverMsisdn, smsText);

            HttpResponseMessage response = await httpClient.PostAsync(_options.BaseUrl, new StringContent($@"<?xml version='1.0' encoding='UTF-8'?>
<SubmitSMSRequest xmlns:='http://www.edafa.com/web2sms/sms/model/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.edafa.com/web2sms/sms/model/ SMSAPI.xsd ' xsi:type='SubmitSMSRequest'>
    <AccountId>{_options.AccountId}</AccountId>
    <Password>{_options.Password}</Password>
    <SecureHash>{secureHash}</SecureHash>
    <SMSList>
        <SenderName>{_options.SenderName}</SenderName>
        <ReceiverMSISDN>{receiverMsisdn}</ReceiverMSISDN>
        <SMSText>{smsText}</SMSText>
    </SMSList>
</SubmitSMSRequest>", Encoding.ASCII, "application/xml"), cancellationToken);

            response.EnsureSuccessStatusCode();

            return HandleApiResponse(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        private string GenerateSecureHash(string receiverMsisdn, string smsText)
        {
            string concatenatedString = $"AccountId={_options.AccountId}&Password={_options.Password}&SenderName={_options.SenderName}&ReceiverMSISDN={receiverMsisdn}&SMSText={smsText}";

            using (var hmacSha256 = new HMACSHA256(Encoding.ASCII.GetBytes(_options.SecureHashSecretKey)))
            {
                byte[] hashBytes = hmacSha256.ComputeHash(Encoding.ASCII.GetBytes(concatenatedString));

                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        public static SubmitSMSResponse? DeserializeResponse(string xmlResponse)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SubmitSMSResponse));

            using (StringReader reader = new StringReader(xmlResponse))
            {
                return (SubmitSMSResponse?)serializer.Deserialize(reader);
            }
        }
        private bool HandleApiResponse(string resultXml)
        {
            SubmitSMSResponse? response = DeserializeResponse(resultXml);

            if (response?.ResultStatus == "FAILED_TO_SUBMIT" || response?.ResultStatus == "INVALID_REQUEST")
            {
                throw new Exception($"Could not send SMS Message with status:{response.ResultStatus}, description:{response.Description}");
            }

            return true;
        }
    }

    [XmlRoot("SubmitSMSResponse", Namespace = "http://www.edafa.com/web2sms/sms/model/")]
    public class SubmitSMSResponse
    {
        [XmlElement("ResultStatus")]
        public string ResultStatus { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }
}
