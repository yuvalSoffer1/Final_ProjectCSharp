using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Final_Project
{
    public partial class MainWindow : Window
    {
        private List<Student> students = new List<Student>();
        private string JsonFilesPath;

        public MainWindow()
        {
            InitializeComponent();

            // Creates a directory that will contain all the JSON files
            string currDir = Directory.GetCurrentDirectory();
            JsonFilesPath = System.IO.Path.Combine(currDir, "JsonFiles");
            if (!Directory.Exists(JsonFilesPath))
            {
                Directory.CreateDirectory(JsonFilesPath);
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files (*.csv)|*.csv";

            if (fileDialog.ShowDialog() == true)
            {
                ExcelPathTextBox.Text = fileDialog.FileName;
                LoadExcelData(fileDialog.FileName);
            }
        }

        private void LoadExcelData(string excelFilePath)
        {
            var csv = new List<string[]>();
            var lines = File.ReadAllLines(excelFilePath);

            foreach (string line in lines)
                csv.Add(line.Split(','));

            var properties = lines[0].Split(',');


            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Student();
                objResult.Details = new List<Details>();
                for (int j = 1; j < properties.Length; j++)
                {
                    objResult.Name = csv[i][0];
                    objResult.Details.Add(new Details { ColumnName = properties[j], Detail = csv[i][j] });
                }
                students.Add(objResult);
            }

            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string userJsonText = JsonSerializer.Serialize<List<Student>>(students, options);
            string jsonFilePath = System.IO.Path.Combine(JsonFilesPath, $"{System.IO.Path.GetFileName(excelFilePath)}.json");
            File.WriteAllText(jsonFilePath, userJsonText);
            // Code to load data from the Excel file
            // Update the AverageGradeTextBox, StudentsListView, and TaskGradesListView

            // Example data for demonstration purposes
            /*students = new List<Student>
            {
                new Student { Name = "John Doe", Tasks = new List<Task> { new Task { TaskName = "Task 1", Grade = 80 }, new Task { TaskName = "Task 2", Grade = 90 } } },
                new Student { Name = "Jane Smith", Tasks = new List<Task> { new Task { TaskName = "Task 1", Grade = 75 }, new Task { TaskName = "Task 2", Grade = 85 } } }
            };*/

            StudentsListView.ItemsSource = students;
            AverageGradeTextBox.Text = "Average Grade: " + CalcAverage();
        }

        private double FinalGrade(Student student)
        {
            double sum = 0;
            int count = 0;
            student.Details.Any(x => {
                if (x.ColumnName.Contains("%"))
                {

                    string perc = x.ColumnName.Substring(x.ColumnName.Length - 3);
                    double percentValue = ConvertToDecimalPercentage(perc);
                    sum += Int32.Parse(x.Detail) * percentValue;
                    count++;

                }
                return true;

            });
            return sum / count;

        }

        private double CalcAverage()
        {
            double average = 0;

            foreach (Student student in students)
            {
                average += FinalGrade(student);
            }
            return average / students.Count();
        }

        static double ConvertToDecimalPercentage(string percentString)
        {
            // Remove the "%" symbol
            string numericPart = percentString.TrimEnd('%');

            // Convert to a double and divide by 100 to get the decimal equivalent
            double percentValue = double.Parse(numericPart) / 100.0;

            return percentValue;
        }

        private void StudentsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student selectedStudent = StudentsListView.SelectedItem as Student;

            /*if (selectedStudent != null)
            {
                StudentNameTextBox.Text = selectedStudent.Name;
                TaskGradesListView.ItemsSource = selectedStudent.Tasks;
                StudentDetailsWindow.Show();
            }*/
        }

        private void TaskGradesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsListView.SelectedItem != null)
            {
                // Retrieve the selected student
                Student selectedStudent = (Student)StudentsListView.SelectedItem;

                // Display the details of the selected student in the TextBoxes
                StudentDetailsList.Text = selectedStudent.ToString();

                //extract course and grade for each one.
                Grade.Items.Clear();
                foreach (var detail in selectedStudent.Details)
                {
                    if (detail.ColumnName.Contains("%"))
                    {
                        Grade.Items.Add(detail);
                    }
                }
            }
        }

        private void AverageGradeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListBoxProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Grade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveGradesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsListView.SelectedItem != null)
            {
                // Retrieve the selected student
                Student selectedStudent = (Student)StudentsListView.SelectedItem;

                int index = 0;
                foreach (var item in Grade.Items)
                {
                    var detail = item as Details; // Make sure to cast from object to Details
                    if (detail != null)
                    {
                        Console.WriteLine($"ColumnName: {detail.ColumnName}, Detail: {detail.Detail}");
                    }
                    /*if (item is StackPanel stackPanel)
                    {

                        foreach (var child in stackPanel.Children)
                        {
                            if (child is TextBox textBox)
                            {
                                if (textBox.Text == String.Empty)
                                {
                                    selectedStudent.Details.FirstOrDefault(x => {
                                        x.Detail = "0"
                                    });
                                    
                                    
                                    textBox.Text = "0";
                                }
                                else
                                {
                                    var isNumber = double.TryParse(textBox.Text, out double score);
                                    if (isNumber && score >= 0 && score <= 100)
                                    {
                                        s.Grades[index].Score = textBox.Text;
                                    }
                                    else
                                    {
                                        textBox.Text = s.Grades[index].Score;
                                        MessageBox.Show("Invalid grade!");
                                    }
                                }
                                ++index;
                            }
                        }
                    }*/
                }
            }
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public List<Details> Details { get; set; }

        public override string ToString()
        {
            if (Details == null || Details.Count == 0)
                return base.ToString();

            // Using StringBuilder for efficient string concatenation
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append("Name");
            stringBuilder.Append(" - ");
            stringBuilder.Append(Name);
            stringBuilder.Append(". \n");

            // Iterate through the Details list and append each detail to the StringBuilder
            foreach (var detail in Details)
            {
                if (!detail.ColumnName.Contains("%"))
                {
                    stringBuilder.Append(detail.ColumnName);
                    stringBuilder.Append(" - ");
                    stringBuilder.Append(detail.Detail);
                    stringBuilder.Append(". \n");
                }
            }

            // Remove the trailing newline character
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }
    }

    public class Details
    {
        public string ColumnName { get; set; }
        public string Detail { get; set; }

    }

}