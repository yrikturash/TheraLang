﻿using System;
using System.Collections.Generic;

namespace TheraLang.DAL.Entities
{
    public class UserDetails
    {
        public UserDetails()
        {
            Resources = new HashSet<Resource>();
        }

        public Guid UserDetailsId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShortInformation { get; set; }
        public string ImageURl { get; set; }
        public string City { get; set; }
        public DateTime? BirthDay { get; set; }
        public User User { get; set; }
        public decimal Balance { get; set; }


        public virtual ICollection<Resource> Resources { get; set; }
    }
}