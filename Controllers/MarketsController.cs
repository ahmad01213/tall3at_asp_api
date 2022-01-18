using AutoMapper;
using AutoMapper.Configuration;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Controllers
{


    public class MarketsController: Controller
    {
        private readonly IMapper _mapper;
        private readonly DataContext myDbContext;
        public readonly IWebHostEnvironment _webHostEnvironment;

        public MarketsController(IWebHostEnvironment webHostEnvironment, IMapper mapper,   DataContext context
                     )
        {
            this._mapper = mapper;
            this._webHostEnvironment = webHostEnvironment;
            this.myDbContext = context;
        }
        [HttpGet("get-home")]
        public async Task<ActionResult> getHome(int addressId)
        {
            if (addressId == 0 || addressId == null) addressId = 4;

            var fields = await myDbContext.fields.AsNoTracking().ToListAsync();
            Address address = await myDbContext.addresses.Where(x => x.Id == addressId).FirstOrDefaultAsync();
            var myLat = address.lat;
            var myLon = address.lng;
            var radiusInMile = 50;
            var fods = myDbContext.foods
           .AsEnumerable()
           .Select(f => new {
               f,
               Dist = distanceInMiles(myLon, myLat, f.lng, f.lat)
           }).OrderByDescending(market => market.f.Id)
           .Where(p => p.Dist <= radiusInMile);

            List<FoodDetailResponse> foods = new List<FoodDetailResponse>();
            foreach (var food in fods) {
                List<Photo> photos = await myDbContext.photos.Where(x => x.Modle == "food" && x.ModleId == food.f.Id.ToString()).ToListAsync();
                FoodDetailResponse foodDetail = new FoodDetailResponse() { 
                 food = food.f,
                 photos = photos
                };
                foods.Add(foodDetail);
            }
            return Ok(new { fields, foods });
        }





        [HttpGet("market/detail")]
        public async Task<ActionResult> detail([FromForm]int marketId)
        {
            Market market = await myDbContext.markets.Where(x => x.Id == marketId).FirstAsync();

            List<FieldMarket> fieldMarkets = await myDbContext.fieldMarkets.Where(x => x.market_id == marketId).ToListAsync();
            List<Field> fields = new List<Field>();
            foreach (var fm in fieldMarkets)
            {
                var field = await myDbContext.fields.Where(x => x.Id == fm.field_id).FirstAsync();
                fields.Add(field);
            }

            var fods = await myDbContext.foods.AsNoTracking().ToListAsync();
            List<FoodDetailResponse> foods = new List<FoodDetailResponse>();
            foreach (var food in fods)
            {
                List<Photo> photos = await myDbContext.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();
                FoodDetailResponse foodDetail = new FoodDetailResponse()
                {
                    food = food,
                    photos = photos

                };
                foods.Add(foodDetail);
            }

            MarketDetailResponse marketDetail = new MarketDetailResponse()
            {
                market = market,
                fields=fields,
                foods = foods,

            };
            return Ok(marketDetail);
        }

        [HttpPost("market/search")]
        public async Task<ActionResult> searchMarket([FromForm]SearchRequest searRequest)
        {
            var radiusInMile = 50;
            List<MarketDetailResponse> markets = new List<MarketDetailResponse>();
            var mrkts = myDbContext.markets
               .Where(p => p.title.Contains(searRequest.search))

                   .AsEnumerable()
                   .Select(market => new { market, Dist = distanceInMiles(searRequest.lng, searRequest.lat, market.lng, market.lat) }).OrderBy(market => market.Dist);

            ;

            foreach (var market in mrkts)
            {

                List<FieldMarket> fieldMarkets = await myDbContext.fieldMarkets.Where(x => x.market_id == market.market.Id).ToListAsync();
                List<Field> fields = new List<Field>();
                foreach (var fm in fieldMarkets)
                {
                    var field = await myDbContext.fields.Where(x => x.Id == fm.field_id).FirstAsync();
                    fields.Add(field);
                }

                var fods = await myDbContext.foods.Where(x=>x.market_id==market.market.Id).AsNoTracking().ToListAsync();
                List<FoodDetailResponse> foods = new List<FoodDetailResponse>();
                foreach (var food in fods)
                {
                    List<Photo> photos = await myDbContext.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();
                    FoodDetailResponse foodDetail = new FoodDetailResponse()
                    {
                        food = food,
                        photos = photos

                    };
                    foods.Add(foodDetail);
                }

                MarketDetailResponse marketDetail = new MarketDetailResponse()
                {
                    market = market.market,
                    fields = fields,
                    foods = foods,
                    dist = market.Dist.ToString()
                };

                markets.Add(marketDetail);
            }
            

            return Ok(markets);

        }


        [Authorize]
        [HttpPost("market/update")]
        public async Task<ActionResult> updateMarket([FromForm] MarketForAddDto marketForAdd)
        {
            Market oldMarket= await myDbContext.markets.Where(x => x.user_id == marketForAdd.user_id).FirstAsync();
            var oldFields = await myDbContext.fieldMarkets.Where(x=>x.market_id==oldMarket.Id).ToListAsync();
            myDbContext.fieldMarkets.RemoveRange(oldFields);
            Market market = _mapper.Map<Market>(marketForAdd);
            market.Id = oldMarket.Id;
             myDbContext.markets.Remove(oldMarket);
            await myDbContext.SaveChangesAsync();
            await myDbContext.markets.AddAsync(market);
            var user = await myDbContext.Users.Where(x => x.Id == marketForAdd.user_id).FirstAsync();
            var fields = marketForAdd.fields.Split("#");
            foreach (var id in fields)
            {
                FieldMarket fieldMarket = new FieldMarket()
                {
                    field_id = int.Parse(id),
                    market_id = market.Id,
                    lat = market.lat,
                    lng = market.lng
                };
                await myDbContext.fieldMarkets.AddAsync(fieldMarket);
            }
            await myDbContext.SaveChangesAsync();
            return Ok(market);
        }


        [HttpPost("image/upload")]
        public ActionResult uploadImage([FromForm]PhotoForAddDto photoForAdd)
        {
            string path = _webHostEnvironment.WebRootPath + "/uploads/";
           IFormFile file = photoForAdd.file;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            String fileName = DateTime.Now.ToString("yyyyMMddTHHmmss")+".jpeg";
            using (var fileStream = System.IO.File.Create(path +fileName ))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
                Photo image = new Photo()
                {
                    Url = fileName,
                    Modle = photoForAdd.Modle,
                    ModleId = photoForAdd.ModleId
                };
                myDbContext.photos.AddAsync(image);
                myDbContext.SaveChanges();
                return Ok(fileName);
            }

        }

        [HttpPost("image/delete")]
        public async Task<ActionResult> deleteImageAsync([FromForm] string id)
        {
            var item = await myDbContext.photos.Where(x =>x.Url == id).FirstAsync();
            myDbContext.photos.Remove(item);
            myDbContext.SaveChanges();
            return Ok(id);

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
