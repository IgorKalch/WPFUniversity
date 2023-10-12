namespace UniversityDataLayer.Entities
{
    public class Course : Entity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<Group>? Groups { get; set; } = default;   
        public List<Teacher>? Teachers { get; set; } = default;   
    }
}
