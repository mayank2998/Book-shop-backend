using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace API.Models
{
    public class Ecommerce
    {
       
        public ObjectId _id { get; set; }

        public int pid { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string img { get; set; }
        public string category { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public Boolean inCart { get; set; } 
    }
}
