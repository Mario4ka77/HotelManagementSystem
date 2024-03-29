//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Hotel
    {
        public Hotel()
        {
            this.Rooms = new HashSet<Room>();
        }
    
        public int HotelId { get; set; }
        [Required]
        [Display(Name = "Hotel Name")]
        public string HotelName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int CountryId { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
