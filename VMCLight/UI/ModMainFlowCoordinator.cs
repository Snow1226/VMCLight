using BeatSaberMarkupLanguage;
using HMUI;
using VMCLight.Configuration;

namespace VMCLight.UI;

public class ModMainFlowCoordinator : FlowCoordinator
{
    private const string titleString = "VMC Light";
    private VMCLightUI lightUI;

    public bool IsBusy { get; set; }
    private void Awake()
    {
        this.lightUI = BeatSaberUI.CreateViewController<VMCLightUI>();
        this.lightUI.mainFlowCoordinator = this;
    }
    
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        SetTitle(titleString);
        this.showBackButton = true;

        var viewToDisplay = DecideMainView();

        this.IsBusy = true;
        ProvideInitialViewControllers(viewToDisplay);
        this.IsBusy = false;
    }
    
    private ViewController DecideMainView()
    {
        ViewController viewToDisplay;
        viewToDisplay = this.lightUI;
        return viewToDisplay;
    }
    
    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        if (this.IsBusy) return;
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
    }
}