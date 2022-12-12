using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _catalogContext;

    public ProductRepository(ICatalogContext catalogContext)
    {
        _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
    }
    
    public async Task<IEnumerable<Product>> GetProducts() => await _catalogContext.Products.AsQueryable().ToListAsync();

    public Task<Product?> GetProduct(ObjectId id) => _catalogContext.Products.AsQueryable().SingleOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<Product>> GetProductByName(string name) =>
        await _catalogContext.Products.AsQueryable()
            .Where(x => x.Name == name)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)=>
        await _catalogContext.Products.AsQueryable()
            .Where(x => x.Category == categoryName)
            .ToListAsync();

    public Task CreateProduct(Product product)
    {
        return _catalogContext.Products.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var replaceResult = await _catalogContext.Products.ReplaceOneAsync(x => x.Id == product.Id, product);
        return replaceResult.IsModifiedCountAvailable;
    }

    public async Task<bool> DeleteProduct(ObjectId id)
    {
        var deleteResult = await _catalogContext.Products.DeleteOneAsync(x => x.Id == id);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
}