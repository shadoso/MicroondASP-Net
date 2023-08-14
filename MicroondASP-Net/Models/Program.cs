namespace MicroondASP_Net.Models
{
    public class Program
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public int Power { get; set; }
        public string Warning { get; set; }
        public string Symbol { get; set; }
        public bool Deletable { get; set; }
        public bool Updatable { get; set; }
        public bool Customizable { get; set; }
    }
}