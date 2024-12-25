using ECommerce.Core.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repo.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbContext)
        {
            try
            {
                // Start a transaction to ensure atomic operations
                using var transaction = await dbContext.Database.BeginTransactionAsync();

                // Seed ProductBrands if empty
                if (!await dbContext.ProductBrands.AnyAsync())
                {
                    var brandData = await ReadFileAsync("../ECommerce.Repo/Data/DataSeed/brands.json");
                    if (brandData != null)
                    {
                        var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
                        if (brands != null && brands.Any())
                        {
                            await dbContext.ProductBrands.AddRangeAsync(brands);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }

                // Seed ProductTypes if empty
                if (!await dbContext.ProductTypes.AnyAsync())
                {
                    var typeData = await ReadFileAsync("../ECommerce.Repo/Data/DataSeed/types.json");
                    if (typeData != null)
                    {
                        var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                        if (types != null && types.Any())
                        {
                            await dbContext.ProductTypes.AddRangeAsync(types);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }

                // Seed Products if empty
                if (!await dbContext.Products.AnyAsync())
                {
                    var productData = await ReadFileAsync("../ECommerce.Repo/Data/DataSeed/products.json");
                    if (productData != null)
                    {
                        var products = JsonSerializer.Deserialize<List<Product>>(productData);
                        if (products != null && products.Any())
                        {
                            await dbContext.Products.AddRangeAsync(products);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during seeding: {ex.Message}");
                // Optional: Log to a logging service or use another mechanism to track errors
            }
        }

        // Helper method to handle file reading asynchronously
        private static async Task<string?> ReadFileAsync(string filePath)
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file at {filePath}: {ex.Message}");
                return null; // Return null if the file couldn't be read
            }
        }
    }
}
