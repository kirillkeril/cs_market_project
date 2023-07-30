﻿using Microsoft.EntityFrameworkCore;
using Zefir.DAL.Dto;
using Zefir.Domain.Entity;

namespace Zefir.DAL.Services;

public class ProductService
{
    private readonly AppDbContext _appDbContext;

    public ProductService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _appDbContext.Products.Include(p => p.Characteristics).ToListAsync();
    }

    public async Task<List<Product>> GetBySearch(string search)
    {
        var products = _appDbContext.Products.Where(
            p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search) ||
                p.Characteristics.Any(c => c.Value.Contains(search)));

        return await products.ToListAsync();
    }

    public async Task<Product?> GetProductById(int id)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> CreateProduct(CreateProductDto dto)
    {
        //TODO make image file path
        var newProduct = new Product(dto.Name, dto.Description, "");
        try
        {
            foreach (var characteristic in dto.Characteristics)
            {
                var newCharacteristic = new Characteristics(characteristic.Key, characteristic.Value);
                // _appDbContext.Characteristics.Add(newCharacteristic);
                newProduct.Characteristics.Add(newCharacteristic);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }


        _appDbContext.Products.Add(newProduct);
        await _appDbContext.Characteristics.AddRangeAsync(newProduct.Characteristics);
        await _appDbContext.SaveChangesAsync();
        return newProduct;
    }

    public async Task<Product?> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) throw new Exception("Product not found");
        product.Name = dto.Name;
        product.Description = dto.Description;
        foreach (var characteristic in dto.Characteristics)
        {
            var newCharacteristic = new Characteristics(characteristic.Key, characteristic.Value);
            var possibleCharacteristic = product.Characteristics.Find(c => c.Key == newCharacteristic.Key);

            if (possibleCharacteristic is not null) possibleCharacteristic.Value = newCharacteristic.Value;
            else product.Characteristics.Add(newCharacteristic);
        }

        await _appDbContext.SaveChangesAsync();

        return product;
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var candidate = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (candidate is null) return false;
        _appDbContext.Products.Remove(candidate);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
}
