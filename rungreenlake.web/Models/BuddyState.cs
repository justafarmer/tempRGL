using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using rungreenlake.web.Areas.Identity.Data;

namespace rungreenlake.Models
{
    public class BuddyState
    {

        //First User ID
        public int FirstProfileID { get; set; }
        
        //Second User ID.
        public int SecondProfileID { get; set; }
        
        // 1 = Matched, 2 = Requested, 3 = Blocked
        public int Status { get; set; }

        public Profile FirstProfile { get; set; }
        public Profile SecondProfile { get; set; }

    }
}
