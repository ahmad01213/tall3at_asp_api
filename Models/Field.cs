using System;
namespace Donia.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int status { get; set; }



        public Field()
        {
            status = 1;
        }
    }

}
