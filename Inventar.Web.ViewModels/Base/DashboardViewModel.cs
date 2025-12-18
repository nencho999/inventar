using System.Collections.Generic;
using Inventar.Web.ViewModels.Base;

namespace Inventar.Web.ViewModels.Base
{
    public class DashboardViewModel
    {
        public int TotalBases { get; set; }
        public decimal TotalMonthlyExpenses { get; set; }
        public decimal TotalOneTimeExpenses { get; set; }
        public decimal TotalInventoryValue { get; set; }

        public IEnumerable<BaseListViewModel> Bases { get; set; }
    }
}