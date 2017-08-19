using System.ComponentModel.DataAnnotations;
using System;

namespace blackbelt.Models
{
    public class Auction : BaseEntity
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required(ErrorMessage="Enter A Starting Bid")]
        public float Bid { get; set; }

        [Required(ErrorMessage="Enter Enough Information For Us to Display")]
        [MinLength(15)]
        public string Description { get; set; }

        [Required(ErrorMessage="Enter an Ending Date for this auction")]
        public DateTime EndingDate { get; set; }
        public int UserId { get; set; }
        public User User {get; set;}
        public TimeSpan TimeLeft {get; set;}
        public int Id {get; set;}
    }
}