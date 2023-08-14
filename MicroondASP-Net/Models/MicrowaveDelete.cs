using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroondASP_Net.Models
{
    public class MicrowaveDelete
    {
        public int ProgramId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool Deletable { get; set; }
    }
    public class MicrowaveRemove
    {
        public List<string> Available { get; set; }
        public List<Program> Programs { get; set; }
    }
}