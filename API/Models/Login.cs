using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace API.Models
{
    public class Login
    {
        public ObjectId _id { get; set; }

       
        public string name { get; set; }
       public string email { get; set; }
        public string password { get; set; }
        public BsonDateTime date { get; set; }
        public Boolean isAdmin { get; set; }
    }
}
