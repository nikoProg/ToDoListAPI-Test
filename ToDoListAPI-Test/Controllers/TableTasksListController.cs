using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAPI.Entities;

namespace ToDoListAPI.Controllers
{
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class TableTasksListController : ControllerBase
    {
        private readonly TasklistContext _context;

        public TableTasksListController(TasklistContext context)
        {
            _context = context;
        }


        // method for getting every single item from the tasklist database
        // GET: api/TableTasksList
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableTasksList>>> GetTableTasksLists()
        {
            return await _context.TableTasksLists.ToListAsync();
        }

        // method for getting a custom {amount} of items from a custom {start} point in the db
        // GET: api/TableTasksList/from=0&take=10
        [HttpGet("from={start}&take={amount}")]
        public async Task<ActionResult<IEnumerable<TableTasksList>>> GetTableTasksListTen(int start, int amount)
        {
            return await _context.TableTasksLists.Skip(start).Take(amount).ToListAsync();
        }

        // method for getting 1 specific element by id
        // GET: api/TableTasksList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TableTasksList>> GetTableTasksList(int id)
        {
            var tableTasksList = await _context.TableTasksLists.FindAsync(id);

            if (tableTasksList == null)
            {
                return NotFound();
            }

            return tableTasksList;
        }

        // method for editing every property in an item, except ID, ID must be the same in the response.
        // PUT: api/TableTasksList/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTableTasksList(int id, [FromBody] TableTasksList tableTasksList)
        {
            if (id != tableTasksList.Id)
            {
                return BadRequest();
            }

            _context.Entry(tableTasksList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableTasksListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTableTasksList", new { id = tableTasksList.Id }, tableTasksList);
        }

        // method for creating a new item in the tasklist
        // POST: api/TableTasksList
        [HttpPost]
        public async Task<ActionResult<TableTasksList>> PostTableTasksList(TableTasksList tableTasksList)
        {
            _context.TableTasksLists.Add(tableTasksList);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TableTasksListExists((int)tableTasksList.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTableTasksList", new { id = tableTasksList.Id }, tableTasksList);
        }

        // method for copying(duplicating) any item when given the item's ID. The new item is created at the end of the tasklist DB.
        // POST: api/TableTasksList/dupe5
        [HttpPost("dupe={id}")]
        public async Task<ActionResult<TableTasksList>> CopyTableTasksList(int id)
        {
            var tableTasksList = await _context.TableTasksLists.FindAsync(id);
            if (tableTasksList == null)
            {
                return NotFound();
            }

            /*dynamic taskDTO = new System.Dynamic.ExpandoObject();
            taskDTO.Task = tableTasksList.Task;
            taskDTO.IsCompleted = tableTasksList.IsCompleted;
            taskDTO.EntryDate = tableTasksList.EntryDate*/

            //setting to null is a workaround, I was trying to make a data transfer object, but had problems, this will simply make it null and the DB will make new Id, since
            //2 items cannot have the same Id.
            tableTasksList.Id = null;
            _context.TableTasksLists.Add(tableTasksList);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TableTasksListExists((int)tableTasksList.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetTableTasksList", new { id = tableTasksList.Id }, tableTasksList);
        }

        // this method will only switch a task's property IsCompleted - true becomes false, false becomes true.
        // NULL is possible to happen, the Database allows NULL values. But this method will treat NULL as false!
        // PUT: api/TableTasksList/check-uncheck-5
        [HttpPut("check-uncheck={id}")]
        public async Task<IActionResult> CheckUncheckTableTasksList(int id)
        {
            var tableTasksList = await _context.TableTasksLists.FindAsync(id);
            if (id != tableTasksList.Id)
            {
                return BadRequest();
            }
            //avoiding NULL but still treating NULL as false
            tableTasksList.IsCompleted = (tableTasksList.IsCompleted == true) ? (tableTasksList.IsCompleted = false) : (tableTasksList.IsCompleted = true);
            _context.Entry(tableTasksList).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTableTasksList", new { id = tableTasksList.Id }, tableTasksList);
        }

        // this method deletes any item, when given the item's ID.
        // DELETE: api/TableTasksList/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TableTasksList>> DeleteTableTasksList(int id)
        {
            var tableTasksList = await _context.TableTasksLists.FindAsync(id);
            if (tableTasksList == null)
            {
                return NotFound();
            }

            _context.TableTasksLists.Remove(tableTasksList);
            await _context.SaveChangesAsync();

            return tableTasksList;
        }

        // a helper method, checks if the table has any elements
        private bool TableTasksListExists(int id)
        {
            return _context.TableTasksLists.Any(e => e.Id == id);
        }
    }
}
