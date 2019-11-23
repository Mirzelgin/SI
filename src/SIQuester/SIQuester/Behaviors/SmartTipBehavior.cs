﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Media;

namespace SIQuester.Behaviors
{
    /// <summary>
    /// Компонент, обеспечивающий появление всплывающих подсказок у TextBlock, текст которых выводится на экран не полностью
    /// </summary>
    public static class SmartTipBehavior
    {
        private static DependencyPropertyDescriptor TextDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        private static DependencyPropertyDescriptor WidthDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.ActualWidthProperty, typeof(TextBlock));
        
        public static bool GetIsAttached(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAttachedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsWatching.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAttachedProperty =
            DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(SmartTipBehavior), new UIPropertyMetadata(false, OnIsAttachedChanged));

        public static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;

            if (textBlock == null)
                return;

            if ((bool)e.NewValue)
            {
                AddHandlers(textBlock);
                SetToolTip(textBlock, EventArgs.Empty);
            }
            else
            {
                RemoveHandlers(textBlock);
            }
        }

        private static void RemoveHandlers(TextBlock textBlock)
        {
            TextDescriptor.RemoveValueChanged(textBlock, SetToolTip);
            WidthDescriptor.AddValueChanged(textBlock, SetToolTip);
        }

        private static void SetToolTip(object sender, EventArgs e)
        {
            var textBlock = sender as TextBlock;
            var ft = new FormattedText(textBlock.Text, CultureInfo.CurrentUICulture, textBlock.FlowDirection, new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize, textBlock.Foreground) { TextAlignment = textBlock.TextAlignment, Trimming = TextTrimming.None, LineHeight = textBlock.LineHeight };
            
            var showTooltip = ft.WidthIncludingTrailingWhitespace > (textBlock.ActualWidth - textBlock.Padding.Left - textBlock.Padding.Right);
            textBlock.ToolTip = showTooltip ? textBlock.Text : null;
        }

        private static void AddHandlers(TextBlock textBlock)
        {
            TextDescriptor.AddValueChanged(textBlock, SetToolTip);
            WidthDescriptor.AddValueChanged(textBlock, SetToolTip);
        }
    }
}