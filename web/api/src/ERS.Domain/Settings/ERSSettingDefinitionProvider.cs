using Volo.Abp.Settings;

namespace ERS.Settings;

public class ERSSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ERSSettings.MySetting1));
    }
}
