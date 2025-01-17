﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.Services;

public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private List<Teacher> _teachers = [];
    public List<Teacher> Teachers => _teachers;

    public TeacherService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersByCourseIdAsync(int courseId)
    {
        List<Teacher> result = [];

        var studentsToAdd = await _unitOfWork.TeacherRepository.GetAsync(x => x.CourseId == courseId);

        foreach (var student in studentsToAdd)
        {
            var studentToAdd = _unitOfWork.TeacherRepository.GetById(student.Id);
            result.Add(studentToAdd);
        }

        return result;
    }

    public async Task Load()
    {
        Teachers.Clear();

        var teachers = await _unitOfWork.TeacherRepository.GetAsync();

        foreach (var teacher in teachers)
        {
            var teacherToAdd = _unitOfWork.TeacherRepository.GetById(teacher.Id);
            Teachers.Add(teacher);
        }
    }

    public async Task<Teacher> GetTeacherByIdAsync(int id)
    {
        return await Task.Run(() => _unitOfWork.TeacherRepository.GetById(id));
    }

    public async Task AddTeacherAsync(Teacher teacher)
    {
        _unitOfWork.TeacherRepository.Add(teacher);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateTeacherAsync(Teacher teacher)
    {
        _unitOfWork.TeacherRepository.Update(teacher);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteTeacherAsync(Teacher teacher)
    {
        if (teacher != null)
        {
            _unitOfWork.TeacherRepository.Remove(teacher);
            await _unitOfWork.CommitAsync();
        }
    }
}
