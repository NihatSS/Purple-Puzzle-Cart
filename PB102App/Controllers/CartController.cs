using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PB102App.Data;
using PB102App.ViewModels;

namespace PB102App.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly AppDbContext _context;

        public CartController(IHttpContextAccessor httpContext,
                              AppDbContext context)
        {
            _httpContext = httpContext;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BasketVM> basket;

            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContext.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            List<BasketDetailVM> basketDetails = new();

            foreach (var item in basket)
            {
                var work = await _context.Works.Include(m => m.Images).FirstOrDefaultAsync(m => m.Id == item.WorkId);

                basketDetails.Add(new BasketDetailVM
                {
                    Id = work.Id,
                    WorkCount = item.WorkCount,
                    Title = work.Title,
                    Image = work.Images.FirstOrDefault(m => m.IsMain).Image,
                    Total = (decimal)(item.WorkCount * work.Price),
                });
            }

            return View(basketDetails);
        }

        public async Task<IActionResult> DeleteWorkFromBasket(int id)
        {
            List<BasketVM> basket;

            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContext.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            var removeData = basket.FirstOrDefault(x=>x.WorkId == id);

            basket.Remove(removeData);

            _httpContext.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction(nameof(Index));
        }


        
        public async Task<IActionResult> DecreaseWorkInBasket(int id)
        {
            List<BasketVM> basket;

            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContext.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            var work = basket.FirstOrDefault(x => x.WorkId == id);

            if (work != null)
            {
                if (work.WorkCount > 1)
                {
                    work.WorkCount--;
                }
            }
            _httpContext.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> IncreaseWorkInBasket(int id)
        {
            List<BasketVM> basket;

            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContext.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            var work = basket.FirstOrDefault(x => x.WorkId == id);

            if (work != null)
            {
                work.WorkCount++;
            }
            _httpContext.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction(nameof(Index));
        }

    }
}
