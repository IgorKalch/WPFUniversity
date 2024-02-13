﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;

namespace WpfUniversity.Services.Courses;

public class CourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly List<Course> _courses = new();
    public List<Course> Courses => _courses;

    public event Action CourseLoaded;
    public event Action<Course> CourseAdded;
    public event Action<Course> CourseUpdated;
    public event Action<Course> CourseDeleted;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Load()
    {            
        Courses.Clear();

        var coursesToAdd = _unitOfWork.CourseRepository.Get();

        foreach (var course in coursesToAdd)
        {
            var courseToadd = _unitOfWork.CourseRepository.GetById(course.Id);
            Courses.Add(courseToadd);
        }

        CourseLoaded?.Invoke();
    }

    public async Task Add(Course course)
    {
        _unitOfWork.CourseRepository.Add(course);
        _unitOfWork.Commit();

        CourseAdded?.Invoke(course);
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

            CourseUpdated?.Invoke(courseToUpdate);
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

        CourseDeleted?.Invoke(course);
    }
}