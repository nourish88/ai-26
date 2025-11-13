using AutoMapper;
using Juga.Data.Paging;
using ToolGateway.Application.Dtos;
using ToolGateway.Application.Features.TodoHandlers;
using ToolGateway.Domain.Entities;

namespace AdminBackend.Application.Profiles
{
    public class TodoProfiles : Profile
    {       
        public TodoProfiles()
        {      
            #region [Application]

            CreateMap<CreateTodoCommand, Todo>();
            CreateMap<Todo, CreateTodoCommandResult>();
            CreateMap<Todo, TodoDto>().ReverseMap();
            CreateMap<UpdateTodoCommand, Todo>().ReverseMap();
            CreateMap<Todo, UpdateTodoCommandResult>();
            CreateMap<Paginate<Todo>, PageResponse<TodoDto>>().ReverseMap();
            CreateMap<Todo, CompleteTodoCommandResult>();

            #endregion[END_Application]
        }
    }
}
