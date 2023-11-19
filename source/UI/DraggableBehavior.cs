namespace GeNSIS.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class DraggableBehavior
    {
        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.RegisterAttached("IsDraggable", typeof(bool), typeof(DraggableBehavior), new PropertyMetadata(false, OnIsDraggableChanged));

        public static bool GetIsDraggable(UIElement element)
        {
            return (bool)element.GetValue(IsDraggableProperty);
        }

        public static void SetIsDraggable(UIElement element, bool value)
        {
            element.SetValue(IsDraggableProperty, value);
        }

        private static void OnIsDraggableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                if ((bool)e.NewValue)
                {
                    uiElement.MouseLeftButtonDown += UIElement_MouseLeftButtonDown;
                    uiElement.MouseMove += UIElement_MouseMove;
                    uiElement.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
                }
                else
                {
                    uiElement.MouseLeftButtonDown -= UIElement_MouseLeftButtonDown;
                    uiElement.MouseMove -= UIElement_MouseMove;
                    uiElement.MouseLeftButtonUp -= UIElement_MouseLeftButtonUp;
                }
            }
        }

        private static bool isDragging = false;
        private static Point startPoint;

        private static void UIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement element)
            {
                startPoint = e.GetPosition(element);
                isDragging = true;
                element.CaptureMouse();
            }
        }

        private static void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(sender as Control, (sender as SettingPreviewUI).Clone(), DragDropEffects.Move);
        }

        private static void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (sender is UIElement element)
            {
                element.ReleaseMouseCapture();
            }
        }
    }
}
