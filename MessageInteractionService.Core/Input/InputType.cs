namespace MessageInteractionService.Core.Input;

public enum InputType
{
    /// <summary>
    /// Input is an ordinal number referencing to a menu item position. Navigational values also acceptable
    /// </summary>
    ItemPosition = 1,
    /// <summary>
    /// Input is free text
    /// </summary>
    Text,
    /// <summary>
    /// Input is a date
    /// </summary>
    Date,
}