using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Matilda.WebApi.Plugins;

public class LightPlugin(ILogger<LightPlugin> logger)
{
    public bool IsOn { get; set; }
    
    [KernelFunction("GetLightState")]
    [Description("Gets the current state of the light.")]
    public string GetState() => IsOn ? "on" : "off";

    [KernelFunction("ChangeLightState")]
    [Description("Change the state of the light.")]
    public string ChangeState(bool newState)
    {
        IsOn = newState;
        
        var state = GetState();
        logger.LogInformation("[Light is now {state}]", state);

        return state;
    }
}
