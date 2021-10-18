using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace Donia.Controllers
{
    public class CategoriesController:Controller
    {
        private readonly IMapper _mapper;
        private readonly DataContext myDbContext;
        public readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(IWebHostEnvironment webHostEnvironment, IMapper mapper, DataContext context
                     )
        {
            this._mapper = mapper;
            this._webHostEnvironment = webHostEnvironment;
            this.myDbContext = context;
        }

        [HttpPost("category/add")]
        public async Task<ActionResult> addField(Category modle)
        {
            await myDbContext.categories.AddAsync(modle);
            myDbContext.SaveChanges();
            return Ok(modle);
        }



        [HttpPost("admin/mail")]
        public Task Execute()
        {
            // create message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("Info@matbakh24.com");
                email.Sender.Name = "Douroosi";
            email.From.Add(email.Sender);
            email.To.Add(MailboxAddress.Parse("aebrahima830@gmail.com"));
            email.Subject = "jdksahdjksgf";
            email.Body = new TextPart("plain") { Text = "jdklashfksjdgfjksdg" };

            // send email
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("matbakh24.com", 465);
                smtp.Authenticate("Info@matbakh24.com", "Matbakh24info.2021");
                smtp.Send(email);
                smtp.Disconnect(true);
            }

            return Task.FromResult(true);
        }
    }
}
