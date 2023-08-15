using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using CRM.Authorization.Users;
using CRM.Entities;
using CRM.IoC;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BackgroundWorker
{
    public class EmailBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public IWorkScope WorkScope { get; set; }
        private readonly IEmailSender _emailSender;

        public EmailBackgroundWorker(AbpTimer timer, IEmailSender emailSender) : base(timer)
        {
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
            Timer.Period = 86400000;
            _emailSender = emailSender;
            LocalizationSourceName = CRMConsts.LocalizationSourceName;
        }
        [UnitOfWork]
        protected override void DoWork()
        {
            ScanAndPush();
        }
        private void ScanAndPush()
        {
            var allInvoice = (from i in WorkScope.GetAll<Invoice>().Where(s => s.Status == Enums.StatusEnum.InvoiceStatus.Pending || s.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
                              join c in WorkScope.GetAll<Contract>().Where(s => s.Type == Enums.StatusEnum.ContractType.FixedPrice)
                              on i.ContractId equals c.Id
                              select new Invoice
                              {
                                  Id = i.Id,
                                  Type = i.Type,
                                  Value = i.Value,
                                  Name = i.Name,
                                  InvoiceDate = i.InvoiceDate,
                                  Status = i.Status,
                                  Assignee = i.Assignee,
                                  Currency = i.Currency,
                                  Order = i.Order
                              }).ToList();
            var currentDate = DateTime.Now.Date.AddDays(10);
            var users = WorkScope.GetAll<User>();
            foreach (var invoice in allInvoice)
            {
                var currentAssigne = users.FirstOrDefault(u => u.Id == invoice.Assignee);
                if (currentDate >= invoice.InvoiceDate && invoice.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
                {
                    var email = new EmailBackgroundDto
                    {
                        ReceiverEmails = new List<string> { currentAssigne.EmailAddress },
                        Body = $@"
                                <h4>Trong 10 ngày nữa, sẽ có những hóa đơn sau sẽ đến hạn</h4>
                            <br>
                            <div>
                            <table>
                              <tr>
                                <th> Tên Hóa Đơn </th>
                                <th> Ngày Đến Hạn </th>
                                <th> Giá Trị Hóa Đơn </th>
                              </tr>
                              <tr>
                                <td> {invoice.Name} </td>
                                <td> {invoice.InvoiceDate} </td>
                                <td> {invoice.Value} </td>
                              </tr>
                            </table>
                             </div>
                              <div>
                                <br>
                               <h4>Bạn có thể theo dõi hóa đơn tại hệ thống CRM của công ty.<h4>
                                <br>
                                <h4>Best Regards.</h4>
                              </div>",
                        Subject = $"Xin chào {currentAssigne.FullName}"
                    };
                    SendEmail(email);
                }
                if (currentDate == invoice.InvoiceDate.Date && invoice.Status == Enums.StatusEnum.InvoiceStatus.Pending)
                {
                    invoice.Status = Enums.StatusEnum.InvoiceStatus.Chasing;
                    CurrentUnitOfWork.SaveChanges();
                }
                var datePendingFromInvoice = (currentDate).Subtract(invoice.InvoiceDate.Date).Days;
                if (datePendingFromInvoice % 10 == 0)
                {
                    var email = new EmailBackgroundDto
                    {
                        ReceiverEmails = new List<string> { currentAssigne.EmailAddress },
                        Body = $@"
                                <h3>Hôm nay có những hóa đơn đã đến hạn thanh toán nhưng vẫn chưa được thanh toán. Bạn vui lòng kiểm tra để có thể theo dõi kịp thời nhé</h3>
                                <br>
                            <div>
                            <table>
                              <tr>
                                <th> Tên Hóa Đơn </th>
                                <th> Ngày Đến Hạn </th>
                                <th> Giá Trị Hóa Đơn </th>
                              </tr>
                              <tr>
                                <td> {invoice.Name} </ td >
                                <td> {invoice.InvoiceDate} </ td >
                                <td> {invoice.Value}</ td >
                              </tr>
                            </table>
                             </div>
                               <div>
                                <br>
                               <h4>Bạn có thể theo dõi hóa đơn tại hệ thống CRM của công ty.<h4>
                                <br>
                                <h4>Best Regards.</h4>
                              </div>",
                        Subject = $"Xin chào {currentAssigne.FullName}"
                    };
                    SendEmail(email);
                }
            }
        }


        private void SendEmail(EmailBackgroundDto args)
        {
            try
            {
                Logger.Info("Email job was excecute");
                foreach (var email in args.ReceiverEmails)
                {
                    _emailSender.Send(
                        to: email,
                        subject: args.Subject,
                        body: args.Body,
                        isBodyHtml: true
                    );
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
