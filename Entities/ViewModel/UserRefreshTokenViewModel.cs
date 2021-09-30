using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModel
{
    public class UserRefreshTokenViewModel
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
