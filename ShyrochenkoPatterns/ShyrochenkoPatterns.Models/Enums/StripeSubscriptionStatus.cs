using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.Enums
{
    public enum StripeSubscriptionStatus
    {
        Incomplete,
        IncompleteExpired,
        Trialing,
        Active,
        PastDue,
        Canceled,
        Unpaid
    }
}
