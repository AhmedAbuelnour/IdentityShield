using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IdentityShield.Infrastructure.Clients
{
    public class VodafoneSMSClient(HttpClient httpClient)
    {
        private const string AccountId = "550142009"; // Replace with your account ID
        private const string Password = "Vodafone.1"; // Replace with your password
        private const string SecureHashSecretKey = "A22ED688678C4FA7A8D9B74BDF4F32F1"; // Replace with your secure hash secret key
        private const string SenderName = "Telmeez APP"; // Replace with your secure hash secret key

        public async Task<bool> SendSmsAsync(string receiverMsisdn, string smsText, CancellationToken cancellationToken)
        {
            string secureHash = GenerateSecureHash(receiverMsisdn, smsText);

            HttpResponseMessage response = await httpClient.PostAsync("https://e3len.vodafone.com.eg/web2sms/sms/submit/", new StringContent($@"<?xml version='1.0' encoding='UTF-8'?>
<SubmitSMSRequest xmlns:='http://www.edafa.com/web2sms/sms/model/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.edafa.com/web2sms/sms/model/ SMSAPI.xsd ' xsi:type='SubmitSMSRequest'>
    <AccountId>{AccountId}</AccountId>
    <Password>{Password}</Password>
    <SecureHash>{secureHash}</SecureHash>
    <SMSList>
        <SenderName>{SenderName}</SenderName>
        <ReceiverMSISDN>{receiverMsisdn}</ReceiverMSISDN>
        <SMSText>{smsText}</SMSText>
    </SMSList>
</SubmitSMSRequest>", Encoding.ASCII, "application/xml"), cancellationToken);

            response.EnsureSuccessStatusCode();

            return HandleApiResponse(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        private static string GenerateSecureHash(string receiverMsisdn, string smsText)
        {
            string concatenatedString = $"AccountId={AccountId}&Password={Password}&SenderName={SenderName}&ReceiverMSISDN={receiverMsisdn}&SMSText={smsText}";

            using (var hmacSha256 = new HMACSHA256(Encoding.ASCII.GetBytes(SecureHashSecretKey)))
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
