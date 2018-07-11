﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.API.Application.Commands;
using Project.API.Application.Service;
using Project.API.Dto;
using Project.Domain.AggregatesModel;

namespace Project.API.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private IMediator _mediatR;
        private IRecommendService _recommendService;
        public ProjectController(IMediator mediator, IRecommendService recommendService)
        {
            _mediatR = mediator;
            _recommendService = recommendService;
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateProject([FromBody]Domain.AggregatesModel.Project project)
        {
            CreateProjectCommand command = new CreateProjectCommand { Project = project};
            var result =_mediatR.Send(command, new CancellationToken());
            await result;
            return Ok(result);
        }

        [HttpPut]
        [Route("join/{projectId}")]
        public async Task<IActionResult> JoinProject([FromBody]ProjectContributor contributor)
        {
            if (!(await _recommendService.IsProjectRecommend(contributor.ProjectId, UserIdentity.UserId)))
            {
                return BadRequest("没有查看该项目的权限");
            }

            JoinProjectCommand command = new JoinProjectCommand {Contributor= contributor };
            var result = await  _mediatR.Send(command, new CancellationToken());
            return Ok(result);
        }

        [HttpPut]
        [Route("view/{projectId}")]
        public async Task<IActionResult> ViewProject(int projectId)
        {
            if (!(await _recommendService.IsProjectRecommend(projectId, UserIdentity.UserId)))
            {
                return BadRequest("没有查看该项目的权限");
            }

            UserIdentity identity = UserIdentity;
            ViewProjectCommand command = new ViewProjectCommand
            {
                ProjectId = projectId
                ,
                UserId = identity.UserId
                ,
                UserName = identity.Name
                ,
                Avatar = identity.Avatar
            };
            var result = await _mediatR.Send(command, new CancellationToken());
            return Ok(result);
        }
    }
}
