using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroondASP_Net.Models
{
    public class MicrowaveCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public int Power { get; set; }
        public string Warning { get; set; }
        public int SymbolId { get; set; }
        public string Symbol { get; set; }
        public bool Deletable { get; set; }
        public bool Updatable { get; set; }
        public bool Customizable { get; set; }
    }
    public class MicrowaveSave
    {
        public List<string> Available { get; set; }
        public List<Program> Programs { get; set; }
    }
}