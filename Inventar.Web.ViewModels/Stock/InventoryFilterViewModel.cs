using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Stock
{
    public class InventoryFilterViewModel
    {
        public IEnumerable<StockLevelViewModel> StockLevels { get; set; } = new List<StockLevelViewModel>();

        public string SearchTerm { get; set; }
        public Guid? BaseId { get; set; }
        public Guid? MaterialId { get; set; }

        public IEnumerable<DropdownPairViewModel> Bases { get; set; }
        public IEnumerable<DropdownPairViewModel> Materials { get; set; }
    }
}
