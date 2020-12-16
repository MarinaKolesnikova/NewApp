
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewApp.Models
{
    public class Contacts
    {
        public Contacts()
        {
            idAccC = 0;
            ContId = 0;
            contName = "";
            contSurname = "" ;
            phNumber = "";
            
        }
    
        public int idAccC { get; set; }
        public int ContId { get; set; }
        
        public string contName { get; set; }
        public string contSurname { get; set; }
        
        public string phNumber { get; set; }
       
    }
}