namespace Povala.Api.Models
{
    public class Gym
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public ICollection<Boulder> Boulders { get; set; } = new List<Boulder>();
    }
}
