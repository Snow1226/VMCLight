using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;

namespace VMCLight.UI;

internal static class UIManager
{
    internal static FlowCoordinator _mainFlowCoordinator { get; private set; }

    public static void Init()
    {
        MenuButton menuButton =
            new MenuButton("VMC Light", "Send Light Data with VMCProtocol ", ShowModFlowCoordinator, true);
        MenuButtons.Instance.RegisterButton(menuButton);    
    }

    public static void ShowModFlowCoordinator()
    {
        if(Plugin.Instance.LightController.VMCLightMainFlowCoordinator == null)
            Plugin.Instance.LightController.VMCLightMainFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModMainFlowCoordinator>();
        if(Plugin.Instance.LightController.VMCLightMainFlowCoordinator.IsBusy) return;
        
        BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(Plugin.Instance.LightController.VMCLightMainFlowCoordinator);
    }
}