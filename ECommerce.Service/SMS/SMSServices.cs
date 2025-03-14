//using Microsoft.Extensions.Options;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;

//namespace ECommerce.Service.SMS
//{
//    public class SMSServices :
//    {
//        private readonly TwilioSettings _twilioSettings;

//        public SMSServices(IOptions<TwilioSettings> twilioSettings)
//        {
//            _twilioSettings = twilioSettings.Value;
//        }

//        public MessageResource Send(string phone, string body)
//        {
//            TwilioClient.Init(_twilioSettings.SID, _twilioSettings.Token);
//            return MessageResource.Create(
//                body: body,
//                from: new Twilio.Types.PhoneNumber(_twilioSettings.Phone),
//                to: new Twilio.Types.PhoneNumber(phone)
//            );
//        }
//    }
//}
