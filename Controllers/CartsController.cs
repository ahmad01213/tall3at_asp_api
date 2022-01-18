using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Controllers
{


    public class CartsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext myDbContext;
        public readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartsController(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IMapper mapper, DataContext context
                     )
        {
            this._mapper = mapper;
            this._webHostEnvironment = webHostEnvironment;
            this.myDbContext = context;
            this._httpContextAccessor = httpContextAccessor;

        }



        [Authorize(Roles = "provider,user")]
        [HttpPost("cart/add")]
        public async Task<ActionResult> addCart([FromForm] Cart cart)
        {
            User user = await Functions.getCurrentUser(_httpContextAccessor, myDbContext);
            var carts = await myDbContext.carts.Where(x => x.order_id == 0 && x.user_id == cart.user_id).ToListAsync();

            if (carts.LastOrDefault() != null)
            {
                Cart lastCart = carts[carts.Count - 1];
                if (lastCart.market_id != cart.market_id && lastCart.order_id == 0)
                {
                    myDbContext.carts.RemoveRange(carts);
                    await myDbContext.SaveChangesAsync();
                }
            }
            cart.user_id = user.Id;
            await myDbContext.carts.AddAsync(cart);
           await myDbContext.SaveChangesAsync();
            return Ok(cart);
        }

        [Authorize(Roles = "user,provider")]
        [HttpPost("cart/update")]
        public async Task<ActionResult> updateCartQuantity([FromForm] UpdateQuantityRequest modle)
        {
            Cart cart = await myDbContext.carts.FindAsync(modle.id);

            if (modle.status == 0)
            {

                if (cart.quantity == 1)
                {
                    myDbContext.carts.Remove(cart);
                }
                else
                {
                    cart.quantity--;

                }
            }
            else
            {
                cart.quantity++;
            }

            await myDbContext.SaveChangesAsync();

            return Ok("success");
        }




        [Authorize(Roles = "user,provider")]
        [HttpPost("cart/delete")]
        public async Task<ActionResult> deleteCart([FromForm] int id)
        {
            var item = await myDbContext.carts.Where(x => x.Id == id).FirstAsync();
            myDbContext.carts.Remove(item);
           await myDbContext.SaveChangesAsync();
            return Ok(id);
        }



        [Authorize]
        [HttpPost("cart/get-carts")]
        public async Task<ActionResult> getcarts([FromForm]int addressId)
        {
            User user = await Functions.getCurrentUser(_httpContextAccessor,myDbContext);
            var carts = await myDbContext.carts.Where(x => x.order_id == 0&&x.user_id == user.Id).ToListAsync();
            foreach (Cart cart in carts)
            {
                cart.address_id = addressId;
            }
            await myDbContext.SaveChangesAsync();
            Address address = await myDbContext.addresses.Where(x => x.Id == addressId).FirstOrDefaultAsync();
            double dtotal = 0.0;
            double dtax = 0.0;
            double ddelivery = 0.0;

            List<CartListResponse> cartsResponse = new List<CartListResponse>(); 
            
            foreach (var cart in carts)
            {
                Food food = await myDbContext.foods.Where(x => x.Id == cart.food_id).FirstAsync();
                List<Photo> photos = await myDbContext.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();
                FoodDetailResponse foodDetail = new FoodDetailResponse { 
                food = food,
                photos = photos
                };
                CartListResponse cartListResponse = new CartListResponse { 
                Id = cart.Id,
                quantity = cart.quantity,
                food = foodDetail
                };
                cartsResponse.Add(cartListResponse);
           if (ddelivery == 0.0)
                ddelivery = distanceInMiles(address.lng, address.lat, food.lng, food.lat)*0.62*2;
                dtotal += (cart.quantity*food.price);
            }
            var dsubtotal = dtotal;
            dtotal = dtotal + ddelivery;
            dtax = 0.15 * dtotal;
            //dtotal = dtotal + dtax;

            var subtotal = Math.Round(dsubtotal, 2);
            var total = Math.Round(dtotal, 2);
            var tax = Math.Round(dtax, 2);
            var delivery = Math.Round(ddelivery, 2);
            return Ok(new {
                cartsResponse,
                subtotal,
                delivery,
                tax,
                total,
                address,
            });
        }

        public double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        public double distanceInMiles(double lon1d, double lat1d, double lon2d, double lat2d)
        {
            var lon1 = ToRadians(lon1d);
            var lat1 = ToRadians(lat1d);
            var lon2 = ToRadians(lon2d);
            var lat2 = ToRadians(lat2d);
            var deltaLon = lon2 - lon1;
            var c = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon));
            var earthRadius = 3958.76;
            var distInMiles = earthRadius * c;
            return Math.Round(distInMiles, 2);
        }
    }
}
