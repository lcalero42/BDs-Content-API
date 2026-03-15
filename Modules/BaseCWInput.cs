using Zorro.Settings;

namespace DbsContentApi.Modules;

public abstract class BaseCWInput : KeyCodeSetting
{
    public void HandleKeys(Player player)
    {
        if (_disabled) return;
        if (inputKey.GetKeyDown())
        {
            OnKeyDown(player);
        }
        else if (inputKey.GetKeyUp())
        {
            OnKeyUp(player);
        }
        else if (inputKey.GetKey())
        {
            OnHeld(player);
        }
    }
    protected abstract void OnKeyDown(Player player);

    protected abstract void OnKeyUp(Player player);

    protected abstract void OnHeld(Player player);
    internal GlobalInputHandler.InputKey inputKey;

    public void Disable()
    {
        _disabled = true;
    }

    public void Enable()
    {
        _disabled = false;
    }

    public BaseCWInput()
    {
        inputKey = new GlobalInputHandler.InputKey();

        DbsContentApiPlugin._inputs.Add(this);
        _disabled = false;
    }

    protected bool _disabled;
}
