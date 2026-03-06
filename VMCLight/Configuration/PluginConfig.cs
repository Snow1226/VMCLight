using System.Runtime.CompilerServices;
using UnityEngine;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace VMCLight.Configuration;

public class PluginConfig
{
    public static PluginConfig Instance { get; set; }

    [NonNullable]
    public virtual string VMCProtocolAddress { get; set; } = "127.0.0.1";
    [NonNullable]
    public virtual int VMCProtocolPort { get; set; } = 39540;
    [NonNullable]
    public virtual Color BlendColor { get; set; } = Color.white;
    [NonNullable]
    public virtual float BlendIntensity { get; set; } = 1.0f;
}