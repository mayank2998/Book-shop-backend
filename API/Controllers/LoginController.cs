using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ValidateLogin([FromBody] Models.Login userModel)
        {

           

            IActionResult response = Unauthorized();

            if (!string.IsNullOrEmpty(userModel.password)
                && !string.IsNullOrEmpty(userModel.email) && !string.IsNullOrEmpty(userModel.name))
            {
                MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppConn"));

                var dblist = dbClient.GetDatabase("product").GetCollection<Models.Login>("login").AsQueryable();

                 foreach (var oParam in dblist)
                {
                    if(oParam.email == userModel.email)
                    {
                        response = Ok(new { result = "failure" });

                        return response;
                    }
                }

                dbClient.GetDatabase("product").GetCollection<Models.Login>("login").InsertOne(userModel);

                response = Ok(new {result = "success" });

                return response;
            }

            Models.Login loginData = ValidateUserInDB(userModel);

            if (loginData!=null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var payload = new[] {
                    new Claim(JwtRegisteredClaimNames.Email, userModel.email) ,
                    new Claim(JwtRegisteredClaimNames.UniqueName, userModel.password),
                   /* {"iss","https://localhost:44306" }*/
                };

                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"]
                    , _configuration["Jwt:Issuer"]
                    , payload
                    , expires: DateTime.Now.AddMinutes(10)
                    , signingCredentials: signCredentials);
                var registeredToken = new JwtSecurityTokenHandler().WriteToken(token);
                response = Ok(new { token = registeredToken , result = "success",userName = loginData.name,admin = loginData.isAdmin});
            }

            return response;
        }


        private Models.Login ValidateUserInDB(Models.Login userModel)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppConn"));

            var dblist = dbClient.GetDatabase("product").GetCollection<Models.Login>("login").AsQueryable();

            foreach (var oParam in dblist)
            {
                if(oParam.email == userModel.email && oParam.password == userModel.password)
                {
                    Console.WriteLine("hi from db");
                    return oParam;
                }
            }
            return null;
        }

        /*  public JsonResult Post(Models.Login log)
          {
              MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppConn"));

              dbClient.GetDatabase("product").GetCollection<Models.Login>("login").InsertOne(log);

              return new JsonResult("Register success");




          }*/
        [AllowAnonymous]
        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppConn"));

            var dblist = dbClient.GetDatabase("product").GetCollection<Models.Login>("login").AsQueryable();

            return new JsonResult(dblist);

        }
    }
}
