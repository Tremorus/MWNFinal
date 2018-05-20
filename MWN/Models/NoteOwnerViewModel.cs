using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

//model for notes owners/authors
namespace MWN.Models
{
    public class NoteOwnerViewModel
    {
        public List<Note> notes;
        public SelectList owners;
        public string ownerField { get; set; }
    }
}
