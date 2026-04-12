using System;

namespace LegacyRenewalApp;

public static class RenewalServiceValidator
{
    public static void Validate(int CustomerId, string PlanCode, int SeatCount, string PaymentMethod)
    {
        if (CustomerId <= 0)
        {
            throw new ArgumentException("Customer id must be positive");
        }
        
        if (string.IsNullOrWhiteSpace(PlanCode))
        {
            throw new ArgumentException("Plan code is required");
        }

        if (SeatCount <= 0)
        {
            throw new ArgumentException("Seat count must be positive");
        }

        if (string.IsNullOrWhiteSpace(PaymentMethod))
        {
            throw new ArgumentException("Payment method is required");
        }
        
        if (!CustomerRepository.GetById(CustomerId).IsActive)
        {
            throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
        }
    }
}