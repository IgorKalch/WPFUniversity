using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.Services.Courses;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly List<Course> _courses = [];
    public List<Course> Courses => _courses;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Load()
    {
        Courses.Clear();

        var coursesToAdd = await Task.Run(() => _unitOfWork.CourseRepository.Get());

        Courses.AddRange(coursesToAdd);
    }

    public async Task Add(Course course)
    {
        _unitOfWork.CourseRepository.Add(course);
        _unitOfWork.Commit();

        await Load();
    }

    public async Task Update(Course course)
    {
        var courseToUpdate = _unitOfWork.CourseRepository.GetById(course.Id);

        if (courseToUpdate != null)
        {
            courseToUpdate.Name = course.Name;
            courseToUpdate.Description = course.Description;

            _unitOfWork.CourseRepository.Update(courseToUpdate);
            _unitOfWork.Commit();

            await Load();
        }
        else
        {
            throw new Exception("Course not found.");
        }
    }

    public async Task Delete(Course course)
    {
        _unitOfWork.CourseRepository.Remove(course);
        _unitOfWork.Commit();

        await Load();
    }
}