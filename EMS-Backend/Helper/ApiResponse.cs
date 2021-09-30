using Entities.ViewModel;
using System;
using System.Collections.Generic;


namespace EMS_Backend.Helper
{
    public class ApiResponse
    {
        public Object data { get; set; } = new Object();
        public int dataCount { get; set; }
        public Object supportingLists { get; set; } = new Object();
        public Object errors { get; set; } = new Object();
        public double sessionTimeout { get; set; }
        public List<UserAuthorityViewModel> restrictions { get; set; } = new List<UserAuthorityViewModel>();
        public string message { get; set; } = string.Empty;
    }
}
