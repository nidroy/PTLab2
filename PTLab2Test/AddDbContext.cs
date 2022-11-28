using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTLab2.Models;


namespace PTLab2Test
{
    public class AddDbContext
    {
        public static ServiceCollection InitilizeServices()
        {
            var services = new ServiceCollection();
            // используем бд из памати
            var options = new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase("Shop.db").Options;
            // добавляем контекст ShopContext в качестве сервиса в приложение
            services.AddScoped(newOptions => new ShopContext(options));
            return services;
        }

    }
}
