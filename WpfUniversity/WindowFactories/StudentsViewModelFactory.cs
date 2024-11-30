﻿using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.ViewModels.Students;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.WindowFactories;

public class StudentsViewModelFactory : IStudentsViewModelFactory
{
    private readonly IStudentService _studentService;

    public StudentsViewModelFactory(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public StudentsViewModel Create(Group selectedGroup, IWindowService windowService)
    {
        return new StudentsViewModel(_studentService, windowService, selectedGroup);
    }
}