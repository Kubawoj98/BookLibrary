﻿using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
