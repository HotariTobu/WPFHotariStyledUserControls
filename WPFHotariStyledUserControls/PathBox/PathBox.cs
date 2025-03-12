using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WPFCustomControls;

namespace WPFHotariStyledUserControls.PathBox
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WPFCustomControls"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WPFCustomControls;assembly=WPFCustomControls"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:PathBox/>
    ///
    /// </summary>
    public class PathBox : Control
    {
        static PathBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathBox), new FrameworkPropertyMetadata(typeof(PathBox)));
        }

        #region == PathText ==

        public string PathText { get => GetValue(PathTextProperty) as string; set => SetValue(PathTextProperty, value); }
        public static readonly DependencyProperty PathTextProperty = DependencyProperty.Register("PathText", typeof(string), typeof(PathBox));

        #endregion

        #region == PathType ==

        public PathTypes? PathType { get => GetValue(PathTypeProperty) as PathTypes?; set => SetValue(PathTypeProperty, value); }
        public static readonly DependencyProperty PathTypeProperty = DependencyProperty.Register("PathType", typeof(PathTypes?), typeof(PathBox));

        #endregion

        #region == PathTextChanged ==

        public static readonly RoutedEvent PathTextChangedEvent = EventManager.RegisterRoutedEvent("PathTextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathBox));
        public event RoutedEventHandler PathTextChanged { add => AddHandler(PathTextChangedEvent, value); remove => RemoveHandler(PathTextChangedEvent, value); }
        void RaisePathTextChangedEvent() => RaiseEvent(new RoutedEventArgs(PathBox.PathTextChangedEvent));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("Border") is Border border)
            {
                border.PreviewDragOver += Border_PreviewDragOver;
                border.Drop += Border_Drop;
            }

            if (GetTemplateChild("Button") is Button button)
            {
                button.PreviewDragOver += Border_PreviewDragOver;
                button.Drop += Border_Drop;
                button.Click += Button_Click;
            }

            if (GetTemplateChild("TextBox") is TextBox textBox)
            {
                textBox.TextChanged += TextBox_TextChanged;
            }

            PathDialog = new PathDialog(PathDialog.Purposes.Load, PathType ?? PathTypes.FileAndDirectory, PathDialog.SelectionModes.Single);
        }

        private PathDialog PathDialog { get; set; }

        private void Border_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text))
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                bool isFile = File.Exists(path);
                bool isDirectory = Directory.Exists(path);

                if (PathType == PathTypes.FileOnly && isFile)
                {
                    PathText = path;
                }

                if (PathType == PathTypes.DirectoryOnly && isDirectory)
                {
                    PathText = path;
                }

                if (PathType == PathTypes.FileAndDirectory && (isFile || isDirectory))
                {
                    PathText = path;
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                InsertText(e.Data.GetData(DataFormats.UnicodeText) as string);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                InsertText(e.Data.GetData(DataFormats.Text) as string);
            }

            void InsertText(string text)
            {
                if (!string.IsNullOrWhiteSpace(text) && GetTemplateChild("TextBox") is TextBox textBox)
                {
                    textBox.SelectedText = text;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RaisePathTextChangedEvent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PathDialog.ShowDialog() == true)
            {
                throw new NotImplementedException();
            }
            /*if (FolderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathText = FolderDialog.FileName;
            }*/


        }
    }
}
