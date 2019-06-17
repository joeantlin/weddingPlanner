using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class RSVP
    {
        [Key]
        public int Id {get;set;}
        public int UserId {get;set;}
        public int WeddingId {get;set;}
        public User User {get; set;}
        public Wedding Wedding {get;set;}
        public bool Creator {get;set;} = false;
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}