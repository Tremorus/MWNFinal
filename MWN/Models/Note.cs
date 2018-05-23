using System;
using System.ComponentModel.DataAnnotations;

namespace MWN.Models
{
    public class Note
    {

        public string Id { get; set; }
        public string OwnerId { get; set; }
        [Display(Name="Author")]
        public string Owner { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MM-yyyy hh:mm}")]
        [Display(Name = "Creation date")]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MM-yyyy hh:mm}")]
        public DateTime Changed { get; set; }
        public string NoteAvatar { get; set; }
        //public Note()
        //{
        //    Created= Changed = DateTime.Now;
            
        //}
    }
}
