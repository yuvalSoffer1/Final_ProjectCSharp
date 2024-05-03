using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Final_ProjectCSharp
{
    /// <summary>
    /// Interaction logic for factorWindow.xaml
    /// </summary>
    public partial class factorWindow : Window
    {
        public factorWindow(string excelFilePath)
        {
            InitializeComponent();
            TasksShow(excelFilePath);
        }
        private void AddFactorBtn_Click(object sender, RoutedEventArgs e)
        {
            string task=AssignmentListBox.SelectedItem.ToString();


        }
        private void TasksShow(string excelFilePath)

        {
            var lines = File.ReadAllLines(excelFilePath);
            var properties = lines[0].Split(',');
            var newProp = new List<String>();
            foreach ( String task in properties) 
            {
                if (task.Contains("%"))
                {
                    newProp.Add(task);

                }
                
            }
            AssignmentListBox.ItemsSource = newProp;

        }
    }
    
}
