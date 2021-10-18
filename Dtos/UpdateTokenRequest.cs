using System;
namespace Donia.Dtos
{
    public class UpdateTokenRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string isDriver { get; set; }
    }
}
