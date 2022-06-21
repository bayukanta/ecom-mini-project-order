using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BLL.Redis;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BLL
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;

        public ProductService(IUnitOfWork unitOfWork, IRedisService redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _unitOfWork.ProductRepository.GetAll().ToListAsync();
        }

      

        public async Task<List<Product>> FilterPost(Product product)
        {

            return await _unitOfWork.ProductRepository.GetAll()
                .Where(x => product.Type == null || x.Type == product.Type) 
                .Where(x => product.Gender == null || x.Gender == product.Gender)
                .Where(x => product.Name == null || x.Name.Contains(product.Name))
                .ToListAsync();
        }
        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await _redis.GetAsync<Product>($"product_productId:{id}");
            if (product == null)
            {
                product = await _unitOfWork.ProductRepository
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);
                await _redis.SaveAsync($"product_productId:{id}", product);
            }
            return product;
        }

        public async Task CreateProductAsync(Product product)
        {
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveAsync();

            //await SendGameToEventHub(game);
        }

        public async Task UpdateProductAsync(Product product)
        {
            Product productFromDb = await _unitOfWork.ProductRepository.GetSingleAsync(x => x.Id == product.Id);
            if (productFromDb == null)
            {
                throw new Exception($"Product with id {product.Id} not exist");
            }

            _unitOfWork.ProductRepository.Edit(product);
            await _unitOfWork.SaveAsync();
            await _redis.DeleteAsync($"product_productId:{product.Id}");
        }

        public async Task DeleteProductAsync(Guid id)
        {
            _unitOfWork.ProductRepository.Delete(x => x.Id == id);
            await _unitOfWork.SaveAsync();
            await _redis.DeleteAsync($"product_productId:{id}");
        }





    }
}
