using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityDataLayer.Entities
{
    public class Teacher : Entity
    {
        public int CourseId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Subject { get; set; }
        public List<Group>? Groups { get; set; }
        public Course? Course { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
