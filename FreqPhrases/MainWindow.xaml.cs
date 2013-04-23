using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FreqPhrases
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListBox[] lbxArray;

        public MainWindow()
        {
            InitializeComponent();
            lbxArray = new ListBox[] { kitchenLbx, livingRoomLbx, bedroomLbx, outdoorLbx, saunaLbx, commonLbx, equipmentLbx };
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var lb = (((sender as TextBox).Parent) as ListBoxItem).Parent as ListBox;
            if ((e.Key == Key.Enter)
                || (e.Key == Key.Return))
            {
                var lbi = new ListBoxItem();
                lbi.Content = (sender as TextBox).Text;
                lb.Items.Insert(lb.Items.Count - 1, lbi);
                (sender as TextBox).Text = "";
            }
            lb.SelectedIndex = -1;
        }

        private void lbx_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var lbx = sender as ListBox;
                if ((lbx.SelectedIndex != -1)
                    && (lbx.SelectedIndex != lbx.Items.Count - 1))
                {
                    lbx.Items.RemoveAt(lbx.SelectedIndex);
                }
            }
        }

        private void lbx_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var lbx = sender as ListBox;
            if ((lbx.SelectedIndex != -1)
                && (lbx.SelectedIndex != lbx.Items.Count - 1)
                && (lbx.SelectedItem != null))
            {
                var toInsert = (lbx.SelectedItem as ListBoxItem).Content + ", ";
                var selStart = resultTbx.SelectionStart;
                resultTbx.Text = resultTbx.Text.Insert(selStart, toInsert);                
                resultTbx.SelectionStart = selStart + toInsert.Length;
            }
            resultTbx.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var doc = new XmlDocument();
            XmlElement mainNode = doc.CreateElement("settings");
            foreach (var lbx in lbxArray)
            {
                XmlNode node = doc.CreateNode(XmlNodeType.Element, lbx.Name, null);
                foreach (ListBoxItem lbi in lbx.Items)
                {
                    string add = "";
                    if (lbx.Items.IndexOf(lbi) != lbx.Items.Count - 2)
                        add = "\t";
                    if (lbx.Items.IndexOf(lbi) != lbx.Items.Count - 1)
                    {
                        //if (node.Value == null)
                        //    node.Value = "";
                        node.InnerText += lbi.Content as string + add;
                    }
                }
                mainNode.AppendChild(node);
            }
            doc.AppendChild(mainNode);
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = ("    ");
            XmlWriter writer = XmlWriter.Create("settings.xml", xmlSettings);
            doc.Save(writer);
            writer.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("settings.xml");
            var mainNode = doc.LastChild;
            foreach (XmlNode node in mainNode.ChildNodes)
            {
                var strs = node.InnerText.Split('\t');
                var lbx = GetListBoxByName(node.Name);
                foreach (var str in strs)
                {
                    var lbi = new ListBoxItem();
                    lbi.Content = str;
                    lbx.Items.Insert(lbx.Items.Count - 1, lbi);
                }
            }
        }

        private ListBox GetListBoxByName(string name)
        {
            foreach (var lbx in lbxArray)
            {
                if (lbx.Name == name)
                    return lbx;
            }
            return null;
        }
    }
}
