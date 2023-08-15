using Abp.UI;
using CRM.Entities;
using CRM.ExportExcel.Dto;
using CRM.Net.MimeTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.ExportExcel
{
    public class ExportExcelAppService : CRMAppServiceBase
    {
        private readonly string templateFolder = Path.Combine(CRMConsts.wwwRootFolder, "template");

        /// <summary>
        /// xuất file excel hoá đơn
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FileBase64Dto> ExportInvoice(long invoiceId)
        {
            #region Get data to export excel

            var invoice = await WorkScope.GetAsync<Invoice>(invoiceId);
            if (invoice == null)
            {
                throw new UserFriendlyException("Không tìm thấy Invoice");
            }
            var contract = await WorkScope.GetAsync<Contract>(invoice.ContractId);
            var project = await WorkScope.GetAll<Project>().Include(x => x.Client).FirstOrDefaultAsync(p => p.Id == contract.ProjectId);
            var invoiceBilling = new List<InvoiceBillingExcelDto>();
            var invoiceUserBilling = new List<InvoiceUserBillingExcelDto>();
            var templateFilePath = string.Empty;
            if (contract.Type == ContractType.Internal)
            {
                throw new UserFriendlyException("Dự án nội bộ");
            }

            if (contract.Type == ContractType.FixedPrice)
            {
                templateFilePath = Path.Combine(templateFolder, "InvoiceTemplate.xlsx");
                invoiceBilling.Add(new InvoiceBillingExcelDto
                {
                    Description = project.Description,
                    Quantity = null,
                    UnitPrice = null
                });

                invoiceBilling.Add(new InvoiceBillingExcelDto
                {
                    Description = string.Empty,
                    Quantity = 1,
                    UnitPrice = invoice.Value,
                });
            }

            else
            {
                templateFilePath = Path.Combine(templateFolder, "InvoiceUserTemplate.xlsx");
                var invoiceUsers = await WorkScope.GetAll<InvoiceUser>().Include(iu => iu.User).Where(iu => iu.InvoiceId == invoice.Id).ToListAsync();
                foreach (var invoiceUser in invoiceUsers)
                {
                    invoiceUserBilling.Add(new InvoiceUserBillingExcelDto
                    {
                        Name = invoiceUser.User.FullName,
                        WorkingDay = invoiceUser.ManMonth,
                        DailyRate = invoiceUser.Rate
                    });
                }
            }

            #endregion

            var result = new FileBase64Dto();
            try
            {
                using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
                {
                    using (var excelPackageIn = new ExcelPackage(memoryStream))
                    {
                        var invoiceSheet = excelPackageIn.Workbook.Worksheets[0];
                        var companySetupSheet = excelPackageIn.Workbook.Worksheets[1];

                        #region Fill dat into Invoice sheet

                        if (contract.Type == ContractType.FixedPrice)
                        {
                            invoiceSheet.Names["TenKhachHang"].Value = project.Client.Name;
                        }

                        else
                        {
                            invoiceSheet.Names["TenProject"].Value = project.Name;
                        }

                        invoiceSheet.Names["DiaChiKhachHang"].Value = project.Client.Country;
                        // insert billing table
                        var invoiceDetailTable = invoiceSheet.Tables.First();
                        var invoiceDetailTableStart = invoiceDetailTable.Address.Start;
                        if (contract.Type == ContractType.FixedPrice)
                        {
                            int d = 0;
                            for (int i = invoiceDetailTableStart.Row + 1; i <= invoiceDetailTable.Address.End.Row; i++)
                            {
                                for (int j = invoiceDetailTable.Address.Start.Column; j <= invoiceDetailTable.Address.End.Column; j++)
                                {
                                    //add the cell data to the List
                                    switch (j)
                                    {
                                        case 2:
                                            invoiceSheet.Cells[i, j].Value = invoiceBilling[d].Description;
                                            break;
                                        case 3:
                                            invoiceSheet.Cells[i, j].Value = invoiceBilling[d].Quantity;
                                            break;
                                        case 4:
                                            invoiceSheet.Cells[i, j].Value = invoiceBilling[d].UnitPrice;
                                            break;
                                        case 5:
                                            if (i > (invoiceDetailTableStart.Row + 1))
                                                invoiceSheet.Cells[i, j].Formula = invoiceSheet.Cells[i, 3] + "*" + invoiceSheet.Cells[i, 4];
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                d++;
                            }
                        }
                        else
                        {
                            if (invoiceUserBilling.Count > 0)
                            {
                                invoiceSheet.InsertRow(invoiceDetailTableStart.Row + 1, invoiceUserBilling.Count - 1, invoiceDetailTableStart.Row + invoiceUserBilling.Count);
                                invoiceSheet.Names["InvoiceNetTotal"].Formula = "=SUM(InvoiceDetails[LINE TOTAL])-Discount";

                                int d = 0;
                                for (int i = invoiceDetailTableStart.Row + 1; i <= invoiceDetailTable.Address.End.Row; i++)
                                {
                                    for (int j = invoiceDetailTable.Address.Start.Column; j <= invoiceDetailTable.Address.End.Column; j++)
                                    {
                                        //add the cell data to the List
                                        switch (j)
                                        {
                                            case 2:
                                                invoiceSheet.Cells[i, j].Value = invoiceUserBilling[d].Name;
                                                break;
                                            case 3:
                                                invoiceSheet.Cells[i, j].Value = invoiceUserBilling[d].WorkingDay;
                                                break;
                                            case 4:
                                                invoiceSheet.Cells[i, j].Value = invoiceUserBilling[d].DailyRate;
                                                break;
                                            default:
                                                invoiceSheet.Cells[i, j].Formula = invoiceSheet.Cells[i, 3] + "*" + invoiceSheet.Cells[i, 4];
                                                break;
                                        }
                                    }
                                    d++;
                                }
                            }
                        }

                        #endregion

                        #region Fill dat into Company Setup sheet
                        companySetupSheet.Cells["C14"].Value = Enum.GetName(typeof(CurrencyType), invoice.Currency);
                        #endregion

                        var fileBytes = excelPackageIn.GetAsByteArray();
                        string fileBase64 = Convert.ToBase64String(fileBytes);

                        result = new FileBase64Dto
                        {
                            FileName = $"{invoice.Name.Replace("/", "").Replace(":", "").Replace(" ", "_")}.xlsx",
                            FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                            Base64 = fileBase64
                        };
                        //try
                        //{
                        //    using (var fs = new FileStream(templateFolder + "/" + result.FileName, FileMode.Create, FileAccess.Write))
                        //    {
                        //        fs.Write(fileBytes, 0, fileBytes.Length);

                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("Exception caught in process: {0}", ex);

                        //}
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Không thể xuất ra file excel");
            }
        }
    }
}