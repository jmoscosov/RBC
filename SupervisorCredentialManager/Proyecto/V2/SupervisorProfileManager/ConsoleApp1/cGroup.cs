using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SupervisorProfileManager
{
    public class cGroup
    {
        public string ID { get; set; }
        public string Denied { get; set; }
        public string Name { get; set; }
        public List<cUser> Users { get; set; }
    }
}
