using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BLL.Redis;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BLL.Eventhub;

namespace BLL
{   
    //Ordered product
    public class OrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IRedisService _redis;
        private readonly IOrderEventSenderFactory _msgSenderFactory;

        public OrderDetailService(IUnitOfWork unitOfWork, IRedisService redis, IConfiguration config, IOrderEventSenderFactory msg)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
            _config = config;
            _msgSenderFactory = msg;
        }

        public async Task<List<OrderDetail>> GetAllOrderDetailByOrderIdAsync(Guid orderId)
        {
            
              return await _unitOfWork.OrderDetailRepository.GetAll().Include(a => a.Product).Where(x => x.OrderId == orderId).ToListAsync();

        }

        public async Task<List<OrderDetail>> GetAllOrderDetail()
        {

            return await _unitOfWork.OrderDetailRepository.GetAll().Include(a => a.Product).ToListAsync();

        }

        //sending order detail to eventhub for every orderdetail
        public async Task CreateOrderDetailAsync(OrderDetail od)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(od.ProductId);
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(od.OrderId);
            od.Order = order;
            od.OrderPrice = od.Quantity * product.HargaJual;
            await _unitOfWork.OrderDetailRepository.AddAsync(od);
            await _unitOfWork.SaveAsync();




            //od.Order = order;
            //var product = await _unitOfWork.ProductRepository.GetByIdAsync(od.ProductId);
            //od.Product = product;


            await SendOrderDetailToEventHub(od);
        }

        private async Task SendOrderDetailToEventHub(OrderDetail od)
        {
            string topic = _config.GetValue<string>("EventHub:EventHubNameTest");

            //create event hub producer
            using IOrderEventSender message = _msgSenderFactory.Create(_config, topic);

            //create batch
            await message.CreateEventBatchAsync();

            //add message, ini bisa banyak sekaligus
            message.AddMessage(od);

            //send message
            await message.SendMessage();
        }





    }
}
