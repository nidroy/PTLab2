using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PTLab2.Controllers;
using PTLab2.Models;
using System;

namespace PTLab2Test
{
    public class HomeControllerTest
    {
        private readonly Mock<ILogger<HomeController>> mock = new();
        private readonly IServiceProvider serviceProvider;

        public HomeControllerTest()
        {
            serviceProvider = AddDbContext.InitilizeServices().BuildServiceProvider();
        }

        [Fact]
        public Task TestDiscount()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var user = new User()
            {
                Mail = "123",
                Password = "123",
                Birthday = DateTime.Now,
            };

            HomeController.authorizedUser = user;
            HomeController.isAuthorized = true;
            controller.Products();

            Assert.True(HomeController.isDiscount);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task TestUpdateProducts()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var user = new User()
            {
                Mail = "123",
                Password = "123",
                Birthday = DateTime.Now,
            };

            HomeController.authorizedUser = user;
            HomeController.isAuthorized = true;

            var product = new Product()
            {
                Name = "qqq",
                Price = 10,
                Count = 10,
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            List<Product> products = controller.UpdateProducts();

            Assert.Equal(9, products[0].Price);
        }

        [Fact]
        public async Task TestRegistration()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var user = new User()
            {
                Mail = "123",
                Password = "123",
                FirstName = "qqq",
                LastName = "www",
                Birthday = DateTime.Now,
            };

            await controller.Registration(user);

            User contextUser = db.Users.Last();

            Assert.Equal(contextUser, user);
        }

        [Fact]
        public async Task TestAuthorization()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var user = new User()
            {
                Mail = "123",
                Password = "123",
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            await controller.Authorization(user);

            Assert.Equal(HomeController.authorizedUser, user);
        }

        [Fact]
        public async Task TestCreateProduct()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var product = new Product()
            {
                Name = "qqq",
                Price = 25,
                Count = 1,
            };

            await controller.CreateProduct(product);

            Product contextProduct = db.Products.Last();

            Assert.Equal(contextProduct, product);
        }

        [Fact]
        public async Task TestDeleteProduct()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var product1 = new Product()
            {
                Name = "qqq",
                Price = 25,
                Count = 1,
            };

            var product2 = new Product()
            {
                Name = "www",
                Price = 25,
                Count = 1,
            };

            db.Products.Add(product1);
            db.Products.Add(product2);
            await db.SaveChangesAsync();

            Product contextProduct = db.Products.Last();

            int id = contextProduct.Id;

            await controller.DeleteProduct(id);

            contextProduct = db.Products.Last();

            Assert.Equal(contextProduct.Id, id - 1);
        }

        [Fact]
        public async Task TestBuyProduct()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var product = new Product()
            {
                Name = "qqq",
                Price = 25,
                Count = 5,
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            Product contextProduct = db.Products.Last();

            int count = contextProduct.Count;

            await controller.BuyProduct(contextProduct.Id);

            contextProduct = db.Products.Last();

            Assert.Equal(contextProduct.Count, count - 1);
            Assert.Equal(1, db.Basket.Count());
        }

        [Fact]
        public async Task TestBasket()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var basket = new Basket()
            {
                ProductName = "123",
                ProductPrice = 23,
                ProductCount = 10,
                Amount = 230,
            };

            db.Basket.Add(basket);
            await db.SaveChangesAsync();

            Assert.Equal(1, db.Basket.Count());

            await controller.Pay();

            Assert.Equal(0, db.Basket.Count());
        }

        [Fact]
        public async Task TestAmount()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var product = new Product()
            {
                Name = "qqq",
                Price = 5,
                Count = 10,
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            Product contextProduct = db.Products.Last();

            for (int i = 0; i < 5; i++)
                await controller.BuyProduct(contextProduct.Id);

            Assert.Equal(25, db.Basket.Last().Amount);
        }

        [Fact]
        public async Task TestDiscountAmount()
        {
            var db = serviceProvider.GetRequiredService<ShopContext>();
            var controller = new HomeController(mock.Object, db);

            var product = new Product()
            {
                Name = "qqq",
                Price = 10,
                Count = 10,
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            HomeController.isDiscount = true;

            Product contextProduct = db.Products.Last();

            for (int i = 0; i < 5; i++)
                await controller.BuyProduct(contextProduct.Id);

            Assert.Equal(45, db.Basket.Last().Amount);
        }
    }
}