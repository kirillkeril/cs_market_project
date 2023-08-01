using Microsoft.EntityFrameworkCore;
using Zefir.Core.Entity;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;

namespace Zefir.DAL.Services;

public class OrderService
{
    private readonly AppDbContext _appDbContext;

    public OrderService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<PublicOrderData>> GetAllOrders(int? userId = null)
    {
        List<Order> orders;
        if (userId is null)
            orders = await _appDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Products)
                .ToListAsync();
        else
            orders = await _appDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Products)
                .Where(o => o.User != null && o.User.Id == userId)
                .ToListAsync();

        var publicOrders = new List<PublicOrderData>();

        foreach (var order in orders)
        {
            var status = Enum.Parse<Status>(order.Status.ToString());
            if (order.User != null)
                publicOrders.Add(
                    new PublicOrderData(order.Id, order.User.Id, order.Products, status.ToString(), order.Deadline));
        }

        return publicOrders;
    }

    public async Task<List<PublicOrderData>> GetOwnOrders(int ownerId)
    {
        var orders = await _appDbContext.Orders
            .Include(o => o.Products)
            .Include(o => o.User)
            .Where(o => o.User != null && o.User.Id == ownerId)
            .ToListAsync();

        List<PublicOrderData> publicOrders = new();
        foreach (var order in orders)
        {
            var status = Enum.Parse<Status>(order.Status.ToString());
            if (order.User != null)
                publicOrders.Add(
                    new PublicOrderData(order.Id, order.User.Id, order.Products, status.ToString(), order.Deadline));
        }

        return publicOrders;
    }

    public async Task<PublicOrderData> CreateOrder(int userId, CreateOrderDto orderDto)
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

        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            throw new ServiceBadRequestError((nameof(userId), "User with such id is not exists"));
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

        var status = Enum.Parse<Status>(order.Status.ToString());
        var publicOrder = new PublicOrderData(
            order.Id,
            order.User.Id,
            order.Products,
            status.ToString(),
            order.Deadline);
        return publicOrder;
    }

    public async Task<PublicOrderData> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
    {
        var candidate = await _appDbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (candidate is null) throw new ServiceNotFoundError("No order with such id");

        candidate.Status = dto.Status;
        await _appDbContext.SaveChangesAsync();

        if (!Enum.TryParse(candidate.Status.ToString(), out Status newStatus))
            throw new ServiceBadRequestError((nameof(dto.Status), "Invalid status"));

        if (candidate.User != null)
        {
            var publicOrderData = new PublicOrderData(
                candidate.Id,
                candidate.User.Id,
                candidate.Products.ToList(),
                newStatus.ToString(),
                candidate.Deadline);
            return publicOrderData;
        }

        throw new ServiceNotFoundError("User is undefined");
    }
}
