using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spalce.Catalog.Service.Entities;
using Spalce.Common.Classes;
using Spalce.Common.Repositories;

namespace Spalce.Catalog.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _repository;

    public ItemsController(IRepository<Item> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<Response<IReadOnlyCollection<Item>>>> GetAll()
    {
        return await _repository.GetAllAsync();
    }

    [HttpGet("get-all-filtered")]
    public async Task<ActionResult<Response<IReadOnlyCollection<Item>>>> GetAll([FromQuery] string filter)
    {
        Expression<Func<Item, bool>> filterExpression = null;

        if (!string.IsNullOrEmpty(filter))
        {
            // Parse and create the filter expression based on the query string parameter
            // Here, you need to implement your own logic to parse and create the filter expression
            // filterExpression = ParseFilterExpression(filter);
        }

        var result = await _repository.GetAllAsync(filterExpression);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Response<Item>>> GetById([FromRoute] Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult<Response<Item>>> Create([FromBody] Item item)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(",", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            var list = message.Split(",").ToList();
            return BadRequest(new Response<Item>
            {
                Message = "Validation failed",
                Data = null,
                Errors = list,
                IsSuccess = false
            });
        }

        if (item.Id == Guid.Empty)
        {
            item.Id = Guid.NewGuid();
        }

        return await _repository.CreateAsync(item);
    }

    [HttpPost("create-many")]
    public async Task<ActionResult<Response<Item>>> CreateMany([FromBody] IReadOnlyCollection<Item> items)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(",", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            var list = message.Split(",").ToList();
            return BadRequest(new Response<Item>
            {
                Message = "Validation failed",
                Data = null,
                Errors = list,
                IsSuccess = false
            });
        }

        return await _repository.CreateManyAsync(items);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Response<Item>>> Update([FromRoute] Guid id, [FromBody] Item item)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(",", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            var list = message.Split(",").ToList();
            return BadRequest(new Response<Item>
            {
                Message = "Validation failed",
                Errors = list,
                IsSuccess = false
            });
        }

        var existingItem = await _repository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return NotFound(new Response<Item>
            {
                Message = "Validation failed",
                Errors = new List<string> { "Item not found" },
                IsSuccess = false
            });
        }

        return await _repository.UpdateAsync(item);
    }

    [HttpPut("update-many")]
    public async Task<ActionResult<Response<Item>>> UpdateMany([FromBody] IReadOnlyCollection<Item> items)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(",", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            var list = message.Split(",").ToList();
            return BadRequest(new Response<Item>
            {
                Message = "Validation failed",
                Errors = list,
                IsSuccess = false
            });
        }

        return await _repository.UpdateManyAsync(items);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Response<Item>>> Delete([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new Response<Item>
            {
                Message = "Validation failed",
                Errors = new List<string> { "Item not found" },
                IsSuccess = false
            });
        }

        var existingItem = await _repository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return NotFound(new Response<Item>
            {
                Message = "Validation failed",
                Errors = new List<string> { "Item not found" },
                IsSuccess = false
            });
        }

        return await _repository.DeleteAsync(id);
    }

    [HttpDelete("delete-many")]
    public async Task<ActionResult<Response<Item>>> DeleteMany([FromBody] IReadOnlyCollection<Guid> ids)
    {
        if (ids == null)
        {
            return BadRequest(new Response<Item>
            {
                Message = "Validation failed",
                Errors = new List<string> { "Item not found" },
                IsSuccess = false
            });
        }

        return await _repository.DeleteManyAsync(ids);
    }
}
