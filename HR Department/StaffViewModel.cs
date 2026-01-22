using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace HR_Department
{
    public class StaffViewModel
    {
        private readonly Staff _staff;

        public StaffViewModel(Staff staff)
        {
            _staff = staff;
        }

        public int staff_id => _staff.staff_id;
        public string full_name => _staff.full_name;
        public string service => _staff.full_name;
        public string phone_number => _staff.phone_number;
        public string email => _staff.email;
        public DateTime birth_date => _staff.birth_date;
        public string city => _staff.city;
        public int post_number => _staff.post_number;

        public string birth_date_str => birth_date.ToString("dd.MM.yyyy");
    }
}
