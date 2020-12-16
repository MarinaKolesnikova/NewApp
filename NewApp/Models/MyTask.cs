using NewApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewApp.Models
{
    
    public class MyTask
    {
        public MyTask()
        {
            AccId = 0;
            TaskId = 0;
           
            descriptionT = "";
        }
        //public MyTask(TaskD row)
        //{
        //    AccId = row.AccId;
        //    TaskId = row.TaskId;
        //    dateT = row.dateT;
        //    timeT = row.timeT;
        //    descriptionT = descriptionT;
        //}
        public int AccId { get; set; }

        public int TaskId { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public DateTime startDateT { get; set; }
        [Required]
        public DateTime endDateT { get; set; }
        [Required]
        public string descriptionT { get; set; }
        public List<Contacts> contacts;
    }
}