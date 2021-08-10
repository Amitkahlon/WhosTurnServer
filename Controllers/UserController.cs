using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhosTurnServer.ApplictionModels.Models;
using WhosTurnServer.Repository.IRepository;

namespace WhosTurnServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepo;

        public UserController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] User model)
        {
            var user = this.userRepo.Authenticate(model.Email, model.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] User model)
        {
            var isUnique = this.userRepo.IsUniqueUser(model.Email);
            if (!isUnique)
            {
                return Conflict(new { message = "Email Is Already Exist" });
            }
            else
            {
                var user = this.userRepo.Register(model.Email, model.Password);
                return Ok(user);
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
