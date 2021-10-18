using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

class Functions
{
    public static Functions slt = new Functions();

    public static string upload(IFormFile file, IWebHostEnvironment _webHostEnvironment)
    {

        Random rd = new Random();

        int rand_num = rd.Next(100, 1000000);
        string path = _webHostEnvironment.WebRootPath + "/uploads/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (var fileStream = System.IO.File.Create(path + rand_num.ToString() + ".png"))
        {
            file.CopyTo(fileStream);
            fileStream.Flush();

            return rand_num.ToString() + ".png";
        }

    }

    public async Task<bool> SendNotificationAsync(List<string> userIds, string title, string body, DataContext context)
    {

        List<string> tokens = 
             userIds
         .Select(x => context.Users.Where(u=>u.Id == x).First().DeviceToken)
          .ToList();

        using (var client = new HttpClient())
        {
            var firebaseOptionsServerId = "AAAAy2vBM78:APA91bFK6gL93jsWFegHQHotUFz796Ak07hkPjo2teNMrT5C2KFmfuv4ZM9qjE9l1MdT_FChgVYxS0haGLdvyf2LdXm3d3xYeRQb2SSQtsGRM1GWg8NcD5670ahx52gucfak4AWOlYRo";
            var firebaseOptionsSenderId = "873686184895";

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
                data=new  {
                    orderId =1
                },
                priority = "high"
            };

            var json = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await client.PostAsync("/fcm/send", httpContent);
            return result.StatusCode.Equals(HttpStatusCode.OK);
        }
    }

    public static string uploadVideo(IFormFile file, IWebHostEnvironment _webHostEnvironment)
    {

        Random rd = new Random();

        int rand_num = rd.Next(100, 1000000);
        string path = _webHostEnvironment.WebRootPath + "/uploads/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (var fileStream = System.IO.File.Create(path + rand_num.ToString() + ".mp4"))
        {
            file.CopyTo(fileStream);
            fileStream.Flush();

            return rand_num.ToString() + ".mp4";
        }

    }

    public static async Task<User> getCurrentUser(IHttpContextAccessor _httpContextAccessor, DataContext _context)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _context.Users.FindAsync(userId);
        return user;
    }
}