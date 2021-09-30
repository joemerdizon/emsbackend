using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModel
{
    public class ChatUser
    {      
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public bool IsSupport { get; set; }
        public DateTime JoinedDateTime
        {
            get { return DateTime.UtcNow; }
        }

    }
}
