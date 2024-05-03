using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            students = new List<Student>();
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
            int index = System.IO.Path.GetFileName(excelFilePath).Length - 4;
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string userJsonText = JsonSerializer.Serialize<List<Student>>(students, options);
            string jsonFilePath = System.IO.Path.Combine(JsonFilesPath, $"{System.IO.Path.GetFileName(excelFilePath).Substring(0,index)}.json");
            File.WriteAllText(jsonFilePath, userJsonText);
            if (!CoursesBox.Items.Contains(System.IO.Path.GetFileName(jsonFilePath)))
            {
                CoursesBox.Items.Add(System.IO.Path.GetFileName(jsonFilePath));
            }
            // Code to load data from the Excel file
            // Update the AverageGradeTextBox, StudentsListView, and TaskGradesListView

            // Example data for demonstration purposes
            /*students = new List<Student>
            {
                new Student { Name = "John Doe", Tasks = new List<Task> { new Task { TaskName = "Task 1", Grade = 80 }, new Task { TaskName = "Task 2", Grade = 90 } } },
                new Student { Name = "Jane Smith", Tasks = new List<Task> { new Task { TaskName = "Task 1", Grade = 75 }, new Task { TaskName = "Task 2", Grade = 85 } } }
            };*/
            StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
            StudentsListView.ItemsSource = students;
            AverageGradeTextBox.Text = "Average Grade: " + CalcAverage(students).ToString("0.##");
        }

        private double FinalGrade(Student student)
        {
            double sum = 0;
            foreach (var col  in student.Details)
            {
                if (col.ColumnName.Contains("%"))
                {

                    string perc = col.ColumnName.Substring(col.ColumnName.Length - 3);
                    double percentValue = ConvertToDecimalPercentage(perc);
                    if (col.Detail == "") 
                    { col.Detail = "0"; }
                    sum += Int32.Parse(col.Detail) * percentValue;


                }
                

            }
          return sum;
            

        }

        private double CalcAverage(List<Student> students)
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
                // Update other TextBoxes with the details of the selected student
            }
        }

        private void AverageGradeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListBoxProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void PutCourseOnView(string excelFilePath)
        {

            int index = System.IO.Path.GetFileName(excelFilePath).Length - 5;
            if (!CoursesBox.Items.Contains(System.IO.Path.GetFileName(excelFilePath).Substring(0, index)))
            {
                CoursesBox.Items.Add(System.IO.Path.GetFileName(excelFilePath).Substring(0, index));
            }
            
            string filename = System.IO.Path.GetFileName(excelFilePath);
            string currDir = Directory.GetCurrentDirectory();
            string JsonFilesPath = System.IO.Path.Combine(currDir, "JsonFiles");
            string text = File.ReadAllText($"{JsonFilesPath}/{filename.Substring(0, index)}");
            List<Student> students1 = JsonSerializer.Deserialize<List<Student>>(text);
            StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
            StudentsListView.ItemsSource = students1;
            AverageGradeTextBox.Text = "Average Grade: " + CalcAverage(students1).ToString("0.##");








        }
        private void CoursesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object item = CoursesBox.SelectedItem;
            if (CoursesBox.SelectedIndex != 0)
            {
                string filename=CoursesBox.SelectedValue.ToString();
                PutCourseOnView($"{JsonFilesPath}/{filename}.json");
                if (StudentDetailsList != null)
                {
                    StudentDetailsList.Clear();
                }
            }
            else
            {
               if(AverageGradeTextBox!=null)
                {
                    AverageGradeTextBox.Clear();
                }
               if(ExcelPathTextBox!=null)
                {
                    ExcelPathTextBox.Clear();
                }
               if(StudentsListView!=null)
                {
                    StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
                    StudentsListView.ItemsSource = null;
                }
                if (StudentDetailsList != null)
                {
                    StudentDetailsList.Clear();
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

            stringBuilder.Append(Name);
            stringBuilder.Append(" : \n");

            // Iterate through the Details list and append each detail to the StringBuilder
            foreach (var detail in Details)
            {
                stringBuilder.Append(detail.ColumnName);
                stringBuilder.Append(" - ");
                stringBuilder.Append(detail.Detail);
                stringBuilder.Append(", \n");
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

    public class ReadAndParseJsonFileWithNewtonsoftJson
    {
        private readonly string _sampleJsonFilePath;

        public ReadAndParseJsonFileWithNewtonsoftJson(string sampleJsonFilePath)
        {
            _sampleJsonFilePath = sampleJsonFilePath;
        }
    }
}