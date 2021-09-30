using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModel
{
    public class PrecinctViewModel: BaseViewModel
    {
        //public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ClusterId { get; set; }
    }
}
