﻿namespace ShoppingWebsite.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }   
    }
}
