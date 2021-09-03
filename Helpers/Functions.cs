using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

class Functions
{
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