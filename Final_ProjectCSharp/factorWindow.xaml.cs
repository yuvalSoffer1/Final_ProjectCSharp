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
using System.Text.Json;

namespace Final_ProjectCSharp
{
    /// <summary>
    /// Interaction logic for factorWindow.xaml
    /// </summary>
    public partial class factorWindow : Window
    {
        private string JsonFilesPath = "";
        private string currentJsonFilesPath = "";
        
        public factorWindow(string JsonFilesPath, string CurrentJsonFilesPath)
        {
            InitializeComponent();
            this.JsonFilesPath = JsonFilesPath;
            currentJsonFilesPath = CurrentJsonFilesPath;
            TasksShow();
        }

        private void AddFactorBtn_Click(object sender, RoutedEventArgs e)
        {
            string task = AssignmentListBox.SelectedItem.ToString();
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
                        int index = System.IO.Path.GetFileNameWithoutExtension(currentJsonFilesPath).IndexOf("+");
                        string sub = System.IO.Path.GetFileNameWithoutExtension(currentJsonFilesPath).Substring(0, index);
                        currentJsonFilesPath = $"{JsonFilesPath}/{sub}+{DateTime.Today.ToString("dd-MM-yyyy")}";
                        File.WriteAllText($"{currentJsonFilesPath}.json", modifiedJson);
                        MessageBox.Show($"Students in Course - {sub}\ngot {factor} factor on task '{task}' ");
                    }
                    else
                    {
                        MessageBox.Show("Invalid factor!");
                    }
                }
            }
        }
        private void TasksShow()
        {
            var newProp = new List<String>();
            if (!currentJsonFilesPath.Contains(".json"))
            {
                currentJsonFilesPath = $"{currentJsonFilesPath}.json";
            }
            string text = File.ReadAllText($"{currentJsonFilesPath}");
            List<Student> studentsFromJson = JsonSerializer.Deserialize<List<Student>>(text);
            foreach(var student in studentsFromJson)
            {
                foreach(var info in student.Details)
                {
                    if (info.ColumnName.Contains("%"))
                    {
                        newProp.Add(info.ColumnName);
                    }
                }
                break;
            }
            AssignmentListBox.ItemsSource = newProp;

        }
    }
    
}
