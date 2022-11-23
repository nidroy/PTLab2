using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTLab2.Models;


namespace PTLab2Test
{
    public class DependencyInjection
    {
        public static ServiceCollection InitilizeServices()
        {
            var services = new ServiceCollection();
            var options = new DbContextOptionsBuilder<ShopContext>().UseInMemoryDatabase("Shop.db").Options;
            services.AddScoped(newOptions => new ShopContext(options));
            return services;
        }

    }
}
