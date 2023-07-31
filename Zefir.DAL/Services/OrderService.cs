using Microsoft.EntityFrameworkCore;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;
using Zefir.Domain.Entity;

namespace Zefir.DAL.Services;

// TODO сделать заказы
public class OrderService
{
    private readonly AppDbContext _appDbContext;

    public OrderService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<PublicOrderData> CreateOrder(CreateOrderDto orderDto)
    {
        DateOnly deadline;
        try
        {
            deadline = DateOnly.Parse(orderDto.Deadline);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ServiceBadRequestError((nameof(orderDto.Deadline), "invalid date. Format: mm-dd-yyyy"));
        }

        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == orderDto.UserId);
        if (user is null)
            throw new ServiceBadRequestError((nameof(orderDto.UserId), "User with such id is not exists"));
        var products = await _appDbContext.Products
            .Where(p => orderDto.ProductsId.Contains(p.Id))
            .ToListAsync();
        if (products.Count < 1) throw new ServiceBadRequestError((nameof(orderDto.ProductsId), "Invalid products id"));

        var order = new Order(deadline)
        {
            User = user,
            Products = products
        };
        await _appDbContext.Orders.AddAsync(order);
        await _appDbContext.SaveChangesAsync();

        var publicOrder = new PublicOrderData(order.Id, order.User.Id, order.Products, order.Status, order
            .Deadline);
        return publicOrder;
    }
}
