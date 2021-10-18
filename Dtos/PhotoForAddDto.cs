using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Dtos
{
    public class PhotoForAddDto
    {

        public int Id { get; set; }
        public IFormFile file { get; set; }
        public string ModleId { get; set; }
        public string Modle { get; set; }
    }
}
