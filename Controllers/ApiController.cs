        using Microsoft.AspNetCore.Http;
        using System.IO;
        using System.Threading.Tasks;
        using AutoMapper;
        using Microsoft.AspNetCore.Mvc;

        using Microsoft.AspNetCore.Hosting;
        using Microsoft.EntityFrameworkCore;
        using System.Linq;

        using System.Collections.Generic;
        using System;
        using Microsoft.AspNetCore.Authorization;
    using System.Net;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Text;
using Donia.Dtos;
using Donia.Models;

namespace Donia.Controllers
        {

            public class ApiController : Controller
            {


        public async Task<bool> SendNotificationAsync(List<string> tokens,string title,string body)
        {
            using (var client = new HttpClient())
            {
                var firebaseOptionsServerId = "AAAAnh3Nbes:APA91bE3GtVFUXdeK2In1SjuKytaK9kJphq_BMvn_sBTflt4ZqTGYWSzu16tE-acZ7Ul0Vfx_6OEumubBBn0UHHh5jx9Noxxm1HhKB6CIdboyD8s4DyZWzTkh9Frw2JbURqhGxjwVCDm";
                var firebaseOptionsSenderId = "679104835051";

                client.BaseAddress = new Uri("https://fcm.googleapis.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                    $"key={firebaseOptionsServerId}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={firebaseOptionsSenderId}");


                var data = new
                {
                    registration_ids = tokens,
                    notification = new
                    {
                        body = body,
                        title = title,
                    },
                    priority = "high"
                };

                var json = JsonConvert.SerializeObject(data);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("/fcm/send", httpContent);
                return result.StatusCode.Equals(HttpStatusCode.OK);
            }
        }

            private DataContext myDbContext;
                private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            IWebHostEnvironment _webHostEnvironment;
                public ApiController(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
                {
                    myDbContext = context;
                    _mapper = mapper;
                    _webHostEnvironment = webHostEnvironment;
                _httpContextAccessor = httpContextAccessor;
                }

           



               [HttpPost("dashboard/slider/add-slider")]
                public async Task<ActionResult> addSlider([FromForm] Slider modle)
                {
                    await myDbContext.sliders.AddAsync(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/field/add-field")]
                public async Task<ActionResult> addField([FromForm] Field modle)
                {
                    await myDbContext.fields.AddAsync(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/food/add-food")]
                public async Task<ActionResult> addFood([FromForm] Food modle)
                {
                    await myDbContext.foods.AddAsync(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/option/add-option")]
                public async Task<ActionResult> addOption([FromForm] Option modle)
                {
                    await myDbContext.options.AddAsync(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/option-group/add-option-group")]
                public async Task<ActionResult> addOptionGroup([FromForm] OptionGroup modle)
                {
                    await myDbContext.OptionGroups.AddAsync(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

    

                [HttpPost("dashboard/user/add-user")]
                public async Task<ActionResult> addUser([FromForm] User modle)
                {
                    await myDbContext.AspNetUsers.AddAsync(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

          


                [HttpPost("image/u")]
                public ActionResult uploadImage(IFormFile file)
                {
                    string path = _webHostEnvironment.WebRootPath + "uploads/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = System.IO.File.Create(path + file.FileName))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                   
                        return Ok(file.FileName);
                    }

                }

            // ----------------------------------------------------------------------------------------



            [HttpGet("dashboard/home/get-home")]
            public async Task<ActionResult> getDashboardHome()
            {
                int userCount = myDbContext.AspNetUsers.Where(x => x.Role == "user").Count();
                int orderCount = myDbContext.orders.Count();
                int marketCount = myDbContext.markets.Count();
                int earnCount = 2334;
                List<Market> markets = new List<Market>{ };
                var orders = await myDbContext.orders.OrderByDescending(x => x.Id).Take(7).AsNoTracking().ToListAsync();
                List<int> ids = orders.ConvertAll(x =>  x.market_id );
                        
                markets = await myDbContext.markets.Where(x => ids.Contains(x.Id)).AsNoTracking().ToListAsync();
                return Ok(new { orders,markets,userCount,orderCount,marketCount,earnCount});
            }

              [HttpGet("dashboard/user/get-users")]
                public async Task<ActionResult> getusers()
                {
                    var data = await myDbContext.AspNetUsers.Where(x=>x.Role == "user").AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/slider/get-sliders")]
                public async Task<ActionResult> getsliders()
                {
                    var data = await myDbContext.sliders.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/user/get-admins")]
                public async Task<ActionResult> getAdmins()
                {
                    var data = await myDbContext.AspNetUsers.Where(x => x.Role == "admin").AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/driver/get-drivers")]
                public async Task<ActionResult> getdrivers()
                {
                    var data = await myDbContext.drivers.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/market/get-markets")]
                public async Task<ActionResult> getmarkets()
                {
                    var data = await myDbContext.markets.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/food/get-foods")]
                public async Task<ActionResult> getfoods()
                {
                    List<Food> foods = await myDbContext.foods.AsNoTracking().ToListAsync();

                List<int> marketids = foods.ConvertAll(x => x.market_id);
                List<int> catids = foods.ConvertAll(x => x.category_id);

                List<Market> markets = await myDbContext.markets.Where(x => marketids.Contains(x.Id)).AsNoTracking().ToListAsync();
                List<Category> categories = await myDbContext.categories.Where(x => catids.Contains(x.Id)).AsNoTracking().ToListAsync();


                return Ok(new { foods,markets,categories});
                }

                [HttpGet("dashboard/category/get-categories")]
                public async Task<ActionResult> getcategories()
                {
                    var data = await myDbContext.categories.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/field/get-fields")]
                public async Task<ActionResult> getfields()
                {
                    var data = await myDbContext.fields.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/orders/get-orders")]
                public async Task<ActionResult> getorders()
                {
                    var data = await myDbContext.orders.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/option/get-options")]
                public async Task<ActionResult> getoptions()
                {
                    var data = await myDbContext.options.AsNoTracking().ToListAsync();
                    return Ok(data);
                }

                [HttpGet("dashboard/option-group/get-option-groups")]
                public async Task<ActionResult> getOptionGroups()
                {
                    var groups = await myDbContext.OptionGroups.AsNoTracking().ToListAsync();
                List<int> ids = groups.ConvertAll(x => x.food_id);
                List<Food> foods = await myDbContext.foods.Where(x => ids.Contains(x.Id)).AsNoTracking().ToListAsync();


                return Ok(new { groups,foods});
                }

                // ----------------------------------------------------------------------------------------

                [HttpPost("dashboard/user/delete-user")]
                public async Task<ActionResult> deleteUser([FromForm]string id)
                {
                   var item =  await myDbContext.AspNetUsers.Where(x => x.Id == id).FirstAsync();
                    myDbContext.AspNetUsers.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/category/delete-category")]
                public async Task<ActionResult> deleteCategory([FromForm] int id)
                {
                    var item = await myDbContext.categories.Where(x => x.Id == id).FirstAsync();
                    myDbContext.categories.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/slider/delete-slider")]
                public async Task<ActionResult> deleteSlider([FromForm] int id)
                {
                    var item = await myDbContext.sliders.Where(x => x.Id == id).FirstAsync();
                    myDbContext.sliders.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/market/delete-market")]
                public async Task<ActionResult> deleteMarket([FromForm] int id)
                {
                    var item = await myDbContext.markets.Where(x => x.Id == id).FirstAsync();
                    myDbContext.markets.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/food/delete-food")]
                public async Task<ActionResult> deleteFood([FromForm] int id)
                {
                    var item = await myDbContext.foods.Where(x => x.Id == id).FirstAsync();
                    myDbContext.foods.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

             

                [HttpPost("dashboard/field/delete-field")]
                public async Task<ActionResult> deleteField([FromForm] int id)
                {
                    var item = await myDbContext.fields.Where(x => x.Id == id).FirstAsync();
                    myDbContext.fields.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/order/delete-order")]
                public async Task<ActionResult> deleteOrder([FromForm] int id)
                {
                    var item = await myDbContext.orders.Where(x => x.Id == id).FirstAsync();
                    myDbContext.orders.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/option/delete-option")]
                public async Task<ActionResult> deleteOption([FromForm] int id)
                {
                    var item = await myDbContext.options.Where(x => x.Id == id).FirstAsync();
                    myDbContext.options.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }

                [HttpPost("dashboard/option-group/delete-option-group")]
                public async Task<ActionResult> deleteOptionGroup([FromForm] int id)
                {
                    var item = await myDbContext.OptionGroups.Where(x => x.Id == id).FirstAsync();
                    myDbContext.OptionGroups.Remove(item);
                    myDbContext.SaveChanges();
                    return Ok(id);
                }



                // ----------------------------------------------------------------------------------------

                [HttpPost("dashboard/category/update-category")]
                public async Task<ActionResult> updateCategory([FromForm] Category modle)
                {
                    myDbContext.Entry(await myDbContext.categories.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/user/update-user")]
                public async Task<ActionResult> updateUser([FromForm] User modle)
                {
                    myDbContext.Entry(await myDbContext.AspNetUsers.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/field/update-field")]
                public async Task<ActionResult> updateField([FromForm] Field modle)
                {
                    myDbContext.Entry(await myDbContext.fields.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/food/update-food")]
                public async Task<ActionResult> updateFood([FromForm] Food modle)
                {
                    myDbContext.Entry(await myDbContext.foods.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/order/update-order")]
                public async Task<ActionResult> updateOrder([FromForm] Order modle)
                {
                    myDbContext.Entry(await myDbContext.orders.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

              /*  [HttpPost("dashboard/city/update-city")]
                public async Task<ActionResult> updateCity([FromForm] City modle)
                {
                    myDbContext.Entry(await myDbContext.cites.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }*/
                [HttpPost("dashboard/option/update-option")]
                public async Task<ActionResult> updateOption([FromForm] Option modle)
                {
                    myDbContext.Entry(await myDbContext.options.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/option-group/update-option-group")]
                public async Task<ActionResult> updateOptionGroup([FromForm] OptionGroup modle)
                {
                    myDbContext.Entry(await myDbContext.OptionGroups.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }

                [HttpPost("dashboard/market/update-market")]
                public async Task<ActionResult> updateMarket([FromForm] Market modle)
                {
                    myDbContext.Entry(await myDbContext.markets.FirstOrDefaultAsync(x => x.Id == modle.Id)).CurrentValues.SetValues(modle);
                    myDbContext.SaveChanges();
                    return Ok(modle);
                }



                // ----------------------------------------------------------------------------------------


                [HttpGet("mobile/get-home")]
                public async Task<ActionResult> getHome()
                {
                    var fields =await myDbContext.fields.AsNoTracking().ToListAsync();
                    var sliders = await myDbContext.sliders.AsNoTracking().ToListAsync();
                    var mostrated = await myDbContext.markets.OrderByDescending(x => x.rate).Take(10).AsNoTracking().ToListAsync();
                    var readyfordeliver = await myDbContext.markets.OrderByDescending(x => x.order_count).Take(10).AsNoTracking().ToListAsync();
                    return Ok( new { fields,mostrated,sliders,readyfordeliver});
                }

            [HttpGet("mobile/get-user-info")]
            public async Task<ActionResult> getUserInfo([FromForm] int id)
            {
                var user = await myDbContext.AspNetUsers.FindAsync(id);
                return Ok(user);
            }

      
   

            [HttpPost("mobile/get-field-markets")]
            public async Task<ActionResult> getFieldmarkets([FromForm]FieldMarketsRequest modle)
                {
                var myLat = modle.lat;
                var myLon = modle.lng;
                var radiusInMile = 1000000;
                var markets = myDbContext.markets
                       .AsEnumerable()
                       .Select(market => new { market, Dist = distanceInMiles(myLon, myLat, market.lng, market.lat) }).OrderBy(market => market.Dist)
                       .Where(p => p.Dist <= radiusInMile);

                return Ok(markets);
                }


                [HttpPost("mobile/get-market-foods")]
                public async Task<ActionResult> getMarketfoods([FromForm] int id)
                {
                var foods = await myDbContext.foods.Where(x => x.market_id == id).AsNoTracking().ToListAsync();
                    return Ok(foods);
                }

            [HttpPost("mobile/add-cart")]
            public async Task<ActionResult> addCart([FromForm] CartAddRequest modle)
            {

                Cart cart = _mapper.Map<Cart>(modle);
           
              await myDbContext.carts.AddAsync(cart);
                myDbContext.SaveChanges();

                //if (modle.options != null) {
                //    foreach (var item in modle.options)
                //    {
                //        CartGroupOption cartGroupOption = new CartGroupOption
                //        {
                //            cart_id = cart.Id,
                //            group_id = int.Parse(item.Key),
                //            option_id = int.Parse(item.Value),

                //        };
                //        await myDbContext.CartGroupoptions.AddAsync(cartGroupOption);

                //    }
                //}

                //myDbContext.SaveChanges();
                return Ok(cart);
            }

 

            [HttpPost("mobile/get-category-foods")]
            public async Task<ActionResult> getCategoryfoods([FromForm] CategoryFoodRequest model)
            {
                var data = await myDbContext.foods.Where(x => x.market_id == model.marketId).Where(x => x.category_id == model.categoryId).AsNoTracking().ToListAsync();
                return Ok(data);
            }

      

     

            [HttpPost("mobile/add-address")]
            public async Task<ActionResult> addAdress([FromForm] Address modle)
            {

                await myDbContext.addresses.AddAsync(modle);
                myDbContext.SaveChanges();
                return Ok(modle);
            }

            [HttpPost("mobile/driver/update-order-status")]
            public async Task<ActionResult> get([FromForm] UpdateOrderStatusRequest modle)
            {
                Order order = await myDbContext.orders.FindAsync(modle.orderId);
                order.delivery_id = modle.driverId;
                order.status = modle.status;
                var orderDrivers = await myDbContext.driverOrders.Where(x=>x.order_id==modle.orderId).ToArrayAsync();
                myDbContext.driverOrders.RemoveRange(orderDrivers);
                await  myDbContext.SaveChangesAsync();
                return Ok(order);
            }

            //[HttpPost("mobile/user/update-deviceToken")]
            //public async Task<ActionResult> updateToken([FromForm] UpdateTokenRequest modle)
            //{

            //    if (modle.isDriver=="1")
            //    {
            //        Driver driver = await myDbContext.drivers.Where(x => x.user_id == modle.UserId).FirstAsync();
            //        driver.DeviceToken = modle.Token;
            //    }
            //    else {
            //        User user = await myDbContext.AspNetUsers.Where(x => x.Id == modle.UserId).FirstAsync();
            //        user.DeviceToken = modle.Token;
            //    }
            //    await myDbContext.SaveChangesAsync();
           
            //    return Ok("success");

            //}

         /*   [HttpPost("mobile/add-order")]
            public async Task<ActionResult> addOrder([FromForm] Order modle)
            { 
                int totalPrice = 0;
                List<Cart> carts = await myDbContext.carts.Where(x => x.user_id == modle.user_id).Where(x => x.order_id == 0).ToListAsync();
                carts.ForEach(c => { totalPrice += c.price; });
                modle.price = totalPrice;
                await myDbContext.orders.AddAsync(modle);
                await myDbContext.SaveChangesAsync();
                carts.ForEach(c => { c.order_id = modle.Id; });
                //myDbContext.carts.RemoveRange(carts);


                var myLat = modle.user_lat;
                var myLon = modle.user_lng;
                var radiusInMile = 30000000000000000;

               var drivers = myDbContext.drivers
                       .AsEnumerable()
                       .Select(driver => new { driver, Dist = distanceInMiles(myLon, myLat, driver.lng, driver.lat) }).OrderBy(driver => driver.Dist)
                       .Where(p => p.Dist <= radiusInMile).ToList();

                List<string> tokens = new List<string>();

                for (var i = 0; i < drivers.Count; i++) {
                    Driver driver = drivers[i].driver;
                    DriverOrder driverOrder = new DriverOrder() {
                        order_id = modle.Id,
                        driver_id = driver.Id,
                        market_id=modle.market_id
                    };
                    tokens.Add(driver.device_token);
                    await myDbContext.driverOrders.AddAsync(driverOrder);
                }


                string title = "طلبات جديدة";
                string body = " طلبات جديدة"+"  :  "+modle.marketname;

                await SendNotificationAsync(tokens,title,body);

               await myDbContext.SaveChangesAsync();
                return Ok(modle);

            }

        */



        

      

            [HttpPost("mobile/driver/get-market-detail")]
            public async Task<ActionResult> getMarketDetails([FromForm] MarketDetailRequest modle)
            {
                var orders = await myDbContext.orders.Where(x=>x.market_id == modle.marketId&&x.status == 1).AsNoTracking().ToListAsync();

                Market market =await myDbContext.markets.Where(x=>x.Id == modle.marketId).FirstAsync();

                double distance = distanceInMiles(modle.lng, modle.lat, market.lng, market.lat);
                MarketDetailResponse data = new MarketDetailResponse()
                {
                 /*   market = market,
                    distance = distance,
                    orders = orders
                 */
                };
                return Ok(data);
            }




            [HttpPost("mobile/driver/get-order-detail")]
            public async Task<ActionResult> getOrderItems([FromForm] int id)
            {
                var items = myDbContext.carts.Where(x => x.order_id == id).Select(o => new { food =  myDbContext.foods.Where(x => x.Id == o.food_id).First(),quantity = o.quantity});
                var order =await myDbContext.orders.Where(x => x.Id == id).FirstAsync();
                return Ok(new { order, items });
            }


            [HttpPost("mobile/driver/get-orders")]
            public async Task<ActionResult> getCurrentorders([FromForm] int id)
            {
                var orders = await myDbContext.orders.AsNoTracking().ToListAsync();
                return Ok(orders);
            }



            [HttpPost("mobile/user/get-orders")]
            public async Task<ActionResult> getUserOrders([FromForm] string id)
            {
                var orders = await myDbContext.orders.Where(x => x.user_id ==id).AsNoTracking().ToListAsync();
                return Ok(orders);
            }


            [HttpPost("mobile/user/get-info")]

            public async Task<ActionResult> getCurrentUser([FromForm] int id)
            {
                var user = await myDbContext.AspNetUsers.FindAsync(id);
                return Ok(user);
            }



            [HttpPost("mobile/user/get-notifications")]
            public async Task<ActionResult> getUserNotifications([FromForm] int id)
            {
                var notifications = await myDbContext.UserNotifications.Where(x => x.user_id == id).AsNoTracking().ToListAsync();
                return Ok(notifications);
            }

            [HttpPost("mobile/driver/get-profile")]
            public async Task<ActionResult> getDriverProfile([FromForm] string id)
            {
                var driver = await myDbContext.drivers.Where(x => x.user_id == id).FirstAsync();
                return Ok(driver);
            }

            //[HttpPost("mobile/driver/update-order-status")]
            //public async Task<ActionResult> updateorderstatus([FromForm] OrderStatusRequest modle)
            //{
            //    Order order = await myDbContext.orders.Where(x => x.Id == modle.orderId).FirstAsync();
            //    if (order.delivery_id == 0||order.delivery_id == modle.deliveryId) {
            //        order.delivery_id = modle.deliveryId;
            //        order.status = modle.status;
            //       await myDbContext.SaveChangesAsync();
            //        return Ok("تم بنجاح");

            //    }
            //    return Ok("نأسف تم إسناد الطلب لمندوب اخر");
            //}



            [HttpPost("mobile/update-cart-quantity")]
            public async Task<ActionResult> updateCartQuantity([FromForm] UpdateQuantityRequest modle)
            {
                Cart cart = await myDbContext.carts.FindAsync(modle.id);

                if (modle.status == 0) {

                    if (cart.quantity == 1)
                    {
                        myDbContext.carts.Remove(cart);
                    }
                    else {
                        cart.quantity--;

                    }
                }
                else {
                    cart.quantity++;
                }
   
                await myDbContext.SaveChangesAsync();

                return Ok("تم بنحاح");
            }


            [HttpPost("mobile/driver/update-location")]
            public async Task<ActionResult> updateDeliveryLocation([FromForm] UpdateDriverLocationRequest modle)
            {
                Driver driver = await myDbContext.drivers.FindAsync(modle.id);
                driver.lat = modle.lat;
                driver.lng = modle.lng;
               await myDbContext.SaveChangesAsync();

                return Ok("تم بنحاح");
            }

            // ------------------------------------ Driver

            [HttpPost("mobile/driver/nearby-markets")]
            public async Task<ActionResult> getDriverNearbymarkets([FromForm] NearbyMarketsRequest modle)
            {
                var myLat = modle.lat;
                var myLon = modle.lng;
                var radiusInMile = 1000000;

                //var minMilePerLat = 68.703;
                //var milePerLon = Math.Cos(myLat) * 69.172;
                //var minLat = myLat - radiusInMile / minMilePerLat;
                //var maxLat = myLat + radiusInMile / minMilePerLat;
                //var minLon = myLon - radiusInMile / milePerLon;
                //var maxLon = myLon + radiusInMile / milePerLon;

                var markets = myDbContext.markets
                       //.Where(market => (minLat <= market.lat && market.lat <= maxLat) && (minLon <= market.lng && market.lng <= maxLon))
                       .AsEnumerable()
                       .Select(market => new { market, Dist = distanceInMiles(myLon, myLat, market.lng, market.lat) }).OrderBy(market => market.Dist)
                       .Where(p => p.Dist <= radiusInMile);

                return Ok(markets);
            }





            [HttpPost("mobile/driver/search-markets")]
            public async Task<ActionResult> getSearchmarkets([FromForm] SearchMarketRequest searchMarket )
            {
                var myLat = searchMarket.lat;
                var myLon = searchMarket.lng;
                //var radiusInMile = 700000;

                //var minMilePerLat = 68.703;
                //var milePerLon = Math.Cos(myLat) * 69.172;
                //var minLat = myLat - radiusInMile / minMilePerLat;
                //var maxLat = myLat + radiusInMile / minMilePerLat;
                //var minLon = myLon - radiusInMile / milePerLon;
                //var maxLon = myLon + radiusInMile / milePerLon;

                var markets = myDbContext.markets
                   .Where(p => p.title.Contains(searchMarket.searchText))
                       //.Where(market => (minLat <= market.lat && market.lat <= maxLat) && (minLon <= market.lng && market.lng <= maxLon))
                       .AsEnumerable()
                       .Select(market => new { market, Dist = distanceInMiles(myLon, myLat, market.lng, market.lat) }).OrderBy(market => market.Dist)
                       ;

                return Ok(markets);
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

