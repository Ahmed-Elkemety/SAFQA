using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class MessageAttachment
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
