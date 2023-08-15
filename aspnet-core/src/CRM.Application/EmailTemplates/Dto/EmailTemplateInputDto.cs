using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.EmailTemplates.Dto
{
    public class EmailTemplateInputDto
    {
        public long? Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
