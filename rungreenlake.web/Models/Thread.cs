﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rungreenlake.web.Areas.Identity.Data;

namespace rungreenlake.Models
{
    public class Thread
    {

        public int ThreadID { get; set; }
        public int InitiatorID { get; set; }
        public int ReceiverID { get; set; }

        public ICollection<Conversation> Conversations { get; set; }
        public Profile InitiatorProfile { get; set; }
        public Profile ReceiverProfile { get; set; }
    }
}
