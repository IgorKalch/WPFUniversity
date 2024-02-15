using System;
using System.Collections.ObjectModel;
using System.Linq;
using UniversityDataLayer.Entities;
using WpfUniversity.Services.Groups;

namespace WpfUniversity.ViewModels.Groups;

public class GroupTreeViewModel : ViewModelBase
{
    private readonly SelectedGroupService _selectedGroupService;

    public ObservableCollection<Group> Groups { get; set; }

    public GroupTreeViewModel(ObservableCollection<Group> groups, SelectedGroupService selectedGroupService)
    {
        Groups = groups;
        _selectedGroupService = selectedGroupService;
    }

    public Group SelectedGroup
    {
        get
        {
            return Groups.FirstOrDefault(g => g.Id == _selectedGroupService?.SelectedGroup?.Id);            
        }
        set
        {
            if (_selectedGroupService != null)
            {
                _selectedGroupService.SelectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }
    }
}
