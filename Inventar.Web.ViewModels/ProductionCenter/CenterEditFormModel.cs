using Inventar.Data.Models;

namespace Inventar.Web.ViewModels.ProductionCenter;
public class CenterEditFormModel : CenterCreateInputModel
{
    public Guid Id { get; set; }
}