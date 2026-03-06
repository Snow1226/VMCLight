using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using VMCLight.Configuration;

namespace VMCLight.UI;

public class VMCLightUI : BSMLAutomaticViewController
{
    public ModMainFlowCoordinator mainFlowCoordinator { get; set; }
    public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
    {
        this.mainFlowCoordinator = mainFlowCoordinator;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
    }
    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        Plugin.Instance.LightController.VMCProtocolReconnect();
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
    }

    [UIValue("Address")] private string _address = PluginConfig.Instance.VMCProtocolAddress;

    [UIAction("OnChangeAddress")]
    private void OnChangeAddress(string value)
    {
        _address = value;
        PluginConfig.Instance.VMCProtocolAddress = value;
    }

    [UIValue("Port")] private string _port = PluginConfig.Instance.VMCProtocolPort.ToString();

    [UIAction("OnChangePort")]
    private void OnChangePort(string value)
    {
        _port = value;
        PluginConfig.Instance.VMCProtocolPort = int.Parse(value);
    }
    
    [UIValue("BlendColor")] private Color _blendColor = PluginConfig.Instance.BlendColor;

    [UIAction("OnColorChange")]
    private void OnChangeBlendColor(Color value)
    {
        _blendColor = value;
        PluginConfig.Instance.BlendColor = value;
    }
    
    [UIValue("Intensity")] private float _blendIntensity = PluginConfig.Instance.BlendIntensity;

    [UIAction("OnChangeIntensity")]
    private void OnChangeBlendIntensity(float value)
    {
        _blendIntensity = value;
        PluginConfig.Instance.BlendIntensity = value;
    }
    
}