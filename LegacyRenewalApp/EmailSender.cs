namespace LegacyRenewalApp;

public static class EmailSender
{
    public static void SendEmail(Customer customer, RenewalInvoice invoice, string normalizedPlanCode)
    {
        string subject = "Subscription renewal invoice";
        string body =
            $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
            $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

        LegacyBillingGateway.SendEmail(customer.Email, subject, body);
    }
}