using System.Collections.Generic;
using System.Drawing;
using System.Net.Mime;
using System.Xml;
using GoOS.GUI;

namespace GoOS.GoCode;

public class GoWindow
{
    public int Height { get; private set;  }
    public int Width { get; private set; }
    public string Name { get; private set; }

    public Dictionary<string, bool> ButtonClickers = new Dictionary<string, bool>();
    
    public Window window { get; private set; }

    private string ButtonName = "";

    public GoWindow(int height, int width, string name)
    {
        Height = height;
        Width = width;
        Name = name;

        window = new Window();
        window.Contents.Height = (ushort)Height;
        window.Contents.Width = (ushort)Width;
        window.Title = Name;
        WindowManager.AddWindow(window);
    }

    public void DrawLine(int x, int y, int xTwo, int yTwo, PrismAPI.Graphics.Color color)
    {
        window.Contents.DrawLine(x, y, xTwo, yTwo, color);        
    }

    public void DrawString(int x, int y, string text, PrismAPI.Graphics.Color color)
    {
        window.Contents.DrawString(x, y, text, Resources.Font_1x, color);
    }

    public void Button(int x, int y, int width, int height, string text)
    {
        Button button = new Button(window, (ushort)x, (ushort)y, (ushort)width, (ushort)height, text);
        if (ButtonClickers.ContainsKey(text))
        {
            ButtonClickers.Remove(text);
        }
        ButtonClickers.Add(text, false);
        
        button.ClickedAlt = OnClick;
    }
    
    private void OnClick(string name)
    {
        if (ButtonClickers.ContainsKey(name))
        {
            ButtonClickers.Remove(name);
        }
        
        ButtonClickers.Add(name, true);
    }

    public void ResetButton(string name)
    {
        if (ButtonClickers.ContainsKey(name))
        {
            ButtonClickers.Remove(name);
        }
        
        ButtonClickers.Add(name, false);
    }
}