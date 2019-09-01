using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DualSub.Models
{
    public class PlexData
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string File { get; set; }
        public long AddedAt { get; set; }
        public bool HasDualSub { get; set; }
    }
}
