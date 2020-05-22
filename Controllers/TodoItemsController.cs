using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
  [Route("api/TodoItems")]
  [ApiController]
  public class TodoItemsController : ControllerBase
  {
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
      _context = context;
    }

    // Get All Todo’s
    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
      //return all
      return await _context.TodoItems.ToListAsync();
    }


    // Get Specific Todo
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
    {
      //Find by id
      var todoItem = await _context.TodoItems.FindAsync(id);

      if (todoItem == null)
      {
        return NotFound();
      }

      //return
      return todoItem;

    }


    // Get Incoming ToDo (for today/next day/current week)
    [HttpGet("incoming/{type?}")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItembyIncoming(string type)
    {
      //current date
      DateTime now = DateTime.Now.Date;
      DateTime? date = null;
      List<TodoItem> todoItem = null;

      //default is today, if type is thisweek or tommorow get plus day 
      if (type == "thisweek")
      {
        date = now.AddDays(7);
      }
      else if (type == "tommorow")
      {
        date = now.AddDays(1);
      }


      if (date == null)
      {
        // return only today date 
        todoItem = await _context.TodoItems.Where(b => b.ExpiryDate.Date == now).ToListAsync();
      }
      else
      {
        // return between date
        todoItem = await _context.TodoItems.Where(b => b.ExpiryDate.Date >= now && b.ExpiryDate.Date <= date).ToListAsync();
      }

      if (todoItem == null)
      {
        return NotFound();
      }

      return todoItem;
    }




    // Update Todo
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(long id, TodoItem todoItem)
    {
      if (id != todoItem.Id)
      {
        todoItem.Id = id;
      }

      _context.Entry(todoItem).State = EntityState.Modified;
      try
      {
        //save
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
      {
        return NotFound();
      }

      return NoContent();
    }

    // Set Todo percent complete
    [HttpPut("progress/{id}/{progress}")]
    public async Task<IActionResult> UpdateProgress(long id, int progress)
    {
      var todoItem = await _context.TodoItems.FindAsync(id);

      if (todoItem == null)
      {
        return NotFound();
      }

      // give validation if over 100 or less than 0
      if (progress > 100)
      {
        progress = 100;
      }
      else if (progress < 0)
      {
        progress = 0;
      }

      //set progress
      todoItem.Progress = progress;

      try
      {
        //save
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
      {
        return NotFound();
      }

      return NoContent();
    }

    // Mark Todo as Done
    [HttpPut("marktodo/{id}")]
    public async Task<IActionResult> MarkTodoItem(long id)
    {
      var todoItem = await _context.TodoItems.FindAsync(id);

      if (todoItem == null)
      {
        return NotFound();
      }

      //set progress 100
      todoItem.Progress = 100;
      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
      {
        return NotFound();
      }

      return NoContent();
    }

    // Create Todo
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
    {
      _context.TodoItems.Add(todoItem);
      //save
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }

    // Delete Todo
    [HttpDelete("{id}")]
    public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
    {
      var todoItem = await _context.TodoItems.FindAsync(id);
      if (todoItem == null)
      {
        return NotFound();
      }
      //delete
      _context.TodoItems.Remove(todoItem);

      //save
      await _context.SaveChangesAsync();

      return todoItem;
    }

    private bool TodoItemExists(long id)
    {
      return _context.TodoItems.Any(e => e.Id == id);
    }
  }
}