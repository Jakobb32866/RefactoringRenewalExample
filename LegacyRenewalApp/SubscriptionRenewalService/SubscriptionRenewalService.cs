using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            RenewalServiceValidator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = Util.StringNormaliser(planCode);
            string normalizedPaymentMethod = Util.StringNormaliser(paymentMethod);
            
            var customer = CustomerRepository.GetById(customerId);
            var plan = SubscriptionPlanRepository.GetByCode(normalizedPlanCode);

            decimal subtotalAfterDiscount = RenewalServiceAmountCalculator.CalculateRenewalAmount(plan, seatCount, customer, useLoyaltyPoints, includePremiumSupport, planCode, paymentMethod);

            decimal supportFee = RenewalServiceAmountCalculator.GetSupportFee();
            
            decimal paymentFee = RenewalServiceAmountCalculator.GetPaymentFee();
            
            decimal taxRate = RenewalServiceAmountCalculator.GetTaxRate();
            
            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;
            
            string notes = RenewalServiceAmountCalculator.GetNotes();

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }
            
            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(RenewalServiceAmountCalculator.GetBaseAmount(), 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(RenewalServiceAmountCalculator.GetDiscountAmount(), 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(RenewalServiceAmountCalculator.GetSupportFee(), 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            InvoiceSender.sendInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                EmailSender.SendEmail(customer, invoice, normalizedPlanCode);
            }

            return invoice;
        }
    }
}
