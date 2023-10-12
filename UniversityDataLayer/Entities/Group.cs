namespace UniversityDataLayer.Entities
{
    public class Group : Entity
    {
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public string? Name { get; set; }
        public List<Student>? Students { get; set; }
        public Course? Course { get; set; }
        public Teacher? Techer { get; set; }
    }
}
