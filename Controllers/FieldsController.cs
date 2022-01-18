using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Controllers
{
    public class FieldsController : Controller
    {

        private readonly IMapper _mapper;
        private readonly DataContext myDbContext;

        public FieldsController( IMapper mapper,  DataContext context
                     )
        {
            this._mapper = mapper;
            this.myDbContext = context;
        }

        [HttpPost("field/add")]
        public async Task<ActionResult> addField(Field modle)
        {
            await myDbContext.fields.AddAsync(modle);
            myDbContext.SaveChanges();
            return Ok(modle);
        }

        [HttpPost("field/get-fields")]
        public async Task<ActionResult> getFields()
        {
           var fields =  await myDbContext.fields.AsNoTracking().ToListAsync();
            return Ok(fields);
        }
        [HttpGet("field/markets")]
        public async Task<ActionResult> getMarkets(int fieldId,int addressId)
        {
            if (addressId == 0|| addressId == null) addressId = 4;
            List<MarketDetailResponse> markets = new List<MarketDetailResponse>();
            Address address = await myDbContext.addresses.Where(x => x.Id == addressId).FirstOrDefaultAsync();

            var myLat = address.lat;
            var myLon = address.lng;
            var radiusInMile = 30;
            var fieldMarkets = myDbContext.fieldMarkets.Where(x => x.field_id == fieldId)
           .AsEnumerable()
           .Select(fm => new { fm, 
            Dist = distanceInMiles(myLon, myLat, fm.lng, fm.lat)
           }).OrderBy(market => market.Dist)
           .Where(p => p.Dist <= radiusInMile);
            foreach (var fm in fieldMarkets)
            {
                List<Field> fields = new List<Field>();

                var market = await myDbContext.markets.Where(x => x.Id == fm.fm.market_id).FirstAsync();
                var marketFields = await myDbContext.fieldMarkets.Where(x => x.market_id == market.Id).ToArrayAsync();
                foreach (var mf in marketFields)
                {
                    var field = await myDbContext.fields.Where(x => x.Id == mf.field_id).FirstAsync();
                    fields.Add(field);
                }

                var fods = await myDbContext.foods.Where(x=>x.market_id==market.Id).AsNoTracking().ToListAsync();
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
                MarketDetailResponse marketDetail = new MarketDetailResponse
                {
                    fields = fields,
                    market = market,
                    foods=foods,
                    dist = fm.Dist.ToString()

                };
                markets.Add(marketDetail);
                }
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
