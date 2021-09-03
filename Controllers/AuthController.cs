using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Donia.Controllers
{

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private UserManager<User> userManager;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        public readonly IWebHostEnvironment _webHostEnvironment;

       public AuthController(IWebHostEnvironment webHostEnvironment,IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration _config, DataContext context
                    ) {
            this._roleManager = roleManager;
            this._mapper = mapper;
            this.userManager = userManager;
            this._config = _config;
            this._webHostEnvironment = webHostEnvironment;
            this._context = context;
        }


     [HttpPost]
        [Route("auth/check-username")]
        public async Task<Object> checkPhone([FromForm] string phone)
        {
            User user =await  _context.users.Where(x => x.UserName == phone).FirstOrDefaultAsync();
            string code = "";
            if (user != null)
            {
                if (user.code == null)
                {
                    code = RandomNumber();
                    user.code = code;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    code = user.code;
                }
                return Ok(new
                {
                    status = 1,
                    code = code,
                });
            }
            else {
                code = RandomNumber();

                return Ok(new
                {
                    status = 0,
                    code = code,
                });
            }
               

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string RandomNumber()
        {

            Random r = new Random();
            int randNum = r.Next(1000);
            string fourDigitNumber = randNum.ToString("D4");
            return fourDigitNumber;
        }

        [HttpPost]
        [Route("/auth/register")]
        public async Task<ActionResult> register([FromForm] UserForRegister model)
        {
            model.Password = "Abc123@";
            var userToCreate = _mapper.Map<User>(model);
            userToCreate.Role = "user";
            userToCreate.ProfileImage = Functions.upload(model.image, _webHostEnvironment);
            if (!await _roleManager.RoleExistsAsync("user"))
                await _roleManager.CreateAsync(new IdentityRole("user"));
            var result = await userManager.CreateAsync(userToCreate, model.Password);

            await userManager.AddToRoleAsync(userToCreate, "user");
            string code = RandomNumber();
            userToCreate.code = code;
            await _context.SaveChangesAsync();
            return Ok(new { success=true,code=code});
        }


        [HttpPost]
        [Route("auth/validate")]
        public async Task<ActionResult> validate([FromForm] UserForValidate userForValidate)
        {
            string error = "";
            User user =await _context.users.Where(x => x.UserName == userForValidate.UserName).FirstOrDefaultAsync();
            if (user != null) {
                error = "رقم الهاتف مسجل من قبل";
                return this.StatusCode(StatusCodes.Status200OK, error);
            }
             user = await _context.users.Where(x => x.Email == userForValidate.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                error = "البريد الإلكتروني مسجل من قبل";
                return BadRequest(error);
            }
            user = await _context.users.Where(x => x.knownName == userForValidate.NameKnown).FirstOrDefaultAsync();
            if (user != null)
            {
                error = " اسم المستخدم مسجل من قبل";
                return BadRequest(error);
            }

            return Ok("success");
        }


  





        [HttpPost]
        [Route("/auth/confirm-code")]
        public async Task<IActionResult> Login([FromForm] UserForLogin model)
        {
            var user = await userManager.FindByNameAsync(model.userName);
            if (user != null )
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(10),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user,
                    expiration = token.ValidTo,

                });
            }
            return Unauthorized();
        }
    }
}
