
using SharedLib.Interfaces;


namespace SharedLib.Models
{
    public partial class ServerLog : IModel
    {
        public int Id { get; set; }

        public string Text { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
