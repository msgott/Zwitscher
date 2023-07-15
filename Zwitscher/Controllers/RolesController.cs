using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Attributes;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    [Admin] //Only Admins can use any of the declared endpoints
    public class RolesController : Controller //Controller Class for dealing with Roles Objects
    {
        private readonly ZwitscherContext _context;

        public RolesController(ZwitscherContext context)
        {
            _context = context;//Dependency Injection of DBContext
        }
        #region Base MVC Stuff for Index, Create, Edit, Delete
        //============================================= Base MVC Stuff for Index, Create ,Edit, Delete =====================================================
        // GET: Roles
        [HttpGet]
        [Route("Roles")]
        public async Task<IActionResult> Index()
        //HTTP Get Index endpoint
        //Routed to /Roles
        //Serves the View for the Role Index page
        {
            return _context.Role != null ? 
                          View(await _context.Role.ToListAsync()) :
                          Problem("Entity set 'ZwitscherContext.Role'  is null.");
        }

        // GET: Roles/Details/5
        [HttpGet]
        [Route("Roles/Details")]
        public async Task<IActionResult> Details(Guid? id)
        //HTTP Get Details endpoint
        //Routed to /Roles/Details
        //takes an id
        //Serves the View for the Role Details page
        {
            if (id == null || _context.Role == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        [HttpGet]
        [Route("Roles/Create")]
        public IActionResult Create()
        //HTTP Get Create endpoint
        //Routed to /Roles/Create
        //Serves the View for the Role Create page
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [Route("Roles/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Role role)
        //HTTP Post Create endpoint
        //Routed to /Roles/Create
        //takes a binded role Object
        //Creates a new Role based on given Properties
        {
            if (ModelState.IsValid)
            {
                role.Id = Guid.NewGuid();
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        [HttpGet]
        [Route("Roles/Edit")]
        public async Task<IActionResult> Edit(Guid? id)
        //HTTP Get Edit endpoint
        //Routed to /Roles/Edit
        //takes an id
        //serves the role edit view

        {
            if (id == null || _context.Role == null)
            {
                return NotFound();
            }

            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Roles/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] Role role)
        //HTTP Post Edit endpoint
        //Routed to /Roles/Edit
        //takes a binded and edited role Object
        //Edits the Role with the is based on given Properties
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Delete/5
        [HttpGet]
        [Route("Roles/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        //HTTP Get Delete endpoint
        //Routed to /Roles/Delete
        //takes an id
        //serves the role Delete view
        {
            if (id == null || _context.Role == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [Route("Roles/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        //HTTP Post Delete endpoint
        //Routed to /Roles/Delete
        //takes an id
        //Deletes a Role based on given id 
        {
            if (_context.Role == null)
            {
                return Problem("Entity set 'ZwitscherContext.Role'  is null.");
            }
            var role = await _context.Role.FindAsync(id);
            if (role != null)
            {
                _context.Role.Remove(role);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(Guid id)
        {
          return (_context.Role?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        #endregion
    }
}
