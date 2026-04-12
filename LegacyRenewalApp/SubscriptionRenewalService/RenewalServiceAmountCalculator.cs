using System;

namespace LegacyRenewalApp;

public static class RenewalServiceAmountCalculator
{
    private static string notes;
    private static decimal baseAmount;
    private static decimal discountAmount;
    private static decimal supportFee;
    private static decimal paymentFee;
    private static decimal taxRate;

    public static decimal CalculateRenewalAmount(
        SubscriptionPlan Plan,
        int SeatCount,
        Customer customer,
        bool useLoyaltyPoints,
        bool includePremiumSupport,
        string planCode,
        string paymentMethod
        )
    {
        notes = "";
        
        baseAmount = (Plan.MonthlyPricePerSeat * SeatCount * 12m) + Plan.SetupFee;
        discountAmount = 0m;
        supportFee = 0m;
        taxRate = 0m;
        
        string normalizedPlanCode = Util.StringNormaliser(planCode);
        string normalizedPaymentMethod = Util.StringNormaliser(paymentMethod);

        discountAmountCalc(customer, Plan, SeatCount, useLoyaltyPoints);

        decimal subtotalAfterDiscount = baseAmount - discountAmount;
            
        if (subtotalAfterDiscount < 300m)
        {
            subtotalAfterDiscount = 300m;
            notes += "minimum discounted subtotal applied; ";
        }

        supportFeeCalc(includePremiumSupport, normalizedPlanCode);

        paymentFeeCalc(normalizedPaymentMethod, subtotalAfterDiscount);

        texRateCalc(customer);
        
        return subtotalAfterDiscount;
    }

    private static void discountAmountCalc(Customer customer, SubscriptionPlan Plan, int SeatCount, bool useLoyaltyPoints)
    {
        if (customer.Segment == "Silver")
        {
            discountAmount += baseAmount * 0.05m;
            notes += "silver discount; ";
        }
        else if (customer.Segment == "Gold")
        {
            discountAmount += baseAmount * 0.10m;
            notes += "gold discount; ";
        }
        else if (customer.Segment == "Platinum")
        {
            discountAmount += baseAmount * 0.15m;
            notes += "platinum discount; ";
        }
        else if (customer.Segment == "Education" && Plan.IsEducationEligible)
        {
            discountAmount += baseAmount * 0.20m;
            notes += "education discount; ";
        }
        
        if (customer.YearsWithCompany >= 5)
        {
            discountAmount += baseAmount * 0.07m;
            notes += "long-term loyalty discount; ";
        }
        else if (customer.YearsWithCompany >= 2)
        {
            discountAmount += baseAmount * 0.03m;
            notes += "basic loyalty discount; ";
        }

        if (SeatCount >= 50)
        {
            discountAmount += baseAmount * 0.12m;
            notes += "large team discount; ";
        }
        else if (SeatCount >= 20)
        {
            discountAmount += baseAmount * 0.08m;
            notes += "medium team discount; ";
        }
        else if (SeatCount >= 10)
        {
            discountAmount += baseAmount * 0.04m;
            notes += "small team discount; ";
        }
        
        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            discountAmount += pointsToUse;
            notes += $"loyalty points used: {pointsToUse}; ";
        }
    }

    private static void supportFeeCalc(bool includePremiumSupport, string normalizedPlanCode)
    {
        if (includePremiumSupport)
        {
            if (normalizedPlanCode == "START")
            {
                supportFee = 250m;
            }
            else if (normalizedPlanCode == "PRO")
            {
                supportFee = 400m;
            }
            else if (normalizedPlanCode == "ENTERPRISE")
            {
                supportFee = 700m;
            }

            notes += "premium support included; ";
        }
    }

    private static void paymentFeeCalc(string normalizedPaymentMethod, decimal subtotalAfterDiscount)
    {
        if (normalizedPaymentMethod == "CARD")
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * 0.02m;
            notes += "card payment fee; ";
        }
        else if (normalizedPaymentMethod == "BANK_TRANSFER")
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * 0.01m;
            notes += "bank transfer fee; ";
        }
        else if (normalizedPaymentMethod == "PAYPAL")
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * 0.035m;
            notes += "paypal fee; ";
        }
        else if (normalizedPaymentMethod == "INVOICE")
        {
            paymentFee = 0m;
            notes += "invoice payment; ";
        }
        else
        {
            throw new ArgumentException("Unsupported payment method");
        }
    }

    private static void texRateCalc(Customer customer)
    {
        if (customer.Country == "Poland")
        {
            taxRate = 0.23m;
        }
        else if (customer.Country == "Germany")
        {
            taxRate = 0.19m;
        }
        else if (customer.Country == "Czech Republic")
        {
            taxRate = 0.21m;
        }
        else if (customer.Country == "Norway")
        {
            taxRate = 0.25m;
        }
    }
    
    public static string GetNotes()
    {
        return notes;
    }
    
    public static decimal GetBaseAmount()
    {
        return baseAmount;
    }
    
    public static decimal GetDiscountAmount()
    {
        return discountAmount;
    }

    public static decimal GetSupportFee()
    {
        return supportFee;
    }
    
    public static decimal GetPaymentFee()
    {
        return paymentFee;
    }
    
    public static decimal GetTaxRate()
    {
        return taxRate;
    }
}