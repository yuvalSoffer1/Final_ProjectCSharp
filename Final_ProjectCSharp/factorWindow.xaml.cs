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
using Final_Project;
using System.Text.Json;

namespace Final_ProjectCSharp
{
    /// <summary>
    /// Interaction logic for factorWindow.xaml
    /// </summary>
    public partial class factorWindow : Window
    {
        private string currentJsonFilesPath = "";
        public factorWindow(string excelFilePath,string CurrentJsonFilesPath)
        {
            InitializeComponent();
            currentJsonFilesPath = CurrentJsonFilesPath;
            TasksShow(excelFilePath);
        }

        private void AddFactorBtn_Click(object sender, RoutedEventArgs e)
        {
            string task=AssignmentListBox.SelectedItem.ToString();
            string text = File.ReadAllText($"{currentJsonFilesPath}");
            List<Student> studentsFromJson = JsonSerializer.Deserialize<List<Student>>(text);
            string factor = FactorValue.Text;
            var isNumber = double.TryParse(factor, out double score);
            if (factor == String.Empty)
            {
                factor = "0";
            }
            else
            {
                if (isNumber)
                {
                    var Factor = double.Parse(factor);
                    if (Factor >= 0 && Factor <= 100)
                    {
                        foreach (var student in studentsFromJson)
                        {
                            foreach (var info in student.Details)
                            {
                                if (info.ColumnName == task)
                                {
                                    double newgrade = double.Parse(info.Detail) + Factor;
                                    if (newgrade > 100){ newgrade = 100; }
                                    else if (newgrade < 0) {  newgrade = 0; }
                                    info.Detail = newgrade.ToString();
                                }
                            }
                        }
                        string modifiedJson = JsonSerializer.Serialize(studentsFromJson);
                        File.WriteAllText($"{currentJsonFilesPath}", modifiedJson);
                        
                    }
                    else
                    {
                        
                        MessageBox.Show("Invalid factor!");
                    }
                }
            }
            


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
