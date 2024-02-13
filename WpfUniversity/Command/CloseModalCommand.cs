﻿using WpfUniversity.Services;

namespace WpfUniversity.Command;

public class CloseModalCommand : CommandBase
{
    private readonly ModalNavigationService _modalNavigationService;

    public CloseModalCommand(ModalNavigationService modalNavigationService)
    {
        _modalNavigationService = modalNavigationService;
    }

    public override void Execute(object parameter)
    {
        _modalNavigationService.Close();
    }
}