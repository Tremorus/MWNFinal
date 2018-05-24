using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MWN.Models;
using System.Net;
using System.IO;

namespace MWN.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly MWNContext _context;

        public NotesController(MWNContext context)
        {
            _context = context;
        }

        // GET: Notes
        public async Task<IActionResult> Index(string OwnerId, string ownerField, string searchString) //search by owner: ownerField , content filter: searchString
        {
            // create LINQ query for furhter owners extraction
            IQueryable<string> ownerQuery = from m in _context.Note
                                            orderby m.Owner
                                            select m.Owner;

            var notes = from m in _context.Note
                        select m; //LINQ query
            notes = notes.Where(x => x.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier));   //GET  ONLY OWN NOTES by OwnerId
            if (!String.IsNullOrEmpty(searchString))
            {
                notes = notes.Where(s => s.Content.Contains(searchString) || s.Title.Contains(searchString));
            }

            if(!String.IsNullOrEmpty(ownerField))
            {
                notes = notes.Where(x => x.Owner == ownerField);
            }

            var noteOwnerVM = new NoteOwnerViewModel();
            //noteOwnerVM.owners = new SelectList(await ownerQuery.Distinct().ToListAsync()); //select unique values from owners
            noteOwnerVM.notes = await notes.ToListAsync();

            return View(noteOwnerVM);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .SingleOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create view

        public IActionResult Create()
        {

            var note = new Note();
            note.OwnerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            note.Owner = this.User.FindFirstValue(ClaimTypes.Name);
            note.Created = note.Changed = DateTime.Now;
            //note.Changed = DateTime.Now;

            _context.Add(note);
            return View(note);
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Owner,OwnerId,Title,Content,Created,Changed")] Note note)
        {
            if (ModelState.IsValid)
            {
                //note.Changed = note.Created= DateTime.Now;
                string noteTitle = note.Title;

                string avatar = GetAvas(noteTitle);
                note.NoteAvatar = avatar;
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        private string GetAvas(string noteTitle)
        {
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create($"https://avatars.dicebear.com/v2/identicon/{noteTitle}.svg");
            Console.WriteLine(request);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.SingleOrDefaultAsync(m => m.Id == id);
            //note.Changed = DateTime.Now;

            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string NoteAvatar, [Bind("Id,Owner,OwnerId,Title,Content,Changed,Created,NoteAvatar")] Note note)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string noteTitle = note.Title;
                    string avatar = GetAvas(noteTitle);
                    note.NoteAvatar = avatar;
                    note.Changed = DateTime.Now;
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
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
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .SingleOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var note = await _context.Note.SingleOrDefaultAsync(m => m.Id == id);
            _context.Note.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(string id)
        {
            return _context.Note.Any(e => e.Id == id);
        }
    }
}
