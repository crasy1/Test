using Godot;
using System;

public class Main : Node2D
{
    public override void _Ready()
    {
        var comboDatas = FileUtil.LoadJson<ComboData>("res://combo.json");
        GD.Print(GD.Var2Str(comboDatas));
    }
}