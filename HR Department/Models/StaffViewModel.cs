namespace HR_Department
{
    public class StaffViewModel
    {
        private readonly Staff _s;

        public StaffViewModel(Staff staff) => _s = staff;

        public int staff_id => _s.staff_id;
        public string full_name => _s.full_name;
        public string service => _s.service;
        public string phone_number => _s.phone_number;
        public string email => _s.email;
        public string birth_date_str => _s.birth_date_str;
        public string city => _s.city;
        public int post_number => _s.post_number;

        public Staff GetStaff() => _s;
    }
}
