using Twilio.Rest.Api.V2010.Account;

namespace ECommerce.Service.SMS
{
    public interface ISMSServices 
    {
        MessageResource Send(string phone, string body); 
    }
}
