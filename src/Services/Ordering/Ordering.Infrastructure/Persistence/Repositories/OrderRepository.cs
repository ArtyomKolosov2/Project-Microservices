using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence.Context;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName) =>
        await DbContext.Orders
            .Where(o => o.UserName == userName)
            .ToListAsync();
}