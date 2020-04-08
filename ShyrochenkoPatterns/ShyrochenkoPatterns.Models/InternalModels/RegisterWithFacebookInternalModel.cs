using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.InternalModels
{
    public class RegisterWithFacebookUsingPhoneInternalModel
    {
        public string PhoneNumber { get; set; }

        public string FacebookId { get; set; }
    }

    public class RegisterWithFacebookUsingEmailInternalModel
    {
        public string Email { get; set; }

        public string FacebookId { get; set; }
    }
}
