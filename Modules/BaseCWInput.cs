using Zorro.Settings;
using UnityEngine;

namespace DbsContentApi.Modules;

public abstract class BaseCWInput : KeyCodeSetting
{
    public void HandleKeys(Player player)
    {
        if (_disabled) return;
        
        KeyCode key = Keycode();
        
        if (Input.GetKeyDown(key))
            OnKeyDown(player);
        else if (Input.GetKeyUp(key))
            OnKeyUp(player);
        else if (Input.GetKey(key))
            OnHeld(player);
    }
    protected abstract override KeyCode GetDefaultKey();
    public override int GetDefaultValue() => (int)GetDefaultKey();

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
        Logger.Log($"[BaseCWInput] Created input: {GetType().Name}, default key: {GetDefaultKey()}");
    }

    protected bool _disabled;
}
