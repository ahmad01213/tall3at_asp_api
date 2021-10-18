using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Controllers
{
    public class FoodsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly DataContext myDbContext;
        public readonly IWebHostEnvironment _webHostEnvironment;

        public FoodsController(IMapper mapper, DataContext context, IWebHostEnvironment webHostEnvironment
                     )
        {
            this._mapper = mapper;
            this.myDbContext = context;
            this._webHostEnvironment = webHostEnvironment;
        }


        [Authorize(Roles = "provider,user")]
        [HttpPost("food/add")]
        public async Task<ActionResult> addFood(FoodForAdd foodForAdd)
        {
            
            Food food = _mapper.Map<Food>(foodForAdd);
            food.price = (int)(food.price * 1.25);
            await myDbContext.foods.AddAsync(food);
            await myDbContext.SaveChangesAsync();
            var imagesToAdd = foodForAdd.images.Split("#");
            var images =   await myDbContext.photos
            .Where(p => imagesToAdd.Contains(p.Url))
            .ToListAsync();
            images.ForEach(img => { img.ModleId = food.Id.ToString(); });
            await myDbContext.SaveChangesAsync();

            return Ok(food);
        }

        [HttpGet("food/search")]
        public async Task<ActionResult> searchFood(string searchText)
        {
            List<FoodDetailResponse> foods = new List<FoodDetailResponse>();
            List<Food> fods =  myDbContext.foods.Where(x => x.name.Contains(searchText)).ToList();
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
            return Ok(foods);
        }


        [HttpPost("food/delete")]
        public async Task<ActionResult> deleteFood([FromForm] int id)
        {
            string dirPath = _webHostEnvironment.WebRootPath + "/uploads/";

            var food = await myDbContext.foods.Where(x => x.Id == id).FirstAsync();
            List<Photo> photos = await myDbContext.photos.Where(x => x.Modle == "food" && x.ModleId == food.Id.ToString()).ToListAsync();


            foreach (var item in photos)
            {
                string fullPath = dirPath +item.Url;

                System.IO.File.Delete(fullPath);

            }
            List<Cart> carts = await myDbContext.carts.Where(x => x.food_id==id).ToListAsync();
            myDbContext.carts.RemoveRange(carts);
            myDbContext.photos.RemoveRange(photos);
            myDbContext.foods.Remove(food);
            await myDbContext.SaveChangesAsync();
            return Ok(id);
        }
    }
}
