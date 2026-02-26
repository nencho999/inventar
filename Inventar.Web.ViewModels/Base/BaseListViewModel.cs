using Inventar.Data.Models;
using System;

namespace Inventar.Web.ViewModels.Base
{
    public class BaseListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public decimal MonthlyExpenses { get; set; }
        public decimal OneTimeExpenses { get; set; }

        public double OccupiedCapacity { get; set; }
        public double MaxCapacity { get; set; }
        public WarehouseStatus Status { get; set; }
    }
}