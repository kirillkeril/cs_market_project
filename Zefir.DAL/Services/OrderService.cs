﻿using Microsoft.EntityFrameworkCore;
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

        var publicOrder = new PublicOrderData(
            order.Id,
            order.User.Id,
            order.Products,
            (Status)order.Status,
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

        // var user = _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == candidate.User.Id);

        var publicOrderData = new PublicOrderData(
            candidate.Id,
            candidate.User.Id,
            candidate.Products.ToList(),
            newStatus,
            candidate.Deadline);
        return publicOrderData;
    }
}
