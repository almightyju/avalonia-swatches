using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace AvaloniaColourSwatches.Controls;

[TemplatePart("PART_Container", typeof(Border))]
public class ColourSquare : ContentControl
{
    public static readonly StyledProperty<string> ResourceKeyProperty =
        AvaloniaProperty.Register<ColourSquare, string>(nameof(ResourceKey));

    public string ResourceKey
    {
        get => GetValue(ResourceKeyProperty);
        set => SetValue(ResourceKeyProperty, value);
    }


    public static readonly DirectProperty<ColourSquare, ISolidColorBrush> BrushProperty =
        AvaloniaProperty.RegisterDirect<ColourSquare, ISolidColorBrush>(nameof(Brush), o => o.Brush);

    private ISolidColorBrush _brush = Brushes.Transparent;
    public ISolidColorBrush Brush
	{
        get => _brush;
        private set => SetAndRaise(BrushProperty, ref _brush, value);
    }


	public static readonly DirectProperty<ColourSquare, bool>ColorHasTransparencyProperty =
		AvaloniaProperty.RegisterDirect<ColourSquare, bool>(nameof(ColorHasTransparency),
			o => o.ColorHasTransparency);
	private bool _colorHasTransparency = false;
	public bool ColorHasTransparency 
	{
		get => _colorHasTransparency;
		private set => SetAndRaise(ColorHasTransparencyProperty, ref _colorHasTransparency, value);
	}


	public static readonly DirectProperty<ColourSquare, string> ColorOpacityProperty =
		AvaloniaProperty.RegisterDirect<ColourSquare, string>(nameof(ColorOpacity),
			o => o.ColorOpacity);
	private string _colorOpacity = "100%";
	public string ColorOpacity 
	{
		get => _colorOpacity;
		private set => SetAndRaise(ColorOpacityProperty, ref _colorOpacity , value);
	}




	protected override void OnLoaded(RoutedEventArgs e)
	{
		if (!this.TryFindResource(ResourceKey, ActualThemeVariant, out object? resourceValue))
			throw new Exception($"ResourceKey '{ResourceKey}' is not found");

		if (resourceValue == null)
			throw new NullReferenceException($"ResourceKey '{ResourceKey}' is set but the value is null");

		if (resourceValue is not ISolidColorBrush brush)
			throw new Exception($"Resource key '{ResourceKey}' is of type {resourceValue.GetType().Name} not an {nameof(ISolidColorBrush)}");

		if(brush.Opacity == 1 && brush.Color.A == 255)
			Brush = brush;
		else
		{
			Brush = new SolidColorBrush(brush.Color, 1);
			ColorHasTransparency = true;
			int p = (int)(brush.Color.A / 255d * 100);
			ColorOpacity = $"{100-p}% Transparent";
		}
	}

	Border? containerBorder;
	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		containerBorder = e.NameScope.Find<Border>("PART_Container");
	}

	bool ignoreNextMouseOut = false;
	protected override void OnPointerEntered(PointerEventArgs e)
	{
		if (containerBorder != null)
		{
			ignoreNextMouseOut = true;
			FlyoutBase.ShowAttachedFlyout(containerBorder);
		}
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		if(!IsPointerOver && containerBorder != null)
			FlyoutBase.GetAttachedFlyout(containerBorder)?.Hide();

		if (ignoreNextMouseOut)
			ignoreNextMouseOut = false;
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		if (e.Pointer.IsPrimary)
			TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(ResourceKey);
	}
}
