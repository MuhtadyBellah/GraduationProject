using ECommerce.Core.Models.Laravel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service.ChatHub
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string user, string message);
        public Task BroadCast(string system, string message);
        public Task ReceiveFile(string user, string fileUrl);

        public Task ReceiveAudio(string user, string audio);
    }
}
