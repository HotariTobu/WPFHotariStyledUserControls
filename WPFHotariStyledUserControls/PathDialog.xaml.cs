using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace WPFCustomControls
{
    /// <summary>
    /// PathDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PathDialog : Window
    {
        public PathDialog(Purposes purpose = Purposes.Load, PathTypes pathType = PathTypes.FileAndDirectory, SelectionModes selectionMode = SelectionModes.Single, string caption = null, string currentDirectory = null)
        {
            InitializeComponent();

            Purpose = purpose;
            PathType = pathType;
            SelectionMode = selectionMode;

            if (string.IsNullOrWhiteSpace(caption))
            {
                switch (Purpose)
                {
                    case Purposes.Load:
                        Caption = Properties.Resources.Open;
                        break;
                    case Purposes.Save:
                        Caption = Properties.Resources.SaveAs;
                        break;
                }
            }
            else
            {
                Caption = caption;
            }

            if (string.IsNullOrWhiteSpace(currentDirectory) || !Directory.Exists(currentDirectory))
            {
                CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                CurrentDirectory = currentDirectory;
            }
        }

        #region == Purpose ==

        public Purposes? Purpose { get => GetValue(PurposeProperty) as Purposes?; set => SetValue(PurposeProperty, value); }
        public static readonly DependencyProperty PurposeProperty = DependencyProperty.Register("Purpose", typeof(Purposes?), typeof(PathDialog));

        #endregion

        #region == PathType ==

        public PathTypes? PathType { get => GetValue(PathTypeProperty) as PathTypes?; set => SetValue(PathTypeProperty, value); }
        public static readonly DependencyProperty PathTypeProperty = DependencyProperty.Register("PathType", typeof(PathTypes?), typeof(PathDialog));

        #endregion

        #region == SelectionMode ==

        public SelectionModes? SelectionMode { get => GetValue(SelectionModeProperty) as SelectionModes?; set => SetValue(SelectionModeProperty, value); }
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionModes?), typeof(PathDialog));

        #endregion

        #region == Caption ==

        public string Caption { get => GetValue(CaptionProperty) as string; set => SetValue(CaptionProperty, value); }
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(PathDialog), new PropertyMetadata(string.Empty));

        #endregion

        #region == CurrentDirectory ==

        public string CurrentDirectory { get => GetValue(CurrentDirectoryProperty) as string; set => SetValue(CurrentDirectoryProperty, value); }
        public static readonly DependencyProperty CurrentDirectoryProperty = DependencyProperty.Register("CurrentDirectory", typeof(string), typeof(PathDialog), new PropertyMetadata(string.Empty));

        #endregion

        #region == ListViewMode ==

        public ListViewModes? ListViewMode { get => GetValue(ListViewModeProperty) as ListViewModes?; set => SetValue(ListViewModeProperty, value); }
        public static readonly DependencyProperty ListViewModeProperty = DependencyProperty.Register("ListViewMode", typeof(ListViewModes?), typeof(PathDialog));

        #endregion

        #region == ButtonEvents ==

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateDirectoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private void Window_Deactivated(object sender, EventArgs e)
        {
            OKButton.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VisualStateManager.GoToState(PathBar, "Normal", true);

            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        public enum Purposes
        {
            Load,
            Save,
        }

        public enum SelectionModes
        {
            Single,
            Multiple,
        }

        public enum ListViewModes
        {
            
        }

    }

    #region == PathBar ==

    public class PathBar : Control
    {
        static PathBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathBar), new FrameworkPropertyMetadata(typeof(PathBar)));
        }

        #region == PathText ==

        public string PathText { get => GetValue(PathTextProperty) as string; set => SetValue(PathTextProperty, value); }
        public static readonly DependencyProperty PathTextProperty = DependencyProperty.Register("PathText", typeof(string), typeof(PathBar),
            new PropertyMetadata(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, (d, baseValue) =>
              {
                  string path = baseValue as string;
                  if (string.IsNullOrWhiteSpace(path))
                  {
                      return string.Empty;
                  }

                  if (File.Exists(path))
                  {
                      path = Directory.GetParent(path).FullName;
                  }

                  if (Directory.Exists(path))
                  {
                      path = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.TrimEndingDirectorySeparator(path) + Path.DirectorySeparatorChar));
                  }
                  else
                  {
                      return string.Empty;
                  }

                  return path;
              }), new ValidateValueCallback(value => File.Exists(value as string) || Directory.Exists(value as string)));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("Border") is Border border)
            {
                border.PreviewDragOver += Border_PreviewDragOver;
                border.Drop += Border_Drop;
            }

            if (GetTemplateChild("ScrollViewer") is ScrollViewer scrollViewer)
            {
                foreach (var resource in scrollViewer.Resources.Values)
                {
                    if (resource is Style style && style.TargetType == typeof(Button))
                    {
                        style.Setters.Add(new EventSetter(Button.ClickEvent, new RoutedEventHandler((sender, e) => PathText = (sender as Button)?.Tag as string)));
                        break;
                    }
                }
            }

            if (GetTemplateChild("ItemsControl") is ItemsControl itemsControl)
            {
                itemsControl.MouseDown += ItemsControl_MouseDown;
                itemsControl.MouseUp += ItemsControl_MouseUp;
            }

            if (GetTemplateChild("TextBox") is TextBox textBox)
            {
                textBox.IsVisibleChanged += TextBox_IsVisibleChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }

            if (GetTemplateChild("Button") is Button button)
            {
                button.GotFocus += Button_GotFocus;
                button.Click += Button_Click;
            }

            IsEditable = false;
        }

        private bool IsMouseHold { get; set; }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseHold = true;
        }

        private void ItemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseHold)
            {
                IsEditable = true;
                IsMouseHold = false;
            }
        }

        private void Border_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            PathText = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
        }

        private bool _IsEditable;
        private bool IsEditable
        {
            get => _IsEditable;
            set
            {
                if ((_IsEditable != value) && (_IsEditable = value))
                {
                    VisualStateManager.GoToState(this, "Editable", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }
        }

        private bool TextBoxLostFocused { get; set; }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Focus())
                {
                    textBox.SelectAll();
                }
            }
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxLostFocused)
            {
                IsEditable = true;
                TextBoxLostFocused = false;
            }
        }

        private void TextBox_LostFocus(object sender = null, RoutedEventArgs e = null)
        {
            IsEditable = false;
            TextBoxLostFocused = true;
        }

        private void Button_Click(object sender = null, RoutedEventArgs e = null)
        {
            IsEditable = !IsEditable;
        }
    }

    public class PathToButtonsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (path == null)
            {
                return null;
            }

            string tag = string.Empty;
            return path.Split(Path.DirectorySeparatorChar).Select(name =>
            {
                if (name.Length > 0)
                {
                    tag = Path.Combine(tag, name);
                    Button button = new Button();
                    button.Content = name;
                    button.Tag = tag;
                    return button;
                }
                else
                {
                    return null;
                }
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    #endregion
}
