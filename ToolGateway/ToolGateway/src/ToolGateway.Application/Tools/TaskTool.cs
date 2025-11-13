namespace ToolGateway.Application.Tools
{
    using AutoMapper;
    using Juga.Data.Paging;
    using MediatR;
    using ModelContextProtocol.Server;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using ToolGateway.Application.Dtos;
    using ToolGateway.Application.Features.TodoHandlers;

    namespace MyMcpServer.Tools
    {
        [McpServerToolType]
        public class TaskTool
        {

            public TaskTool(ISender sender, IMapper mapper)
            {
                this.sender = sender;
                this.mapper = mapper;
            }

            private readonly ISender sender;
            private readonly IMapper mapper;

            [McpServerTool, Description("Add a new task with title and description; returns its GUID.")]
            public async Task<Guid> AddTask(string title, string description, CancellationToken cancellationToken = default)
            {
                var command = new CreateTodoCommand(title, description, false);
                var commandResult = await sender.Send(command, cancellationToken);

                return commandResult.Id;
            }

            [McpServerTool, Description("List all tasks with their IDs, titles, descriptions, and completion status.")]
            public async Task<IList<TodoDto>> ListTasksAsync(CancellationToken cancellationToken = default)
            {
                var command = new TodosQuery(new PageRequest() { PageIndex = 0, PageSize = 1000 });
                var commandResult = await sender.Send(command, cancellationToken);
                return commandResult.result.Items;
            }

            [McpServerTool, Description("Mark a task complete by its GUID.")]
            public async Task<string> CompleteTask(Guid id, CancellationToken cancellationToken = default)
            {
                var command = new CompleteTodoCommand(id);
                var commandResult = await sender.Send(command, cancellationToken);
                if (commandResult == null)
                {
                    return $"Complete task operation is failed: (ID: {id})";
                }
                else
                {
                    return $"Completed task: \"{commandResult.Title}\" (ID: {id})";
                }
            }

            [McpServerTool, Description("Remove a task by its GUID.")]
            public async Task<string> RemoveTask(Guid id, CancellationToken cancellationToken = default)
            {
                var todoDto = await GetTodoById(id,cancellationToken);
                if (todoDto == null)
                {
                    return $"Task with ID {id} not found.";
                }
                var command = new DeleteTodoCommand(id);
                var commandResult = await sender.Send(command, cancellationToken);
                if (commandResult == null || !commandResult.result)
                {
                    return $"Removed task operation is failed: \"{todoDto.Title}\" (ID: {id})";
                }
                else
                {
                    return $"Removed task: \"{todoDto.Title}\" (ID: {id})";
                }
            }

            [McpServerTool, Description("Update a task's title and description by its GUID.")]
            public async Task<string> UpdateTaskAsync(Guid id, string newTitle, string newDescription, CancellationToken cancellationToken = default)
            {
                var todoDto = await GetTodoById(id,cancellationToken);
                if (todoDto == null)
                {
                    return $"Task with ID {id} not found.";
                }

                var command = new UpdateTodoCommand(id,newTitle,newDescription,todoDto.IsCompleted);
                var commandResult = await sender.Send(command, cancellationToken);
                if (commandResult == null)
                {
                    return $"Update task operation is failed: \"{todoDto.Title}\" (ID: {id})";
                }
                else
                {
                    return $"Removed task: \"{newTitle}\" (ID: {id})";
                }
            }

            private async Task<TodoDto?> GetTodoById(Guid id, CancellationToken cancellationToken = default)
            {
                var query = new TodoQuery(id);
                var queryResult = await sender.Send(query, cancellationToken);
                if (queryResult != null)
                {
                    return queryResult.result;
                }
                else
                {
                    return null;
                }
            }

            private async Task<TodoDto?> UpdateTodo(TodoDto todoDto, CancellationToken cancellationToken = default)
            {
                var command = mapper.Map<UpdateTodoCommand>(todoDto);
                var commandResult = await sender.Send(command, cancellationToken);
                if (commandResult != null)
                {
                    var result = mapper.Map<TodoDto>(commandResult);
                    return result;
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
