namespace CRM.ExportExcel.Dto
{
    public class FileBase64Dto
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Base64 { get; set; }
    }

    public class InvoiceBillingExcelDto
    {
        public string Description { get; set; }
        public double? Quantity { get; set; }
        public double? UnitPrice { get; set; }
    }

    public class InvoiceUserBillingExcelDto
    {
        public string Name { get; set; }
        public float WorkingDay { get; set; }
        public double DailyRate { get; set; }
    }
}