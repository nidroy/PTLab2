using Microsoft.AspNetCore.Mvc;
using PTLab2.Models;
using System.Diagnostics;

namespace PTLab2.Controllers
{
    public class HomeController : Controller
    {
        ILogger<HomeController> logger;
        ShopContext context;
        public static User authorizedUser = new User();
        public static bool isAuthorized = false;
        public static bool isDiscount = false;

        public HomeController(ILogger<HomeController> logger, ShopContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        /// <summary>
        /// главная страница
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// вернуться в главное меню
        /// </summary>
        public async Task<IActionResult> ReturnIndex()
        {
            return RedirectToAction("Index");
        }

        #region товары

        /// <summary>
        /// товары
        /// </summary>
        public IActionResult Products()
        {
            List<Product> products = UpdateProducts();
            return View(products);
        }

        /// <summary>
        /// обновить товары
        /// </summary>
        public List<Product> UpdateProducts()
        {
            List<Product> products = new List<Product>();
            products = context.Products.ToList();
            isDiscount = false;
            if (isAuthorized)
            {
                DateTime todayDate = DateTime.Now;
                DateTime userDate = authorizedUser.Birthday;
                if (userDate.Day == todayDate.Day && userDate.Month == todayDate.Month)
                {
                    isDiscount = true;
                    for (int i = 0; i < products.Count; i++)
                        products[i].Price = products[i].Price * 0.9f;
                }
            }

            return products;
        }

        /// <summary>
        /// добавить товар
        /// </summary>
        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        /// <summary>
        /// удалить товар
        /// </summary>
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id != null)
            {
                Product product = context.Products.FirstOrDefault(x => x.Id == id);
                if (product != null)
                {
                    context.Products.Remove(product);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Products");
                }
            }
            return NotFound();
        }

        /// <summary>
        /// купить товар
        /// </summary>
        public async Task<IActionResult> BuyProduct(int? id)
        {
            if (id != null)
            {
                Product product = context.Products.FirstOrDefault(x => x.Id == id);
                if (product != null)
                {
                    Basket basket = context.Basket.FirstOrDefault(x => x.ProductName == product.Name);
                    if (basket == null)
                    {
                        Basket newBasket = new Basket();
                        newBasket.ProductName = product.Name;
                        if (isDiscount)
                            newBasket.ProductPrice = product.Price * 0.9f;
                        else
                            newBasket.ProductPrice = product.Price;
                        newBasket.ProductCount = 1;
                        newBasket.Amount = newBasket.ProductPrice;
                        context.Basket.Add(newBasket);
                    }
                    else
                    {
                        basket.ProductCount++;
                        basket.Amount = basket.ProductPrice * basket.ProductCount;
                        context.Basket.Update(basket);
                    }
                    product.Count--;
                    if (product.Count == 0)
                        context.Products.Remove(product);
                    else
                        context.Products.Update(product);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Products");
                }
            }
            return NotFound();
        }

        #endregion

        #region профиль

        /// <summary>
        /// профиль
        /// </summary>
        public IActionResult Profile()
        {
            return View();
        }

        /// <summary>
        /// регистрация
        /// </summary>
        public IActionResult Registration()
        {
            Pay();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return RedirectToAction("Profile");
        }

        /// <summary>
        /// авторизация
        /// </summary>
        public async Task<IActionResult> Authorization(User user)
        {
            Pay();
            User contextUser = context.Users.FirstOrDefault(x => x.Mail == user.Mail && x.Password == user.Password);
            if (contextUser == null)
                return RedirectToAction("Profile");
            else
            {
                isAuthorized = true;
                authorizedUser = contextUser;
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region корзина

        /// <summary>
        /// корзина
        /// </summary>
        public IActionResult Basket()
        {
            return View(context.Basket);
        }

        /// <summary>
        /// оплатить
        /// </summary>
        public async Task<IActionResult> Pay()
        {
            Clear();
            return RedirectToAction("Basket");
        }

        /// <summary>
        /// очистить
        /// </summary>
        public async Task<IActionResult> Clear()
        {
            foreach (var basket in context.Basket)
                context.Basket.Remove(basket);
            context.SaveChanges();
            return RedirectToAction("Basket");
        }

        #endregion

        /// <summary>
        /// остальной код
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}