using System.ComponentModel.DataAnnotations;
using System;

namespace blackbelt.Models
{
   public class Bidder : BaseEntity
   {
       public int UserId {get; set;}
       public int AuctionId {get; set;}
       public float Bid {get; set;}
       public User User {get; set;}
   }


}