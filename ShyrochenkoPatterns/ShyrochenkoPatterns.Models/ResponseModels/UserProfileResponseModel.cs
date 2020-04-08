using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class UserProfileResponseModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsBlocked { get; set; }

        public int UserId { get; set; }

        public ImageResponseModel Avatar { get; set; }
    }
}
