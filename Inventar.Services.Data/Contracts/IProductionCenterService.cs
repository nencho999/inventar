public interface IProductionCenterService
{
    Task<IEnumerable<CenterIndexViewModel>> GetAllCentersAsync();
    Task<ProductionCenter> FindCenterByIdAsync(Guid? id);
    Task<bool> AddCenterAsync(string userId, CenterCreateInputModel model);
    Task<CenterEditFormModel> GetCenterForEdittingAsync(string userId, Guid? id);

    Task<bool> EditCenterAsync(CenterEditFormModel model);
    Task<CenterDeleteViewModel> GetCenterForDeletingAsync(string userId, Guid? id);
    Task<bool> DeleteCenterAsync(string userId, int id);
}