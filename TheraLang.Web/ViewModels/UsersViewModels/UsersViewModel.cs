﻿using System;

namespace TheraLang.Web.ViewModels.UsersViewModels
{
    public class UsersViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortInformation { get; set; }
        public string ImageURl { get; set; }
        public DateTime BirthdayDate { get; set; }
    }
}