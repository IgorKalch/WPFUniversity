using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityDataLayer.Entities;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.Services.Students;

public class StudentService : IStudentService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly List<Student> _students = [];
    public List<Student> Students => _students;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task LoadStudentsByGroup(int groupId)
    {
        Students.Clear();

        var studentsToAdd = await _unitOfWork.StudentRepository.GetAsync(x => x.GroupId == groupId);

        foreach (var student in studentsToAdd)
        {
            var studentToAdd = _unitOfWork.StudentRepository.GetById(student.Id);
            Students.Add(studentToAdd);
        }
    }

    public IEnumerable<Student> GetStudentsByGroup(int groupId)
    {
        return _unitOfWork.StudentRepository.Get(s => s.GroupId == groupId).ToList();
    }

    public void AddStudentToGroup(int groupId, Student student)
    {
        student.GroupId = groupId;
        _unitOfWork.StudentRepository.Add(student);
        _unitOfWork.Commit();
    }

    public void ClearStudentsInGroup(int groupId)
    {
        var students = _unitOfWork.StudentRepository.Get(s => s.GroupId == groupId);
        _unitOfWork.StudentRepository.RemoveRange(students);
        _unitOfWork.Commit();
    }

    public async Task Add(Student student)
    {
        _unitOfWork.StudentRepository.Add(student);
        _unitOfWork.Commit();
    }

    public async Task Update(Student student)
    {
        var studentToUpdate = _unitOfWork.StudentRepository.GetById(student.Id);

        if (studentToUpdate != null)
        {
            studentToUpdate.FirstName = student.FirstName;
            studentToUpdate.LastName = student.LastName;
            studentToUpdate.GroupId = student.GroupId;

            _unitOfWork.StudentRepository.Update(studentToUpdate);
            _unitOfWork.Commit();
        }
        else
        {
            throw new Exception("Student not found.");
        }
    }

    public async Task Delete(Student student)
    {
        _unitOfWork.StudentRepository.Remove(student);
        _unitOfWork.Commit();
    }
}
