using Entities.Enums;
using System.Collections.Generic;

namespace Entities.ViewModel
{
    public class ListSettings
    {
        public string draw { get; set; }
        public string sortColumn { get; set; }

        public SortDirection sortColumnDir { get; set; }

        public int pageSize { get; set; }

        public int skip { get; set; }

        public string searchValue { get; set; }

        public List<long> selectedIds { get; set; }

        public int filterType { get; set; }

        public int filterId { get; set; }

        public int age { get; set; }

        public int ageCondition { get; set; }

        public int gender { get; set; }

    }
}
