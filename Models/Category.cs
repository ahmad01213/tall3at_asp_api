using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Donia.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int field_id { get; set; }
        public string name { get; set; }
        public int status { get; set; }
        public Category()
        {
            status = 1;
    }
    }



}
