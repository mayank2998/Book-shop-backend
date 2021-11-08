using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.AspNetCore;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get(int pid, string category,string name,string author)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppConn"));
            if (pid == 0 && category == null && name==null)
            {
                var dbList = dbClient.GetDatabase("product").GetCollection<Models.Ecommerce>("ecommerce").AsQueryable();
                return new JsonResult(dbList);
            }
            else if(pid != 0 && category == null && name==null)
            {

                var filter = Builders<Models.Ecommerce>.Filter.Eq("pid", pid);

                var dbList = dbClient.GetDatabase("product").GetCollection<Models.Ecommerce>("ecommerce").Find(filter).FirstOrDefault();

                return new JsonResult(dbList);
            }
            else if(category == null && (name != null || !string.IsNullOrEmpty(author)))
            {

                var filter = Builders<Models.Ecommerce>.Filter.Or(
                    Builders<Models.Ecommerce>.Filter.Where(p => p.name.ToLower().Contains(name.ToLower())),
                    Builders<Models.Ecommerce>.Filter.Where(p => p.author.ToLower().Contains(author.ToLower()))
                            );

                var dbList = dbClient.GetDatabase("product").GetCollection<Models.Ecommerce>("ecommerce").Find(filter).ToList();
                if(dbList.Count == 0)
                {
                    return new JsonResult("failure");
                }
                return new JsonResult(dbList);
            }
            else
            {
                var filter = Builders<Models.Ecommerce>.Filter.Eq("category", category);

                var dbList = dbClient.GetDatabase("product").GetCollection<Models.Ecommerce>("ecommerce").Find(filter).ToList();

                return new JsonResult(dbList);
            }

            
        } 

    }
}
