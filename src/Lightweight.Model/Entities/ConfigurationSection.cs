namespace Lightweight.Model.Entities
{
    public class ConfigurationSection : IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual Configuration Configuration { get; set; }
    }
}