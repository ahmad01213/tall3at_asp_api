using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Mvc;
namespace Donia.Controllers
{

    //[Authorize(Roles = "admin")]
    [ApiController]
    public class AdminController : ControllerBase {


        private readonly DataContext _context;

        public AdminController( DataContext context
                  )
        {
            this._context = context;
        }


        [HttpPost("admin/home")]
        public async Task<ActionResult> register([FromForm] UserForRegister model)
        {

            string counts = "";
            List<Order> orderss = await _context.orders.Take(6).OrderByDescending(o => o.Id).AsNoTracking().ToListAsync();
            List<OrderDetailResponse> orders =
              orderss
          .Select(x => new OrderDetailResponse() { order = x })
           .ToList();

            List<Food> fods = await _context.foods.OrderByDescending(x => x.Id).Take(6).AsNoTracking().ToListAsync();
            List<FoodDetailResponse> foods = new List<FoodDetailResponse>();
            foreach (var food in fods)
            {
                List<Photo> photos = await _context.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();
                FoodDetailResponse foodDetail = new FoodDetailResponse()
                {
                    food = food,
                    photos = photos
                };
                foods.Add(foodDetail);
            }
            counts = _context.Users.Where(x=>x.Role == "user").Count() + "#" +_context.Users.Where(x => x.Role == "driver").Count() + "#"+ _context.Users.Where(x => x.Role == "provider").Count()
                     + "#" +_context.orders.Count() + "#" +_context.foods.Count() + "#" ;

            var response = new
            {
              counts,
             orders,
             foods,

            };
            return Ok(response);
        }


        [HttpGet("admin/get-users")]
        public async Task<ActionResult> getusers()
        {
            var data = await _context.AspNetUsers.Where(x => x.Role == "user").OrderByDescending(x=>x.Id).AsNoTracking().ToListAsync();
            return Ok(data);
        }

      

        [HttpGet("admin/get-admins")]
        public async Task<ActionResult> getAdmins()
        {
            var data = await _context.AspNetUsers.Where(x => x.Role == "admin").OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();
            return Ok(data);
        }

        [HttpGet("admin/get-drivers")]
        public async Task<ActionResult> getdrivers()
        {
            var data = await _context.drivers.OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();
            return Ok(data);
        }

        [HttpGet("admin/get-markets")]
        public async Task<ActionResult> getmarkets()
        {
            var data = await _context.markets.OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();
            return Ok(data);
        }

        [HttpGet("admin/get-foods")]
        public async Task<ActionResult> getfoods()
        {
            List<Food> fods = await _context.foods.OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();
            List<FoodDetailResponse> foods = new List<FoodDetailResponse>();
            foreach (var food in fods)
            {
                List<Photo> photos = await _context.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();
                FoodDetailResponse foodDetail = new FoodDetailResponse()
                {
                    food = food,
                    photos = photos
                };
                foods.Add(foodDetail);
            }
            return Ok( foods );
        }

        [HttpGet("admin/get-fields")]
        public async Task<ActionResult> getfields()
        {
            var data = await _context.fields.AsNoTracking().ToListAsync();
            return Ok(data);
        }

        [HttpGet("admin/get-orders")]
        public async Task<ActionResult> getorders()
        {

            List<Order> orderss = await _context.orders.OrderByDescending(x => x.Id).OrderByDescending(o => o.Id).AsNoTracking().ToListAsync();
            List<OrderDetailResponse> orders =
              orderss
          .Select(x => new OrderDetailResponse() { order = x })
           .ToList();
            return Ok(orders);

        }

    }
}
