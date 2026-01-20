using System;

namespace HR_Department.Models
{
    public class Staff
    {
        public int staff_id { get; set; }
        public string full_name { get; set; }
        public string service { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime birth_date { get; set; }
        public string city { get; set; }
        public int post_number { get; set; }

    }
}
