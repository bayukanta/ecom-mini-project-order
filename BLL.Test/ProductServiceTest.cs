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
    public class ProductServiceTest
    {
        private IEnumerable<Product> products;
        private Mock<IRedisService> redis;
        private Mock<IUnitOfWork> uow;

        public ProductServiceTest()
        {
            products = CommonHelper.LoadDataFromFile<IEnumerable<Product>>(@"MockData\Product.json");
            uow = MockUnitOfWork();
            redis = MockRedis();
        }

        private ProductService CreateProductService()
        {
            return new ProductService(uow.Object, redis.Object);
        }

        #region method mock depedencies


        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var gameQueryable = products.AsQueryable().BuildMock().Object;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(u => u.ProductRepository.GetAll())
                .Returns(gameQueryable);

            mockUnitOfWork
                .Setup(u => u.ProductRepository.IsExist(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns((Expression<Func<Product, bool>> condition) => gameQueryable.Any(condition));

            mockUnitOfWork
               .Setup(u => u.ProductRepository.GetSingleAsync(It.IsAny<Expression<Func<Product, bool>>>()))
               .ReturnsAsync((Expression<Func<Product, bool>> condition) => gameQueryable.FirstOrDefault(condition));

            mockUnitOfWork
               .Setup(u => u.ProductRepository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Product product, CancellationToken token) =>
               {
                   product.Id = Guid.NewGuid();
                   return product;
               });

            mockUnitOfWork
                .Setup(u => u.ProductRepository.Delete(It.IsAny<Expression<Func<Product, bool>>>()))
                .Verifiable();


            mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockUnitOfWork;
        }


        private Mock<IRedisService> MockRedis()
        {
            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetAsync<Product>(It.Is<string>(x => x.Equals("product_productID:311b848d-1234-443e-5e85-08d946c2256c"))))
                .ReturnsAsync(products.FirstOrDefault(x => x.Id == Guid.Parse("311b848d-1234-443e-5e85-08d946c2256c")))
                .Verifiable();

            mockRedis
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockRedis
              .Setup(x => x.DeleteAsync(It.IsAny<string>())).Verifiable();

            return mockRedis;
        }


        #endregion method mock depedencies

        [Fact]
        public async Task GetAllAsync_Success()
        {
            //arrange
            var expected = products;

            var svc = CreateProductService();

            // act
            var actual = await svc.GetAllProductAsync();

            // assert      
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("311b848d-1234-443e-5e85-08d946c2256c")]
        public async Task GetByID_Success(string Id)
        {
            //arrange
            var id = Guid.Parse(Id);
            var expected = products.First(x => x.Id == id);

            var svc = CreateProductService();

            //act
            var actual = await svc.GetProductByIdAsync(id);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateProduct_Success()
        {
            //arrange
            var expected = new Product
            {
                Name = "Black Shirt 2",
                Type = "SHirt",
                Gender = "Male",
                Price = 50000
            };

            var svc = CreateProductService();

            //act
            Func<Task> act = async () => { await svc.CreateProductAsync(expected); };

            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("311b848d-1234-443e-5e85-08d946c2256c")]
        public async Task DeleteProduct_Success(string Id)
        {
            //arrange

            var svc = CreateProductService();
            var id = Guid.Parse(Id);

            //act
            Func<Task> act = async () => { await svc.DeleteProductAsync(id); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);

        }

        [Theory]
        [InlineData("311b848d-1234-443e-5e85-08d946c2256c", "Black Shirt 1 Edit", "Shirt Edit", "Male Edit", 60000)]
        public async Task UpdateProduct_Success(string Id, string name, string type, String gender, int price)
        {
            //arrange
            var expected = new Product
            {
                Id = Guid.Parse(Id),
                Name = name,
                Type = type,
                Gender = gender,
                Price = price
            };

            var svc = CreateProductService();


            //act
            Func<Task> act = async () => { await svc.UpdateProductAsync(expected); };

            //assert
            await act.Should().NotThrowAsync<Exception>();
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }



    }
}
