using Microsoft.EntityFrameworkCore;
using UniversityDataLayer;
using UniversityDataLayer.Entities;
using UniversityDataLayer.Repositories;
using UniversityDataLayer.UnitOfWorks;

namespace UniversityDataLayerTest
{
    [TestClass]
    public class UniversityContextTest
    {
        [TestMethod]
        public void GetCourses_Success()
        {
            var options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            var coursesExpected = new List<Course>
            {
                new Course { Id = 1, Name = "Mathematics", Description = "Advanced calculus and algebra" },
                new Course { Id = 2, Name = "Physics", Description = "Classical mechanics and electromagnetism" },
                new Course { Id = 3, Name = "Computer Science", Description = "Data structures and algorithms" }
            };

            using var dbContext = new UniversityContext(options);
            using var unit = new UnitOfWork(dbContext);
            {
                dbContext.Database.EnsureCreated();
                dbContext.AddRange(coursesExpected);
                dbContext.SaveChanges();

                var courseList = unit.CourseRepository.Get().ToList();

                Assert.AreEqual(coursesExpected.Count, courseList.Count);
            }
        }

        [TestMethod]
        public void AddCourses_Success()
        {
            var options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "TestDB2")
                .Options;

            var coursesExpected = new List<Course>
            {
                new Course { Id = 1, Name = "Mathematics", Description = "Advanced calculus and algebra" },
                new Course { Id = 2, Name = "Physics", Description = "Classical mechanics and electromagnetism" },
                new Course { Id = 3, Name = "Computer Science", Description = "Data structures and algorithms" }
            };

            var coursesList = new List<Course>
            {
                new Course { Id = 1, Name = "Mathematics", Description = "Advanced calculus and algebra" },
                new Course { Id = 2, Name = "Physics", Description = "Classical mechanics and electromagnetism" }
            };

            var courseToAdd = new Course { Name = "Computer Science", Description = "Data structures and algorithms" };

            using var dbContext = new UniversityContext(options);
            using var unit = new UnitOfWork(dbContext);
            {
                dbContext.Database.EnsureCreated();
                dbContext.AddRange(coursesList);
                dbContext.SaveChanges();

                unit.CourseRepository.Add(courseToAdd);
                unit.Commit();

                var courseActual = unit.CourseRepository.Get().ToList();

                Assert.AreEqual(coursesExpected.Count, courseActual.Count);
            }
        }

        [TestMethod]
        public void EditCourses_Success()
        {
            var options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "TestDB3")
                .Options;

            var courseExpected = new Course { Id = 1, Name = "Computer Science", Description = "Advanced calculus and algebra"};
            var courseToAdd = new Course { Id = 1, Name = "Mathematics", Description = "Advanced calculus and algebra"};
            var courseToEdit = new Course { Id = 1, Name = "Computer Science", Description = "Advanced calculus and algebra" };

            using var dbContext = new UniversityContext(options);
            using var unit = new UnitOfWork(dbContext);
            {
                dbContext.Database.EnsureCreated();
                dbContext.Add(courseToAdd);
                dbContext.SaveChanges();

                var courseActual = unit.CourseRepository.GetById(courseToAdd.Id);
                courseActual.Name = courseToEdit.Name;

                unit.Commit();

                Assert.IsNotNull(courseActual);
                Assert.AreEqual(courseExpected.Name, courseActual.Name);
            }
        }

        [TestMethod]
        public void DeleteCourses_Success()
        {
            var options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "TestDB4")
                .Options;

            var coursesList = new List<Course>
            {
                new Course { Id = 1, Name = "Mathematics", Description = "Advanced calculus and algebra" },
                new Course { Id = 2, Name = "Physics", Description = "Classical mechanics and electromagnetism" },
                new Course { Id = 3, Name = "Computer Science", Description = "Data structures and algorithms" }
            };

            var courseToDelete = new Course { Id = 3, Name = "Computer Science", Description = "Data structures and algorithms" };

            var coursesExpected = new List<Course>
            {
                new Course { Id = 1, Name = "Mathematics", Description = "Advanced calculus and algebra" },
                new Course { Id = 2, Name = "Physics", Description = "Classical mechanics and electromagnetism" }
            };

            using var dbContext = new UniversityContext(options);
            {
                dbContext.Database.EnsureCreated();
                dbContext.AddRange(coursesList);
                dbContext.SaveChanges();
            }

            using var dbContextUnit = new UniversityContext(options);
            using var unit = new UnitOfWork(dbContextUnit);
            {              
                unit.CourseRepository.Remove(courseToDelete);
                unit.Commit();

                var courseActual = unit.CourseRepository.Get().ToList();


                Assert.AreEqual(coursesExpected.Count, courseActual.Count);
            }
        }
    }
}