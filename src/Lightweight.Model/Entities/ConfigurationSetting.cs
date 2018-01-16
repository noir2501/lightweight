namespace Lightweight.Model.Entities
{
    public class ConfigurationSetting: IEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string DefaultValue { get; set; }

        public virtual ConfigurationSection Section { get; set; }
    }
}
