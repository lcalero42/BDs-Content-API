using Zorro.Settings;
using UnityEngine;

namespace DbsContentApi;

/// <summary>
/// Base class for custom keybind inputs in Content Warning mods.
/// Extend this class to define a keybind with down/up/held callbacks; instances are automatically
/// registered with the settings menu and input handler when constructed.
/// </summary>
public abstract class BaseCWInput : KeyCodeSetting
{
    /// <summary>
    /// Polls the bound key and dispatches <see cref="OnKeyDown"/>, <see cref="OnKeyUp"/>, or <see cref="OnHeld"/> as appropriate.
    /// Called by the API's input patch each frame.
    /// </summary>
    /// <param name="player">The local player instance receiving the input.</param>
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

    /// <summary>Returns the default key binding for this input.</summary>
    protected abstract override KeyCode GetDefaultKey();

    /// <inheritdoc/>
    public override int GetDefaultValue() => (int)GetDefaultKey();

    /// <summary>Called once when the bound key is pressed.</summary>
    /// <param name="player">The local player instance.</param>
    protected abstract void OnKeyDown(Player player);

    /// <summary>Called once when the bound key is released.</summary>
    /// <param name="player">The local player instance.</param>
    protected abstract void OnKeyUp(Player player);

    /// <summary>Called every frame while the bound key is held.</summary>
    /// <param name="player">The local player instance.</param>
    protected abstract void OnHeld(Player player);

    internal GlobalInputHandler.InputKey inputKey;

    /// <summary>Disables input handling for this keybind without removing it from settings.</summary>
    public void Disable()
    {
        _disabled = true;
    }

    /// <summary>Re-enables input handling for this keybind.</summary>
    public void Enable()
    {
        _disabled = false;
    }

    /// <summary>
    /// Creates and registers this input with the API.
    /// Subclasses should not override this constructor.
    /// </summary>
    public BaseCWInput()
    {
        inputKey = new GlobalInputHandler.InputKey();

        DbsContentApiPlugin._inputs.Add(this);
        _disabled = false;
        ApiLog.Log($"[BaseCWInput] Created input: {GetType().Name}, default key: {GetDefaultKey()}");
    }

    /// <summary>When <c>true</c>, key polling is skipped for this input.</summary>
    protected bool _disabled;
}
