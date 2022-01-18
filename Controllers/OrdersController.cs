using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace Donia.Controllers
{
    public class OrdersController :Controller
    {
        private readonly DataContext myDbContext;
        public OrdersController( DataContext context
                     )
        {
            this.myDbContext = context;
        }

            [Authorize]
            [HttpPost("order/add")]
            public async Task<ActionResult> addOrder([FromForm] Order order)
            {
            order.payment = 0;

            List<Cart> carts = await myDbContext.carts.Where(x => x.user_id == order.user_id).Where(x => x.order_id == 0).ToListAsync();
             await myDbContext.orders.AddAsync(order);
            Market market = await myDbContext.markets.Where(x => x.Id == order.market_id).FirstAsync();
            Address address = await myDbContext.addresses.Where(x => x.Id == order.address_id).FirstAsync();

            order.market_image = market.image;
            order.market_name = market.title;
            order.market_lat = market.lat;
            order.market_lng = market.lng;
            order.user_lat = address.lat;
            order.user_lng = address.lng;
            //order.date = DateTime.Now; ;
            order.market_rate = market.rate;

            await myDbContext.SaveChangesAsync();
                carts.ForEach(c => { c.order_id = order.Id; });
            User user = await myDbContext.AspNetUsers.Where(x => x.Id == order.user_id).FirstAsync();
               await myDbContext.SaveChangesAsync();
             await Functions.slt.SendNotificationAsync(new List<string>() { market.user_id},"مطبخ ٢٤ | Matbakh24","لديكم طلب جديد الرجاء التأكيد والبدء في تجهيز الطلب",myDbContext);
                return Ok(order);
            }

        [Authorize]
        [HttpPost("order/pay")]
        public async Task<ActionResult> payOrder([FromForm] Order order)
        {

            order.payment = order.payment;
            List<Cart> carts = await myDbContext.carts.Where(x => x.user_id == order.user_id).Where(x => x.order_id == 0).ToListAsync();
            await myDbContext.orders.AddAsync(order);
            Market market = await myDbContext.markets.Where(x => x.Id == order.market_id).FirstAsync();
            Address address = await myDbContext.addresses.Where(x => x.Id == order.address_id).FirstAsync();

            order.market_image = market.image;
            order.market_name = market.title;
            order.market_lat = market.lat;
            order.market_lng = market.lng;
            order.user_lat = address.lat;
            order.user_lng = address.lng;
            //order.date = DateTime.Now; ;
            order.market_rate = market.rate;

            await myDbContext.SaveChangesAsync();
            carts.ForEach(c => { c.order_id = order.Id; });
            User user = await myDbContext.AspNetUsers.Where(x => x.Id == order.user_id).FirstAsync();
            await myDbContext.SaveChangesAsync();
            await Functions.slt.SendNotificationAsync(new List<string>() { market.user_id }, "مطبخ ٢٤ | Matbakh24", "لديكم طلب جديد الرجاء التأكيد والبدء في تجهيز الطلب", myDbContext);
            return Ok(order);
        }


        [Authorize]
        [HttpPost("order/get-orders")]
        public async Task<ActionResult> getUserOrders([FromForm] string userId)
        {
            List<Order> orderss = await myDbContext.orders.Where(x => x.user_id == userId).OrderByDescending(o => o.Id).AsNoTracking().ToListAsync();
            List<OrderDetailResponse> orders =
              orderss
          .Select(x => new OrderDetailResponse() { order = x })
           .ToList();
            return Ok(orders);
        }

       

        //[Authorize]
        [HttpPost("order/detail")]
        public async Task<ActionResult> getOrderDetail([FromForm] int orderId)
        {
            Order orderRow = await myDbContext.orders.Where(x => x.Id == orderId).FirstAsync();
            Driver driver = await myDbContext.drivers.Where(x => x.Id == orderRow.delivery_id).FirstOrDefaultAsync();
            User user = await myDbContext.Users.Where(x => x.Id == orderRow.user_id).FirstOrDefaultAsync();
            Market orderMarket =  await myDbContext.markets.Where(x => x.Id == orderRow.market_id).FirstOrDefaultAsync();
            var market = new
            {
                market = orderMarket
        };
            //Address address = await myDbContext.addresses.Where(x => x.Id == orderRow.address_id).FirstAsync();
            List<CartListResponse> cartsResponse = new List<CartListResponse>();

            double dtotal = 0.0;
            double dtax = 0.0;
            double ddelivery = 0.0;
            List<Cart> carts = await myDbContext.carts.Where(x => x.order_id == orderId).ToListAsync();
            foreach (var cart in carts)
            {
                Food food = await myDbContext.foods.Where(x => x.Id == cart.food_id).FirstAsync();
                List<Photo> photos = await myDbContext.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();
                FoodDetailResponse foodDetail = new FoodDetailResponse
                {
                    food = food,
                    photos = photos
                };
                CartListResponse cartListResponse = new CartListResponse
                {
                    Id = cart.Id,
                    quantity = cart.quantity,
                    food = foodDetail
                };
                cartsResponse.Add(cartListResponse);
                if (ddelivery == 0.0)
                    ddelivery = distanceInMiles(orderRow.user_lng, orderRow.user_lat, food.lng, food.lat) * 0.62 * 2;
                dtotal += (cart.quantity * food.price);
            }

            var dsubtotal = dtotal;
            dtotal = dtotal + ddelivery;
            dtax = 0.15 * dtotal;
            //dtotal = dtotal + dtax;
            var subtotal = Math.Round(dsubtotal, 2);
            var total = Math.Round(dtotal, 2);
            var tax = Math.Round(dtax, 2);
            var delivery = Math.Round(ddelivery, 2);

            var order = new  {
                order =orderRow,
                carts = cartsResponse,
                driver = driver,
                market = market,
                tax = tax,
                user= user,
                delivery = delivery,
                total = total,
                subtotal = subtotal
            };

            return Ok(order);

        }
        



        [HttpPost("order/driver/get-orders")]
        public async Task<ActionResult> getDriverUpcommingOrders([FromForm] int id)
        {
            var orders =await myDbContext.driverOrders.Where(x => x.driver_id == id).Select(o => new { order = myDbContext.orders.Where(x => x.Id == o.order_id).First() }).ToListAsync();
            var prevOrders = await myDbContext.orders.Where(x => x.delivery_id == id).ToListAsync();

            //foreach (var order in prevOrders)
            //{
            //    orders.Add(new
            //    {
            //        order = order
            //    });
            //}
            return Ok(orders);
        }

        [HttpPost("order/driver/get-prevous-orders")]
        public async Task<ActionResult> getDriverPrvOrders([FromForm] int id)
        {
            var orders = await myDbContext.orders.Where(x => x.delivery_id == id).OrderByDescending(x=>x.Id).ToListAsync();
            return Ok(orders);
        }


        [Authorize]
        [HttpPost("order/market/get-orders")]
        public async Task<ActionResult> getMarketOrders([FromForm] int marketId)
        {
            var orderss = await myDbContext.orders.Where(x => x.market_id == marketId).ToListAsync();
            var orders = 
                orderss
            .Select(x => new  { order = x })
             .ToList();

            return Ok(orders);
        }

        [Authorize]
        [HttpPost("order/market/accept-Order")]
        public async Task<ActionResult> acceptMarketOrder([FromForm] int orderId)
        {
            var order = await myDbContext.orders.Where(x => x.Id == orderId).FirstAsync();
            Address address = await myDbContext.addresses.Where(x => x.Id == order.address_id).FirstAsync();

            if (order.status==0) order.status = 1;
            myDbContext.SaveChanges();
            var myLat = address.lat;
            var myLon = address.lng;
            var radiusInMile = 50;

            var drivers = myDbContext.drivers
                    .AsEnumerable()
                    .Select(driver => new { driver, Dist = distanceInMiles(myLon, myLat, driver.lng, driver.lat) }).OrderBy(driver => driver.Dist)
                    .Where(p => p.Dist <= radiusInMile).ToList();

            List<string> driversToNotify = new List<string>();

            for (var i = 0; i < drivers.Count; i++)
            {
                Driver driver = drivers[i].driver;
                DriverOrder driverOrder = new DriverOrder()
                {
                    order_id = order.Id,
                    driver_id = driver.Id,
                    market_id = order.market_id
                };
                driversToNotify.Add(driver.user_id);
                await myDbContext.driverOrders.AddAsync(driverOrder);

            }
            await myDbContext.SaveChangesAsync();
            await Functions.slt.SendNotificationAsync(driversToNotify, "مطبخ ٢٤ | Matbakh24", "طلب جديد لدي "+ order.market_name+"يمكنك توصيله ", myDbContext);
            await Functions.slt.SendNotificationAsync(new List<string>() { order.user_id }, "مطبخ ٢٤ | Matbakh24", "مطبخ " + order.market_name + " يقوم بتجهيز طلبك الان", myDbContext);

            return Ok(order);
        }


        [Authorize]
        [HttpPost("order/driver/accept-order")]
        public async Task<ActionResult> driverAcceptOrder([FromForm] int orderId,int driverId)
        {
            Order order = await myDbContext.orders.Where(x => x.Id == orderId).FirstAsync();
             
            if (order.delivery_id == 0)
            {
                order.delivery_id = driverId;
                myDbContext.SaveChanges();
                var ordersToDelete = await myDbContext.driverOrders.Where(x => x.order_id == orderId&& x.driver_id != driverId).ToListAsync();
                myDbContext.driverOrders.RemoveRange(ordersToDelete);
                myDbContext.SaveChanges();
                await Functions.slt.SendNotificationAsync(new List<string>() { order.user_id }, "مطبخ ٢٤ | Matbakh24", "المندوب متوجه لاستلام الطلب #"+order.Id.ToString(), myDbContext);

                return Ok(order);
            }
            else {
                return BadRequest();
            }
        }


        [Authorize]
        [HttpPost("order/driver/update-order")]
        public async Task<ActionResult> updateOrder([FromForm] int orderId, int driverId)
        {
            Order order = await myDbContext.orders.Where(x => x.Id == orderId).FirstAsync();
            order.status = order.status+1;
            if (order.status == 2)
            {
                await Functions.slt.SendNotificationAsync(new List<string>() { order.user_id }, "مطبخ ٢٤ | Matbakh24", "المندوب في الطريق  لتوصيل طلبك  #" + order.Id.ToString(), myDbContext);
            }
            else if (order.status == 3) {
                await Functions.slt.SendNotificationAsync(new List<string>() { order.user_id }, "مطبخ ٢٤ | Matbakh24", "تم توصيل طلبك   #" + order.Id.ToString(), myDbContext);

            }
            await myDbContext.SaveChangesAsync();
            return Ok(order);
        }

        [Authorize]
        [HttpPost("order/rate")]
        public async Task<ActionResult> rateOrder([FromForm] int orderId, int driverRate, int marketRate)
        {
            Order order = await myDbContext.orders.Where(x => x.Id == orderId).FirstAsync();
            Driver driver = await myDbContext.drivers.Where(x => x.Id == order.delivery_id).FirstAsync();
            Market market = await myDbContext.markets.Where(x => x.Id == order.market_id).FirstAsync();
            driver.reviews += 1;
            market.reviews += 1;

            market.review_sum = market.review_sum + marketRate;
            double mRate = market.review_sum / market.reviews;
            market.rate = (int)Math.Round(mRate);

            driver.review_sum = driver.review_sum + driverRate;
            double dRate = driver.review_sum / driver.reviews;
            driver.rate = (int)Math.Round(dRate);
            order.rated = 1;
            await myDbContext.SaveChangesAsync();
            return Ok(marketRate);
        }

        //[Authorize]
        [HttpPost("order/un-rated")]
        public async Task<ActionResult> checkUnRatedOrder([FromForm] string userId)
        {
            var lastOrder = await myDbContext.orders.Where(x => x.user_id == userId).OrderBy(x=>x.Id).LastAsync();
            if (lastOrder.rated == 0&&lastOrder.status==3)
            {
                return Ok(lastOrder);
            }
            else {
                return Ok(1);

            }
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
