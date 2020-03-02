﻿using Microsoft.AspNetCore.Http;

namespace TheraLang.BLL.DataTransferObjects
{
    public class UserAllDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile Image { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ShortInformation { get; set; }
        public string ImageURl { get; set; }
    }
}