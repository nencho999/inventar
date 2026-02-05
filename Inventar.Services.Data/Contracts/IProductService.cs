using Inventar.Web.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Services.Data.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductIndexViewModel>> GetAllProductsAsync();
        Task AddProductAsync(ProductFormModel model);
        Task<ProductFormModel?> GetProductForEditAsync(Guid id);
        Task EditProductAsync(ProductFormModel model);
        Task DeleteMultipleProductsAsync(List<Guid> ids);
    }
}
