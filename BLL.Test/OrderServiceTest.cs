using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using BLL;
using BLL.Redis;
using BLL.Eventhub;
using DAL.Models;
using DAL.Repositories;
using BLL.Test.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BLL.Test
{
    public class OrderServiceTest
    {
        private IEnumerable<Order> orders;
        private Mock<IRedisService> redis;
        private Mock<IUnitOfWork> uow;

        public OrderServiceTest()
        {
            orders = CommonHelper.LoadDataFromFile<IEnumerable<Order>>(@"MockData\Order.json");
            uow = MockUnitOfWork();
        }

        private OrderService CreateOrderService()
        {
            return new OrderService(uow.Object);
        }

        #region method mock depedencies


        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var orderQueryable = orders.AsQueryable().BuildMock().Object;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(u => u.OrderRepository.GetAll())
                .Returns(orderQueryable);

            mockUnitOfWork
                .Setup(u => u.OrderRepository.IsExist(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns((Expression<Func<Order, bool>> condition) => orderQueryable.Any(condition));

            mockUnitOfWork
               .Setup(u => u.OrderRepository.GetSingleAsync(It.IsAny<Expression<Func<Order, bool>>>()))
               .ReturnsAsync((Expression<Func<Order, bool>> condition) => orderQueryable.FirstOrDefault(condition));

            mockUnitOfWork
               .Setup(u => u.OrderRepository.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Order order, CancellationToken token) =>
               {
                   order.Id = Guid.NewGuid();
                   return order;
               });

            mockUnitOfWork
                .Setup(u => u.OrderRepository.Delete(It.IsAny<Expression<Func<Order, bool>>>()))
                .Verifiable();


            mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockUnitOfWork;
        }


       


        #endregion method mock depedencies

        [Theory]
        [InlineData("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public async Task GetAllAsync_Success(string Id)
        {
            //arrange
            var id = Guid.Parse(Id);
            var expected = orders.Where(x => x.UserId == id).Where(x => x.Status != "Pending");

            var svc = CreateOrderService();

            // act
            var actual = await svc.GetAllOrderByUserIdAsync(id);

            // assert      
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public async Task GetCart_Success(string Id)
        {
            //arrange
            var id = Guid.Parse(Id);
            var expected = orders
                .Where(x => x.Status == "Pending")
                .First(b => b.UserId == id);

            var svc = CreateOrderService();

            //act
            var actual = await svc.GetOrderByUserIdAsync(id);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("3fa85f64-1234-4562-b3fc-2c963f66afa6")]
        public async Task GetOrder_Success(string Id)
        {
            //arrange
            var id = Guid.Parse(Id);
            var expected = orders.First(x => x.Id == id);

            var svc = CreateOrderService();

            //act
            var actual = await svc.GetOrderById(id);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateOrder_Success()
        {
            //arrange
            var expected = new Order
            {

                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Status = "Pending"


            };

            var svc = CreateOrderService();

            //act
            Func<Task> act = async () => { await svc.CreateOrderAsync(expected); };

            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        

        [Theory]
        [InlineData("3fa85f64-1234-4562-b3fc-2c963f66afa6", "3fa85f64-5717-4562-b3fc-2c963f66afa6", "Approved")]
        public async Task UpdateOrder_Success(string id, string userId, string status)
        {
            //arrange
            var expected = new Order
            {
                Id = Guid.Parse(id),
                UserId = Guid.Parse(userId),
                Status = status
                
            };

            var svc = CreateOrderService();


            //act
            Func<Task> act = async () => { await svc.UpdateOrderAsync(expected); };

            //assert
            await act.Should().NotThrowAsync<Exception>();
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
        


    }
}
