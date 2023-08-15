namespace CRM.Configuration
{
    public static class AppSettingNames
    {
        public const string UiTheme = "App.UiTheme";
        public const string StorageLocation = "StorageLocation";
        public const string ClientAppId = "App.ClientAppId";

        public class EmailTemplate
        {
            public const string InvoiceEmailHeader = "Crm.Email.InvoiceEmailHeader";
            public const string InvoiceBefore10dayEmail = "Crm.Email.InvoiceBefore10dayEmail";
            public const string InvoiceAfter10dayEmail = "Crm.Email.InvoiceBAfter10dayEmail";
        }
    }
}
