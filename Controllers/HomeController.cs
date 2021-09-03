using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Donia.Dtos;
using Donia.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Donia.Controllers
{
public class HomeController : ControllerBase
{


    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public readonly IWebHostEnvironment _webHostEnvironment;


    public HomeController(DataContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment) {
        this._context = context;
        this._mapper = mapper;
        this._webHostEnvironment = webHostEnvironment;
    }


    [HttpPost]
    [Route("add-trip")]
    public async Task<ActionResult> addTrip([FromForm] TripAdd tripAdd)
    {
        var tripToCreate = _mapper.Map<Trib>(tripAdd);

        var result = await _context.trips.AddAsync(tripToCreate);
        await _context.SaveChangesAsync();
        List<User> organizers = await _context.users.Where(x => x.Role == "provider").ToListAsync();
        foreach (User user in organizers) {
           await SendNotificationAsync(user.Id,
               "طلعات خاصة",
               "هناك طلعة خاصة يمكنك تقديم عرضك الان",
               "special",
               tripToCreate.id.ToString(),
               "a.jpg");
        }
        return Ok(tripAdd);

    }


    [HttpPost]
    [Route("get-trips")]
    public async Task<ActionResult> getTrips()
    {
        List<Trib> trips = await _context.trips.ToListAsync();
        foreach (Trib trip in trips) {
            trip.user = await _context.users.Where(x => x.Id == trip.userId).FirstAsync();
        }
        return Ok(trips);

    }

    [HttpPost]
    [Route("delete-trip")]
    public async Task<ActionResult> deleteTrip([FromForm] int tripId)
    {
        var result = await _context.trips.Where(x=>x.id== tripId).FirstAsync();
        _context.trips.Remove(result);
       await _context.SaveChangesAsync();
        return Ok(result);

    }




    [HttpPost]
    [Route("edit-trip")]
    public async Task<ActionResult> editTrip([FromForm] Trib trip)
    {
        Trib tripToUpdate = await _context.trips.Where(x => x.id == trip.id).FirstAsync();
        _context.trips.Update(tripToUpdate);
        await _context.SaveChangesAsync();

        List<Triporg> offers = await _context.triporgs.Where(x => x.tripId == tripToUpdate.id).ToListAsync();
        _context.triporgs.RemoveRange(offers);
        List<User> organizers = await _context.users.Where(x => x.Role == "provider").ToListAsync();

        foreach (User user in organizers)
        {
            await SendNotificationAsync(user.Id,
                   "طلعات خاصة", 
                "هناك طلعة خاصة يمكنك تقديم عرضك الان",
                "special",
                tripToUpdate.id.ToString(),
                "a.jpg");
        }
        return Ok(tripToUpdate);
    }

    [HttpPost]
    [Route("accept-trip-offer")]
    public async Task<ActionResult> acceptTripOffer([FromForm] OfferForAccept offerForAccept)
    {
        Triporg offer = await _context.triporgs.Where(x => x.Id == offerForAccept.offerId).FirstAsync();
        Trib trib = await _context.trips.Where(x => x.id == offerForAccept.tripId).FirstAsync();
        trib.orgId = offerForAccept.orgId;
        trib.status = 1;
        trib.price = offer.price;
        await _context.SaveChangesAsync();
        await SendNotificationAsync(offer.orgId,
            "طلعات خاصة", 
            "تم تأكيد عرضك علي الطلعة رقم "+trib.id.ToString(),
            "special",
            trib.id.ToString(),
            "a.jpg");
        
        return Ok("success");
    }




    [HttpPost]
    [Route("get-user-trips")]
    public async Task<ActionResult> getUserTrips([FromForm]string userId)
    {
        List<UserTripResponse> userTrips = new List<UserTripResponse>();
        List<Trib> trips = await _context.trips.Where(x=>x.userId==userId).ToListAsync();
        foreach (Trib trip in trips)
        {
            UserTripResponse userTripResponse =  _mapper.Map<UserTripResponse>(trip);
            int offers = _context.triporgs.Where(x => x.tripId == trip.id).Count();
            if (trip.status == 1) userTripResponse.user = await _context.users.Where(x=>x.Id == trip.orgId).FirstAsync();
            if (trip.status == 0&&offers>0) userTripResponse.offered = 1;
            userTrips.Add(userTripResponse);
        }
        return Ok(userTrips);
    }

    [HttpPost]
    [Route("add-trip-offer")]
    public async Task<ActionResult> addTripOffer([FromForm] Triporg model)
    {
        var result = await _context.triporgs.AddAsync(model);
        Trib trip = await _context.trips.Where(x=>x.id==model.tripId).FirstAsync();
        await _context.SaveChangesAsync();
        await SendNotificationAsync(trip.userId,
            "طلعات خاصة", 
            "هناك عرض جديد علي طلعتك رقم"+trip.id.ToString(),
            "special",
            trip.id.ToString(),
            "a.jpg");
        return Ok(model);

    }


    [HttpPost]
    [Route("user/update-name")]
    public async Task<ActionResult> UpdateUserFullName([FromForm] UserForUpdate userForUpdate)
    {
        User user = await _context.users.Where(x => x.Id == userForUpdate.userId).FirstAsync();
        user.FullName = userForUpdate.name;
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost]
    [Route("user/update-summery")]
    public async Task<ActionResult> UpdateUserSummury([FromForm] UserForUpdate userForUpdate)
    {
        User user = await _context.users.Where(x => x.Id == userForUpdate.userId).FirstAsync();
        user.AboutText = userForUpdate.aboutText;
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost]
    [Route("user/update-Bannar")]
    public async Task<ActionResult> UpdateUserBanar([FromForm] UserForUpdate userForUpdate)
    {
        User user = await _context.users.Where(x => x.Id == userForUpdate.userId).FirstAsync();
        user.BanarImage = Functions.upload(userForUpdate.profileImage,_webHostEnvironment);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost]
    [Route("user/update-avatar")]
    public async Task<ActionResult> UpdateUserProfileImage([FromForm] UserForUpdate userForUpdate)
    {
        User user = await _context.users.Where(x => x.Id == userForUpdate.userId).FirstAsync();
        user.BanarImage = Functions.upload(userForUpdate.profileImage, _webHostEnvironment);
        await _context.SaveChangesAsync();
        return Ok(user);
    }


    [HttpPost]
    [Route("user/get-page-profile")]
    public async Task<ActionResult> getPageProfile([FromForm] string userId)
    {
        User user_data = await _context.users.Where(x => x.Id == userId).FirstAsync();
        List<Service> servicesList = await _context.services.Where(x => x.UserId == userId).ToListAsync();
        List<ServiceResponse> trips = servicesList.ConvertAll<ServiceResponse>(s=>new ServiceResponse {service=s,user=_context.users.Where(x=>x.Id==s.UserId).First(),Expired = false });
        List<Photo> photoList = await _context.photos.Where(x =>x.Modle=="user"&& x.ModleId == userId).ToListAsync();
        List<string> photos = photoList.ConvertAll<string>(a => a.Url);

        return Ok(new { user_data, trips, photos });

    }

    [Route("organizer/get-comments")]
    public async Task<ActionResult> getOrgComments([FromForm] string id)
    {
        List<Comment> comments = await _context.comments.Where(x => x.OrgId == id).ToListAsync();
        return Ok(comments);
    }




     [HttpPost]
    [Route("organizer/add-comment")]
    public async Task<ActionResult> addCommentToOrganizer([FromForm] Comment comment)
    {
        Comment commentForCreate = await _context.comments.Where(x => x.UserId == comment.UserId).FirstAsync();
        if (commentForCreate == null)
        {
            await _context.comments.AddAsync(comment);
        }
        else {
            _context.comments.Update(comment);
        }
        await _context.SaveChangesAsync();
        return Ok(comment);

    }






    [Route("home")]
   [HttpPost]
   public async Task<ActionResult> getAppHomeData([FromForm] string user_id)
    {
        List<Category> categories = await _context.categories.ToListAsync();
        List<Service> servicesList = await _context.services.OrderByDescending(x => x.Id).ToListAsync();
        List<ServiceResponse> services = servicesList.ConvertAll<ServiceResponse>(s => new ServiceResponse { service = s, user = _context.users.Where(x => x.Id == s.UserId).First() });
        List<Story> storiesList = await _context.stories.OrderByDescending(x => x.Id).ToListAsync();
        List<StoryResponse> stories = storiesList.ConvertAll<StoryResponse>(s => new StoryResponse { story = s, user = _context.users.Where(x => x.Id == s.UserId).First() });

        List<User> providers = await _context.users.Where(x=>x.Role=="provider").OrderByDescending(x => x.Id).ToListAsync();


        if (user_id != null)
        {
            User user_data = await _context.users.Where(x => x.Id == user_id).FirstAsync();
            return Ok(new { categories, services, stories, providers, user_data });
        }
        else {
            return Ok(new { categories, services, providers, stories });
        }
    }

    [HttpPost]
    [Route("service/update-lnk")]
    public async Task<ActionResult> updateServiceLnk([FromForm] ServiceForUpdateDto serviceForUpdate)
    {
        Service service = await _context.services.Where(x => x.Id == serviceForUpdate.id).FirstAsync();
        service.Lnk = serviceForUpdate.lnk;
        await _context.SaveChangesAsync();
        return Ok(service);

    }

    [HttpPost]
    [Route("user/update-device-token")]
    public async Task<ActionResult> updateDeviceToken([FromForm] UpdateDeviceToken updateDeviceToken)
    {
        User user = await _context.users.Where(x => x.Id == updateDeviceToken.userId).FirstAsync();
        user.DeviceToken = updateDeviceToken.deviceToken;
        await _context.SaveChangesAsync();
        return Ok(user);

    }


    [HttpPost]
    [Route("service/city/get-services")]
    public async Task<ActionResult> getSearchData([FromForm]string city)
    {
        List<Service> servicesList = await _context.services.Where(x => x.City == city).ToListAsync();
        List<ServiceResponse> services = servicesList.ConvertAll<ServiceResponse>(s => new ServiceResponse { service = s, user = _context.users.Where(x => x.Id == s.UserId).First() });


        return Ok(services);

    }

    [HttpPost]
    [Route("/service/get-service-details")]
    public async Task<ActionResult> getServiceDetails([FromForm] ServiceDetail serviceDetail)
    {

        User user = await _context.users.Where(x => x.Id == serviceDetail.UserId).FirstAsync();
        Service service = await _context.services.Where(x => x.Id == serviceDetail.serviceId).FirstAsync();
        int userBookings =await _context.bookings.Where(x => x.UserId == serviceDetail.UserId && x.ServiceId == serviceDetail.serviceId).CountAsync();
        List<Booking> serviceBookings = await _context.bookings.Where(x => x.ServiceId == serviceDetail.serviceId && x.Status < 2).ToListAsync();
        List<Photo> imagesList = await _context.photos.Where(x=>x.Modle=="Service"&&x.ModleId==serviceDetail.serviceId.ToString()).ToListAsync();
        List<string> images = imagesList.ConvertAll<string>(p => p.Url);

        bool isBooked = false;
        int allowedServiceBooks = service.MaxBookings;
        foreach (Booking b in serviceBookings)
        {
            allowedServiceBooks -= b.Sets;
        }
        if (userBookings > 0) {
            isBooked = true;
        }

        var response = new
        {
            isBooked= isBooked,
            allowedBookings=allowedServiceBooks,
            images=images,
            lefttime =3

        };
        return Ok(response);

    }

    [HttpPost]
    [Route("booking/add-booking")]
    public async Task<ActionResult> addBooking([FromForm] Booking booking)
    {
        var result = await _context.bookings.AddAsync(booking);
        Service service =await _context.services.Where(x => x.Id == booking.ServiceId).FirstAsync();
        await _context.SaveChangesAsync();
        await SendNotificationAsync(service.UserId,
            service.Name, 
            "هناك حجز جديد علي طلعتك  "+service.Name,
            "booking",
            service.Id.ToString(),
            service.Image);
        return Ok(booking);
    }
    
    [HttpPost]
    [Route("booking/update-status")]
    public async Task<ActionResult> updateBookingStatus([FromForm] UpdatebookingStatus updatebookingStatus)
    {
        string response = "";
        Booking booking = await _context.bookings.Where(x => x.Id == updatebookingStatus.id).FirstAsync();
        booking.Status = updatebookingStatus.status;
        await _context.SaveChangesAsync();

        if (updatebookingStatus.status == 1)
        {
            response = "تم تأكيد حجزك";
            Service service =await _context.services.Where(x => x.Id == booking.ServiceId).FirstAsync();
            await SendNotificationAsync(booking.UserId,
                service.Name, 
                "تم تأكيد حجزك علي  "+service.Name,
                "booking",
                service.Id.ToString(),
                service.Image);
        }
        else {
            response = "تم إلغاء الحجز"; 
            Service service =await _context.services.Where(x => x.Id == booking.ServiceId).FirstAsync();
            User user = await _context.users.Where(x => x.Id == updatebookingStatus.userId).FirstAsync();
            await SendNotificationAsync(user.Role=="user"? booking.UserId:service.UserId,
                service.Name, 
                "تم إلغاء الحجز رقم  "+booking.Id,
                "booking",
                service.Id.ToString(),
                service.Image);
        }
        return Ok(response);
    }


    [HttpPost]
    [Route("booking/get-service-bookings")]
    public async Task<ActionResult> updateBookingByService([FromForm] int id)
    {
        List<Booking> bookings = await _context.bookings.Where(x => x.ServiceId == id).ToListAsync();
        foreach (Booking b in bookings)
        {
            b.user = await _context.users.Where(x => x.Id == b.UserId).FirstAsync();
        }
        foreach (Booking b in bookings)
        {
            b.service = await _context.services.Where(x => x.Id == b.ServiceId).FirstAsync();
        }
        return Ok(bookings);

    }

    [HttpPost]
    [Route("booking/get-user-bookings")]
    public async Task<ActionResult> getUserBookings([FromForm] string id)
    {
        List<Booking> bookings = await _context.bookings.Where(x => x.UserId == id).ToListAsync();
        foreach (Booking b in bookings) {
            b.service = await _context.services.Where(x => x.Id == b.ServiceId).FirstAsync();
        }
        return Ok(bookings);

    }





    [HttpPost]
    [Route("messages/get-user-chats")]
    public async Task<ActionResult> getUserChats([FromForm] string id)
    {
        List<Message> chatsList = await _context.messages.Where(x => x.SenderId == id||x.RecieverId==id).OrderByDescending(x=>x.Id).ToListAsync();
        List<MessageResponse> chats = chatsList.ConvertAll<MessageResponse>(s => new MessageResponse { service = _context.services.Where(x => x.Id == s.ServiceId).First(), sender = _context.users.Where(x => x.Id == s.SenderId).First(), reciever = _context.users.Where(x => x.Id == s.RecieverId).First() });

        return Ok(chats);

    }

    [HttpPost]
    [Route("messages/update-messages")]
    public async Task<ActionResult> updateMessages([FromForm] string userId)
    {
        List<Message> chatsList = await _context.messages.Where(x => x.Readed == 0&& x.RecieverId == userId).ToListAsync();
        foreach (Message m in chatsList)
        {
            m.Readed = 1;
        }
        await _context.SaveChangesAsync();

        return Ok("updated");

    }

    [HttpPost]
    [Route("messages/get-messages")]
    public async Task<ActionResult> getMessages([FromForm] GetMessages getMessages)
    {
        List<Message> messages = await _context.messages.Where(x =>
       ( x.SenderId == getMessages.user1 && x.ServiceId == getMessages.serviceId&&x.RecieverId==getMessages.user2)
       ||
       (x.SenderId == getMessages.user2 && x.ServiceId == getMessages.serviceId && x.RecieverId == getMessages.user1)

        ).OrderByDescending(x=>x.Id).ToListAsync();
        foreach (Message m in messages)
        {
            m.Readed = 1;
        }
        await _context.SaveChangesAsync();

        return Ok(messages);

    }


    [HttpPost]
    [Route("add-message")]
    public async Task<ActionResult> addMessage([FromForm] Message message)
    {
        var result = await _context.messages.AddAsync(message);
        await _context.SaveChangesAsync();
        User user =await _context.users.Where(x => x.Id == message.SenderId).FirstAsync();
        await SendNotificationAsync(message.RecieverId,
            "رسالة جديدة من "+user.FullName, 
            message.message,
            "chat",
            message.Id.ToString(),
            user.ProfileImage);
        
        return Ok(message);
    }



    [HttpPost]
    [Route("user/get-user-notifications")]
    public async Task<ActionResult> getUserNotification([FromForm] string userId)
    {
        List<Notif> notifs = await _context.notifs.Where(x => x.UserId == userId).ToListAsync();
        return Ok(notifs);
    }

    [HttpPost]
    [Route("user/update-user-notifications")]
    public async Task<ActionResult> updateNotif([FromForm] string userId)
    {
        Notif notif = await _context.notifs.Where(x => x.UserId == userId).FirstAsync();
        notif.IsRead = 1;
        await _context.SaveChangesAsync();

        return Ok(notif);

    }

    [HttpPost]
    [Route("service/category/get-services")]
    public async Task<ActionResult> getServicesByCategory([FromForm] int category)
    {
        List<Service> servicesList = await _context.services.Where(x => x.CategoryId == category).ToListAsync();
        List<ServiceResponse> services = servicesList.ConvertAll<ServiceResponse>(s => new ServiceResponse { service = s, user = _context.users.Where(x => x.Id == s.UserId).First() });

        return Ok(services);
    }

 

    [HttpPost]
    [Route("service/add-service")]
    public async Task<ActionResult> addService([FromForm] AddService addService)
    {
        Service service = _mapper.Map<Service>(addService);
            foreach (var item in addService.images.Select((value, i) => (value, i)))
            {
             
       
            string url = Functions.upload(item.value,_webHostEnvironment);
            if (item.i == 0) {

              service.Image = url;
                await _context.services.AddAsync(service);
                await _context.SaveChangesAsync();
            };
            Photo photo = new Photo
            {
             Url = url,
             Modle="Service",
             ModleId=service.Id.ToString()
            };
          await _context.photos.AddAsync(photo);
        }
        await _context.SaveChangesAsync();
        return Ok(service);
    }

    [HttpPost]
    [Route("user/add-photos")]
    public async Task<ActionResult> addPhoto([FromForm] AddPhoto addPhoto)
    {

        string Url = Functions.upload(addPhoto.image, _webHostEnvironment);
        Photo photo = new Photo
        {

            Url = Url,
            
            Modle="User",
            ModleId = addPhoto.userId
        };

        await _context.photos.AddAsync(photo);
        await _context.SaveChangesAsync();
        return Ok(photo);
    }

    [HttpPost]
    [Route("get-user-stories")]
    public async Task<ActionResult> getOrgStories([FromForm] string user_id)
    {
        List<Story> stories = await _context.stories.Where(x=>x.UserId==user_id).OrderByDescending(x => x.Id).ToListAsync();


        return Ok(stories);
    }

    [HttpPost]
    [Route("delete-story")]
    public async Task<ActionResult> deleteStory([FromForm] int id)
    {
        var result = await _context.stories.Where(x => x.Id == id).FirstAsync();
        _context.stories.Remove(result);
        await _context.SaveChangesAsync();
        return Ok(result);

    }

    [HttpPost]
    [Route("story/add-story")]
    public async Task<ActionResult> addStory([FromForm] AddStory addStory)
    {
        string image = Functions.upload(addStory.image, _webHostEnvironment);
        string VideoUrl = Functions.uploadVideo(addStory.file, _webHostEnvironment);
        Story story = new Story
        {
            ImageUrl=image,
            VideoUrl= VideoUrl,
            UserId=addStory.userId,
            
        };
        await _context.stories.AddAsync(story);
        await _context.SaveChangesAsync();
        return Ok(story);

    }

    [HttpPost]
    [Route("address/countries")]
    public async Task<ActionResult> getCountries([FromForm] AddStory addStory)
    {
        var country = new {
            code="SA",
            name="السعودية"
        };
        return Ok(country);

    }


    [HttpPost]
    [Route("address/cites/SA")]
    public async Task<ActionResult> getCity([FromForm] AddStory addStory)
    {
        var city1 = new
        {
            name = "الجبيل",
            country = "السعودية"
        };
        var city2 = new
        {
            name = "الجبيل",
            country = "السعودية"
        };
        var response = new {
            city1,city2
        };
        return Ok(response);

    }

    [HttpPost]
    [Route("user/counts")]
    public async Task<ActionResult> getCounts([FromForm] string userId)
    {
        int messages = await _context.messages.Where(x => x.RecieverId == userId && x.Readed == 0).CountAsync();
        int notifs = await _context.notifs.Where(x => x.UserId == userId && x.IsRead == 0).CountAsync();
        string response = messages + "," + notifs;
        return Ok(response);
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<bool> SendNotificationAsync(string userId, string title, string body,string modle,string modleId,string image)
    {
        User user = await _context.users.Where(x=>x.Id==userId).FirstAsync();
        string token = user.DeviceToken;
        using (var client = new HttpClient())
        {
            var firebaseOptionsServerId = "AAAAFm6bjeU:APA91bGg7rANBI-OEO1_g0fJZazoX-t1AoNtxgw45NGPmax036DBJyCOjdI1TvNf-mgYAONtDZ5YzhqUpf_lzwlLRvDqeGx7PPlT0DRtxDn29rJvKbDz_yRSqnzckBOtwXHcAYwXujAm";

            client.BaseAddress = new Uri("https://fcm.googleapis.com");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                $"key={firebaseOptionsServerId}");
            var data = new
            {
                to = token,
                notification = new
                {
                    body = body,
                    title = title,
                },
                priority = "high",

            };

            var json = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("/fcm/send", httpContent);
            Notif notif = new Notif {
                Title=title,
                Body=body,
                Modle=modle,
                ModleId=modleId,
            };
            _context.notifs.Add(notif);
             _context.SaveChangesAsync();
            return result.StatusCode.Equals(HttpStatusCode.OK);
        }
    }


    public async Task<bool> SendNotificationAsyncToMultipleUsers(List<string> tokens, string title, string body)
    {
        using (var client = new HttpClient())
        {
            var firebaseOptionsServerId = "AAAAnh3Nbes:APA91bE3GtVFUXdeK2In1SjuKytaK9kJphq_BMvn_sBTflt4ZqTGYWSzu16tE-acZ7Ul0Vfx_6OEumubBBn0UHHh5jx9Noxxm1HhKB6CIdboyD8s4DyZWzTkh9Frw2JbURqhGxjwVCDm";
            var firebaseOptionsSenderId = "679104835051";

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
                priority = "high"
            };

            var json = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await client.PostAsync("/fcm/send", httpContent);
            return result.StatusCode.Equals(HttpStatusCode.OK);
        }
    }


}
}
