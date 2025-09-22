using System;
using FreeSql.DataAnnotations;

namespace WebApplication3.Models.DB
{
    public class BrowsingRecord
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime VisitTime { get; set; }
        public string UserId { get; set; }
        public string Host { get; set; }
    }
}
