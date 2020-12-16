using NewApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewApp.Models
{
    
    public class Meeting
    {
        public Meeting()
        {
            AccId = 0;
            MeetId = 0;
           
            place = "";
            participants = "";
            notes = "";
            notif = 0;
        }
        public int AccId { get; set; }
        [Key]
        public int MeetId { get; set; }
        public string title { get; set; }
        public DateTime dateM { get; set; }
        public string place { get; set; }
        public string participants { get; set;}
        public string notes { get; set; }
        public int notif { get; set; }
    }
}