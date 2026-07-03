namespace Povala.Api.Models
{
    public class Gym
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Boulder> Boulders { get; set; } = new List<Boulder>();
    }
}
