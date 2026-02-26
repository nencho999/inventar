using Inventar.Web.ViewModels.Production;

public interface IProductionService
{
    Task<ProductionRegistrationFormModel> GetRegistrationModelAsync();

    Task<IEnumerable<ProductSelectViewModel>> GetAllowedProductsAsync(Guid centerId);

    Task RegisterProductionAsync(ProductionRegistrationFormModel model);
}