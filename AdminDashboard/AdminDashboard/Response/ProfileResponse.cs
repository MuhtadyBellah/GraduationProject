using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public class UserResponse
    {
        public List<UserInfo> users { get; set; } = null;
    }

    public class ProfileResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserInfo User { get; set; } = null;
    }
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string email_verified_at { get; set; }

        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string Role { get; set; }
        public string image { get; set; }
    }
}