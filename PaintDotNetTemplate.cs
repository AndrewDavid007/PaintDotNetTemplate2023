using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CheckboxControl = System.Boolean;
using DoubleSliderControl = System.Double;
using IntSliderControl = System.Int32;
using RadioButtonControl = System.Byte;

[assembly: AssemblyTitle("PaintDotNetTemplate plugin for Paint.NET")]
[assembly: AssemblyDescription("Built to create a template for VS2022")]
[assembly: AssemblyConfiguration("color|wheel|sat|greyscale")]
[assembly: AssemblyCompany("AndrewDavid")]
[assembly: AssemblyProduct("PaintDotNetTemplate")]
[assembly: AssemblyCopyright("Copyright ©2023 by AndrewDavid")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyMetadata("BuiltByCodeLab", "Version=6.10.8684.39035")]
[assembly: SupportedOSPlatform("Windows")]

namespace PaintDotNetTemplateEffect2023
{
    public class PaintDotNetTemplateSupportInfo : IPluginSupportInfo
    {
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new("https://jmbondrlc007.wixsite.com/andrewdavid");
    }

    [PluginSupportInfo<PaintDotNetTemplateSupportInfo>(DisplayName = "PaintDotNetTemplate")]
    public class PaintDotNetTemplateEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName => "PaintDotNetTemplate";
        public static Image StaticIcon => new Bitmap(typeof(PaintDotNetTemplateEffectPlugin), "PaintDotNetTemplate.png");
        public static string SubmenuName => "AndrewDavid VS2022";

        public Color ColorHsvColor96Float { get; private set; }

        public PaintDotNetTemplateEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, new EffectOptions { Flags = EffectFlags.Configurable })
        {
        }

        public enum PropertyNames
        {
            Amount1,
            Amount2,
            Amount3,
            Amount4,
            Amount5
        }

        public enum Amount4Options
        {
            Amount4Option1,
            Amount4Option2
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new()
            {
                new Int32Property(PropertyNames.Amount1, 255, 10, 255),
                new DoubleProperty(PropertyNames.Amount2, 0, 0, 360),
                new DoubleProperty(PropertyNames.Amount3, 0, 0, 360),
                StaticListChoiceProperty.CreateForEnum<Amount4Options>(PropertyNames.Amount4, 0, false),
                new BooleanProperty(PropertyNames.Amount5, false)
            };

            return new PropertyCollection(props);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.DisplayName, "Radius");
            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.ShowHeaderLine, false);
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.DisplayName, "Rotate Clockwise");
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.SliderLargeChange, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.SliderSmallChange, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.UpDownIncrement, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.DecimalPlaces, 3);
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.ShowHeaderLine, false);
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.DisplayName, "Lighten Darken");
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.SliderLargeChange, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.SliderSmallChange, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.UpDownIncrement, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.DecimalPlaces, 3);
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.ShowHeaderLine, false);
            configUI.SetPropertyControlValue(PropertyNames.Amount4, ControlInfoPropertyNames.DisplayName, "Centre Color ");
            configUI.SetPropertyControlType(PropertyNames.Amount4, PropertyControlType.RadioButton);
            PropertyControlInfo Amount4Control = configUI.FindControlForPropertyName(PropertyNames.Amount4);
            Amount4Control.SetValueDisplayName(Amount4Options.Amount4Option1, "White");
            Amount4Control.SetValueDisplayName(Amount4Options.Amount4Option2, "Black");
            configUI.SetPropertyControlValue(PropertyNames.Amount4, ControlInfoPropertyNames.ShowHeaderLine, false);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.DisplayName, string.Empty);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.Description, "Show as Greyscale");
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.ShowHeaderLine, false);

            return configUI;
        }

        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)
        {
            // Change the effect's window title
            props[ControlInfoPropertyNames.WindowTitle].Value = "PaintDotNetTemplate";
            // Add help button to effect UI
            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.PlainText;
            props[ControlInfoPropertyNames.WindowHelpContent].Value = "PaintDotNetTemplate v1.0\nCopyright ©2023 by AndrewDavid\nAll rights reserved.";
            base.OnCustomizeConfigUIWindowProperties(props);
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken token, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Amount1 = token.GetProperty<Int32Property>(PropertyNames.Amount1).Value;
            Amount2 = token.GetProperty<DoubleProperty>(PropertyNames.Amount2).Value;
            Amount3 = token.GetProperty<DoubleProperty>(PropertyNames.Amount3).Value;
            Amount4 = (byte)(int)token.GetProperty<StaticListChoiceProperty>(PropertyNames.Amount4).Value;
            Amount5 = token.GetProperty<BooleanProperty>(PropertyNames.Amount5).Value;

            PreRender(dstArgs.Surface, srcArgs.Surface);

            base.OnSetRenderInfo(token, dstArgs, srcArgs);
        }

        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            if (length == 0) return;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Render(DstArgs.Surface, SrcArgs.Surface, rois[i]);
            }
        }

        #region User Entered Code
        // Name: PaintDotNetTemplate
        // Submenu: AndrewDavid VS2022
        // Author: AndrewDavid
        // Title:PaintDotNetTemplate
        // Version: 1.0
        // Desc:
        // KeyWords: color|wheel|sat|greyscale
        // URL:https://jmbondrlc007.wixsite.com/andrewdavid
        // Help:https://boltbait.com/pdn/codelab/


        #region UICode
        IntSliderControl Amount1 = 255; // [10,255] Radius
        DoubleSliderControl Amount2 = 0; // [0,360] Rotate Clockwise
        DoubleSliderControl Amount3 = 0; // [0,360] Lighten Darken
        RadioButtonControl Amount4 = 0; // Centre Color | White | Black
        CheckboxControl Amount5 = false; // Show as Greyscale
        #endregion

        // Setup for using a specific blend op
        private readonly UnaryPixelOps.Desaturate desaturateOp = new();
        private readonly object hsvColor;

        // This single-threaded function is called after the UI changes and before the Render function is called
        // The purpose is to prepare anything you'll need in the Render function
        static void PreRender(Surface dst, Surface src)
        {
        }

        // Here is the main multi-threaded render function
        // The dst canvas is broken up into rectangles and
        // your job is to write to each pixel of that rectangle

        void Render(Surface dst, Surface src, Rectangle rect)
        {
            // Delete any of these lines you don't need
            Rectangle selection = EnvironmentParameters.SelectionBounds;
            int cx = (int)(((selection.Right - selection.Left) / 2) + selection.Left);
            int cy = (int)(((selection.Bottom - selection.Top) / 2) + selection.Top);
            int h, s, v = 0;

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    int dx = (x - cx);
                    int dy = (y - cy);

                    double alpha = Math.Sqrt((dx * dx) + (dy * dy));
                    // alpha = radius
                    if (alpha > Amount1)    // outside radius = transparent pixel
                    {
                        dst[x, y] = ColorBgra.FromBgra(0, 0, 0, 0);
                    }
                    else    // on or inside radius = color it in
                    {
                        double theta = Math.Atan2(dy, dx);
                        if (theta < 0)
                        {
                            theta += 2 * Math.PI;
                        }
                        h = (int)(((theta / (Math.PI * 2)) * 360.0) - Amount2);
                        if (h < 0)
                        {
                            h += 360;
                        }
                        if (Amount4 == 0) // Lighten
                        {
                            s = (int)(((360 - Amount3) * (alpha / Amount1) * 100 / 360) + (Amount3 * 100 / 360));
                            v = (int)((Amount3 * (alpha / Amount1) * 100 / 360) + (360 - Amount3) * 100 / 360);
                        }
                        else    // Darken 
                        {
                            s = 100;
                            v = (int)((alpha / Amount1) * 100);
                        }
                        ColorHsvColor96Float = new();
                        if (Amount5)    // Greyscale
                        {
                            dst[x, y] = desaturateOp.Apply(ColorBgra.FromColor(ColorHsvColor96Float));
                        }
                        else
                        {
                            dst[x, y] = ColorBgra.FromColor(ColorHsvColor96Float);
                        }
                    }
                }
            }

            #endregion
        }
    }
}
