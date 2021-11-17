using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversationOverflow.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGroupRepository _groups;

        public GroupController(ILogger<UserController> logger,
            IGroupRepository groups)
        {
            _logger = logger;
            _groups = groups;
        }

        [HttpGet]
        public async Task<List<Group>> Get() => await _groups.GetAllGroupsAsync(User.Identity.Name);
        [HttpPost]
        public async Task Post(Group group) => await _groups.AddGroupAsync(User.Identity.Name, group);
        [HttpPut]
    }
}
