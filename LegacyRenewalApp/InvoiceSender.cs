namespace LegacyRenewalApp;

public static class InvoiceSender
{
    public static void sendInvoice(RenewalInvoice invoice)
    {
        LegacyBillingGateway.SaveInvoice(invoice);
    }
}