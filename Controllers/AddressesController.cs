using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Controllers
{
    public class AddressesController : Controller
    {

        private readonly DataContext myDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AddressesController( DataContext context, IHttpContextAccessor httpContextAccessor
                     )
        {
            this.myDbContext = context;
            this._httpContextAccessor = httpContextAccessor;

        }

        [Authorize]
        [HttpGet("address/get-addresses")]
        public async Task<ActionResult> getAdresses()
        {
            User user = await Functions.getCurrentUser(_httpContextAccessor, myDbContext);
            var data = await myDbContext.addresses.Where(x => x.user_id == user.Id).AsNoTracking().ToListAsync();
            return Ok(data);
        }


        [Authorize]
        [HttpPost("address/add")]
        public async Task<ActionResult> addAdress(Address address)
        {
            User user = await Functions.getCurrentUser(_httpContextAccessor, myDbContext);
            address.user_id = user.Id;
            await myDbContext.addresses.AddAsync(address);
            myDbContext.SaveChanges();
            return Ok(address);
        }


        [Authorize]
        [HttpPost("address/delete")]
        public async Task<ActionResult> deleteAddress(int id)
        {
            Address address = await myDbContext.addresses.FindAsync(id);
            myDbContext.addresses.Remove(address);
            myDbContext.SaveChanges();
            return Ok(address);
        }

    }
}
