using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rungreenlake.web.Areas.Identity.Data;

namespace rungreenlake.Models.ViewModels
{
    public class ProfileViewModel
    {
        public RungreenlakeUser MyUser { get; set; }
        public Profile MyProfile { get; set; }

        //Only used when viewing other profiles, otherwise a null value.
        public int? BuddyFlag { get; set; }

        //Three separate lists to group buddy states.
        public List<RungreenlakeUser> MyListFriends { get; set; }
        public List<RungreenlakeUser> MyListPending { get; set; }
        public List<RungreenlakeUser> MyListBlocked { get; set; }
        public IEnumerable<RaceRecord> MyRaceRecords { get; set; }

        /*
        public User myProfile { get; set; }

        public RaceRecord myBestTime { get; set; }

        public ICollection<User> MyBuddyList { get; set; }
        public ICollection<RaceRecord> MyRaceRecords { get; set; }
        */
    }
}
