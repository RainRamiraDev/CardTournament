﻿

using CTService.Interfaces.User;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Controllers.User
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {


        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

    }
}
