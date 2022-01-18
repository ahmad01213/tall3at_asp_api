using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Authorization;
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


        [HttpPost("auth/check-username")]
        public async Task<Object> checkPhone([FromForm] string phone)
        {
            User user =await  _context.AspNetUsers.Where(x => x.UserName == phone).FirstOrDefaultAsync();
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

                await Functions.slt.SendSmsAsync(code,user.UserName);
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

        [HttpPost("auth/signup")]
        public async Task<ActionResult> register([FromForm] UserForRegister model)
        {
            model.Password = "Abc123@";
            var userToCreate = _mapper.Map<User>(model);
            userToCreate.Role = model.Role;
            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            var result = await userManager.CreateAsync(userToCreate, model.Password);

            await userManager.AddToRoleAsync(userToCreate, model.Role);
            string code = RandomNumber();
            userToCreate.code = code;
            await _context.SaveChangesAsync();
            return Ok(new { success=true,code=code});
        }

        [HttpPost("auth/driver-signup")]
        public async Task<ActionResult> registerDriver([FromForm] DriverForRegister model)
        {
            model.Password = "Abc123@";
            var userToCreate = _mapper.Map<User>(model);
            userToCreate.Role = "driver";
            if (!await _roleManager.RoleExistsAsync("driver"))
                await _roleManager.CreateAsync(new IdentityRole("driver"));
            var result = await userManager.CreateAsync(userToCreate, model.Password);
            await userManager.AddToRoleAsync(userToCreate, "driver");
            Driver driver = _mapper.Map<Driver>(model);
            driver.user_id = userToCreate.Id;
            await _context.drivers.AddAsync(driver);
            string code = RandomNumber();
            userToCreate.code = code;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, code = code });
        }


        [Authorize(Roles = "user,provider")]
        [HttpPost("market/add")]
        public async Task<ActionResult> addMarket([FromForm] MarketForAddDto marketForAdd)
        {
            Market market = _mapper.Map<Market>(marketForAdd);
            await _context.markets.AddAsync(market);
            var user = await _context.Users.Where(x => x.Id == marketForAdd.user_id).FirstAsync();
            user.Role = "provider";

            if (!await _roleManager.RoleExistsAsync("provider"))
             await _roleManager.CreateAsync(new IdentityRole("provider"));

            await userManager.AddToRoleAsync(user, "provider");

            await _context.SaveChangesAsync();

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
                await _context.fieldMarkets.AddAsync(fieldMarket);
            }
            await _context.SaveChangesAsync();
            return Ok(market);
        }



        private static readonly HttpClient client = new HttpClient();

        [HttpPost("auth/sms")]
        public async Task<object> SendSmsAsync()
        {

            var values = new Dictionary<string, string>
{
    { "user", "MATBAKH24" },
    { "pass", "Hassan_ali321@" },
    { "to", "+966580016813" },
    { "message", "12345" },
    { "sender", "MATBAKH24" },
};

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.jawalbsms.ws/api.php/sendsms", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;

        }


        [HttpPost("auth/validate")]
        public async Task<ActionResult> validate([FromForm]UserForValidate userForValidate)
        {
            string error = "";
            User user =await _context.AspNetUsers.Where(x => x.UserName == userForValidate.UserName).FirstOrDefaultAsync();
            if (user != null) {
                error = "رقم الهاتف مسجل من قبل";
                return this.StatusCode(StatusCodes.Status200OK, error);
            }
             user = await _context.AspNetUsers.Where(x => x.Email == userForValidate.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                error = "البريد الإلكتروني مسجل من قبل";
                return BadRequest(error);
            }

            return Ok("success");
        }

        [HttpPost("/auth/admin-login")]
        public async Task<IActionResult> LoginAdmin([FromForm]string userName, [FromForm] string password)
        {
            var loginUser = await userManager.FindByNameAsync(userName);
            if (loginUser != null && await userManager.CheckPasswordAsync(loginUser, password))
            {
                var userRoles = await userManager.GetRolesAsync(loginUser);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginUser.Id),
                    new Claim(ClaimTypes.Name, loginUser.UserName),
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
                    expires: DateTime.Now.AddDays(100),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                UserDetailResponse user = _mapper.Map<UserDetailResponse>(loginUser);
               
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user,
                
                    userRoles,
                    expiration = token.ValidTo,

                });
            }
            return Unauthorized();
        }


       



            [HttpPost("/auth/confirm-code")]
        public async Task<IActionResult> Login([FromForm] UserForLogin model)
        {
            var loginUser = await userManager.FindByNameAsync(model.userName);
            if (loginUser != null &&loginUser.code==model.code)
            {
                 var userRoles = await userManager.GetRolesAsync(loginUser);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginUser.Id),
                    new Claim(ClaimTypes.Name, loginUser.UserName),
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
                    expires: DateTime.Now.AddDays(100),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                UserDetailResponse user = _mapper.Map<UserDetailResponse>(loginUser);
                Driver driver = null;

                MarketDetailResponse market = null;
                var userMarket = await _context.markets.Where(x => x.user_id == user.Id).FirstOrDefaultAsync();

                if (userMarket != null)
                {


                    List<Field> fields = new List<Field>();

                    var marketFields = await _context.fieldMarkets.Where(x => x.market_id == userMarket.Id).ToArrayAsync();
                    foreach (var mf in marketFields)
                    {
                        var field = await _context.fields.Where(x => x.Id == mf.field_id).FirstAsync();
                        fields.Add(field);
                    }


                    var fods = await _context.foods.Where(x => x.market_id == userMarket.Id).AsNoTracking().ToListAsync();
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



                    market = new MarketDetailResponse
                    {
                        fields = fields,
                        market = userMarket,
                        foods = foods
                    };
                }
                if (user.Role == "driver") driver = await _context.drivers.Where(x => x.user_id == user.Id).FirstAsync();
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user,
                    driver,
                    market
                    ,
                    userRoles,
                    expiration = token.ValidTo,

                });
            }
            return Unauthorized();
        }



        [Authorize]
        [HttpPost("auth/update-deviceToken")]
        public async Task<ActionResult> updateToken([FromForm] UpdateTokenRequest modle)
        {

            User user = await _context.Users.Where(x => x.Id == modle.UserId).FirstAsync();
            user.DeviceToken = modle.Token;

            await _context.SaveChangesAsync();
            return Ok("success");

        }



        //[Authorize]
        [HttpPost("auth/user/detail")]
        public async Task<IActionResult> getUserDetail([FromForm]string id)
        {
            User user = await _context.Users.Where(x => x.Id == id).FirstAsync();
            Driver driver = null;
            var addresses= await  _context.addresses.Where(x => x.user_id == id).ToListAsync();
            MarketDetailResponse market = null;
            var userMarket = await _context.markets.Where(x => x.user_id == user.Id).FirstOrDefaultAsync();

            if (userMarket !=null)
            {


                List<Field> fields = new List<Field>();

                var marketFields = await _context.fieldMarkets.Where(x => x.market_id == userMarket.Id).ToArrayAsync();
                foreach (var mf in marketFields)
                {
                    var field = await _context.fields.Where(x => x.Id == mf.field_id).FirstAsync();
                    fields.Add(field);
                }


                var fods = await _context.foods.Where(x => x.market_id == userMarket.Id).AsNoTracking().ToListAsync();
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



                market = new MarketDetailResponse
                {
                    fields = fields,
                    market = userMarket,
                    foods = foods
                };
            }

            if (user.Role == "driver") driver = await _context.drivers.Where(x => x.user_id == user.Id).FirstAsync();
            return Ok(new
            {
                user= _mapper.Map<UserDetailResponse>(user),
                driver,
                market,
                addresses
            });

        }


    }
}
