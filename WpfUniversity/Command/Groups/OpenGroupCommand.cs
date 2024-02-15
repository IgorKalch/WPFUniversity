using UniversityDataLayer.Entities;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.StartUpHelpers;
using WpfUniversity.ViewModels.Courses;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.Views.Groups;

namespace WpfUniversity.Command.Groups
{
    public class OpenGroupCommand : CommandBase
    {
        private readonly Group _group;
        private readonly ModalNavigationService _modalNavigationService;

        public OpenGroupCommand(Group group ,ModalNavigationService modalNavigationService)
        {
            _group = group;
            _modalNavigationService = modalNavigationService;
        }

        public override void Execute(object parameter)
        {
            var model = new GroupViewModel(_group);
            _modalNavigationService.CurrentViewModel = model;
        }
    }
}
