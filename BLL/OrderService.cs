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
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //_redis = redis;
        }

        //get all order that have been paid/approved or rejected
        public async Task<List<Order>> GetAllOrderByUserIdAsync(Guid userId)
        {
            return await _unitOfWork.OrderRepository
                .GetAll()
                .Where(x => x.UserId == userId)
                .Where(x => x.Status != "Pending")
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOngoingOrderByUserIdAsync(Guid userId)
        {
            return await _unitOfWork.OrderRepository
                .GetAll()
                .Where(x => x.UserId == userId)
                .Where(x => x.Status == "Ongoing")
                .ToListAsync();
        }

        //get order that is pending to act as a cart
        public async Task<Order> GetOrderByUserIdAsync(Guid userId)
        {
            return await _unitOfWork.OrderRepository
                .GetAll()
                .Where(x => x.Status == "Pending")
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }

        //get order by order id
        public async Task<Order> GetOrderById(Guid id) 
        {
            return await _unitOfWork.OrderRepository
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        //order created when user want to buy the first time (acting as a cart) before it gets approved later
        public async Task CreateOrderAsync(Order order)
        {
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

        }

        //will get called after getting approved event from eventhub
        public async Task UpdateOrderAsync(Order order)
        {
            Order orderFromDb = await _unitOfWork.OrderRepository.GetSingleAsync(x => x.Id == order.Id);
            if (orderFromDb == null)
            {
                throw new Exception($"Order with id {order.Id} not exist");
            }

            _unitOfWork.OrderRepository.Edit(order);
            await _unitOfWork.SaveAsync();
        }




    }
}
