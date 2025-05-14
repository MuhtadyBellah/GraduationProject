using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.Emails
{
    public interface IMailServices
    {
        Task SendEmailAsync(string mailto, string subject, string message, IList<IFormFile> attachments = null);
    }
}
