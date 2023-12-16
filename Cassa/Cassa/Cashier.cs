using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassa
{
    public class Cashier
    {
        public int ID { get; }
        public string Fullname { get; }
        public string Birthdate { get; }
        public long Phone { get; }

        public Cashier(int ID, string fullname, string birthdate, long phone)
        {
            this.ID = ID;
            this.Fullname = fullname;
            this.Birthdate = birthdate;
            this.Phone = phone;
        }
    }
}
