using AutoMapper;
using Donia.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Controllers
{
    public class UsersController : Controller
    {

        private readonly DataContext _context;
        public UsersController(IMapper mapper, DataContext context
             )
        {
            this._context = context;
        }


        [Authorize(Roles = "admin")]
        [HttpPost("user/get-users")]
        public async Task<ActionResult> getUsers()
        {
            var users = await _context.Users.Where(x => x.Role == "user").AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpPost("user/get-providers")]
        public async Task<ActionResult> getProviders()
        {
            var users = await _context.Users.Where(x=>x.Role=="provider").AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpPost("user/get-drivers")]
        public async Task<ActionResult> getDrivers()
        {
            var users = await _context.Users.Where(x => x.Role == "driver").AsNoTracking().ToListAsync();
            return Ok(users);
        }
    } 
}
