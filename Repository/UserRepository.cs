using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WhosTurnServer.ApplictionModels.Models;
using WhosTurnServer.Data;
using WhosTurnServer.Repository.IRepository;
using WhosTurnServer.Settings;

namespace WhosTurnServer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplictionDbContext db;
        private readonly AppSettings appSettings;

        public UserRepository(ApplictionDbContext db, IOptions<AppSettings> appSettings)
        {
            this.db = db;
            this.appSettings = appSettings.Value;
        }

        public User Authenticate(string email, string password)
        {
            var user = this.db.Users.SingleOrDefault(user => user.Email == email && user.Password == password);

            //user not found
            if (user == null)
            {
                return null;
            }

            //if user was found generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials
                                    (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";

            return user;
        }

        public List<User> GetUsers()
        {
            return this.db.Users.ToList();
        }

        public bool IsUniqueUser(string email)
        {
            //return if email already exists.
            return !this.db.Users.Any(user => user.Email == email);
        }

        public User Register(string email, string password)
        {
            User userObj = new User()
            {
                Email = email,
                Password = password
            };
            this.db.Add(userObj);
            this.db.SaveChanges();

            userObj.Password = "";
            return userObj;
        }
    }
}
