﻿using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;



namespace Final_ProjectCSharp
{
    public partial class MainWindow : Window
    {
        private List<Student> students = new List<Student>();
        private string JsonFilesPath;
        private string CurrentJsonFilesPath = "";
        private string excelFilepath;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "course avarge";
            // Creates a directory that will contain all the JSON files
            string currDir = Directory.GetCurrentDirectory();
            JsonFilesPath = System.IO.Path.Combine(currDir, "JsonFiles");
            if (!Directory.Exists(JsonFilesPath))
            {
                Directory.CreateDirectory(JsonFilesPath);
            }
            if (Directory.Exists(JsonFilesPath))
            {
                // Get all JSON files from the directory
                string[] jsonFiles = Directory.GetFiles(JsonFilesPath, "*.json");

                // Extract filenames without extensions
                string[] fileNames = new string[jsonFiles.Length];
                for (int i = 0; i < jsonFiles.Length; i++)
                {
                    int index = System.IO.Path.GetFileNameWithoutExtension(jsonFiles[i]).IndexOf("+");
                    fileNames[i] = System.IO.Path.GetFileNameWithoutExtension(jsonFiles[i]).Substring(0,index);
                }

                // Populate the ComboBox with the filenames
                foreach (string fileName in fileNames)
                {
                    CoursesBox.Items.Add(fileName);
                }
            }
        }

        #region buttons

        //load excel file and create a json file assccoited 
        private void LoadExcelData(string excelFilePath)
        {
            try
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
                        string detail;
                        if (csv[i][j] == String.Empty)
                        {
                            detail = "0";
                        }
                        else
                        {
                            detail = csv[i][j];
                        }
                        objResult.Details.Add(new Details
                        {
                            ColumnName = properties[j],
                            Detail = detail
                        });
                    }
                    students.Add(objResult);
                }
                students.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase));
                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                string userJsonText = JsonSerializer.Serialize<List<Student>>(students, options);
                string jsonFilePath = System.IO.Path.Combine(JsonFilesPath, $"{System.IO.Path.GetFileNameWithoutExtension(excelFilePath)}+{DateTime.Today.ToString("dd-MM-yyyy")}.json");
                int index = System.IO.Path.GetFileNameWithoutExtension(jsonFilePath).IndexOf("+");
                string sub = System.IO.Path.GetFileNameWithoutExtension(jsonFilePath).Substring(0, index);
                CurrentJsonFilesPath = $"{jsonFilePath}";
                File.WriteAllText(jsonFilePath, userJsonText);
                if (!CoursesBox.Items.Contains(sub))
                {
                    CoursesBox.Items.Add(sub);
                }
                StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
                StudentsListView.ItemsSource = students;
                AverageGradeTextBox.Text = $"{sub} - Average Grade: " + CalcAverage(students).ToString("0.##");
                Grade.Items.Clear();
                StudentFinalGrade.Content = "Final Grade:";
                this.Title = $"{sub}";
                StudentDetailsList.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't load this csv!");
            }
        }


        //open window to browse csv file to load
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files (*.csv)|*.csv";

            if (fileDialog.ShowDialog() == true)
            {
                ExcelPathTextBox.Text = fileDialog.FileName;
                excelFilepath = fileDialog.FileName;
                LoadExcelData(fileDialog.FileName);
            }
        }

        //open window for adding factor
        private void FactorBtn_Click(object sender, RoutedEventArgs e)
        {
            string filename = CoursesBox.SelectedValue.ToString();

            Object item = CoursesBox.SelectedItem;
            if (CoursesBox.SelectedIndex != 0)
            {
                factorWindow factorwindow = new factorWindow(JsonFilesPath, CurrentJsonFilesPath);
                factorwindow.Closed += factorwindow_OnClosed; // Subscribe to the Closed event
                factorwindow.Show();
                if (AverageGradeTextBox != null)
                {
                    AverageGradeTextBox.Clear();
                }
                if (ExcelPathTextBox != null)
                {
                    ExcelPathTextBox.Clear();
                }
                if (StudentsListView != null)
                {
                    StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
                    StudentsListView.ItemsSource = null;
                }
                if (StudentDetailsList != null)
                {
                    StudentDetailsList.Clear();
                }
                if (Grade != null)
                {
                    Grade.Items.Clear();
                    StudentFinalGrade.Content = "Final Grade:";
                }
                PutCourseOnView($"{CurrentJsonFilesPath}.json");
            }
        }

        //save changes in grade for selected student
        private void SaveGradesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsListView.SelectedItem != null)
            {
                // Retrieve the selected student
                Student selectedStudent = (Student)StudentsListView.SelectedItem;

                if (!CurrentJsonFilesPath.Contains(".json"))
                {
                    CurrentJsonFilesPath = $"{CurrentJsonFilesPath}.json";
                }
                string text = File.ReadAllText($"{CurrentJsonFilesPath}");
                List<Student> studentsFromJson = JsonSerializer.Deserialize<List<Student>>(text);
                foreach (var item in Grade.Items)
                {
                    var detail = item as Details;
                    if (detail != null)
                    {
                        foreach (var student in studentsFromJson)
                        {
                            if (student.Name == selectedStudent.Name)
                            {
                                foreach (var info in student.Details)
                                {
                                    if (info.ColumnName == detail.ColumnName)
                                    {
                                        var isNumber = double.TryParse(detail.Detail, out double score);
                                        if (detail.Detail == String.Empty)
                                        {
                                            info.Detail = "0";
                                        }
                                        else
                                        {
                                            if (isNumber)
                                            {
                                                var grade = double.Parse(detail.Detail);
                                                if (grade >= 0 && grade <= 100)
                                                {
                                                    info.Detail = detail.Detail;
                                                }
                                                else
                                                {
                                                    detail.Detail = info.Detail;
                                                    MessageBox.Show("Invalid grade!");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                students = studentsFromJson;
                string modifiedJson = JsonSerializer.Serialize(studentsFromJson);
                int index = System.IO.Path.GetFileNameWithoutExtension(CurrentJsonFilesPath).IndexOf("+");
                string sub = System.IO.Path.GetFileNameWithoutExtension(CurrentJsonFilesPath).Substring(0, index);
                CurrentJsonFilesPath = $"{JsonFilesPath}/{sub}+{DateTime.Today.ToString("dd-MM-yyyy")}";
                File.WriteAllText($"{CurrentJsonFilesPath}.json", modifiedJson);
                StudentFinalGrade.Content = $"Final Grade: {FinalGrade(selectedStudent).ToString("0.##")}";
                AverageGradeTextBox.Text = $"{sub} - Average Grade: " + CalcAverage(students).ToString("0.##");
            }
        }

        //load selected student from the list by click
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
                StudentFinalGrade.Content = $"Final Grade: {FinalGrade(selectedStudent).ToString("0.##")}";
            }
        }

        //load selected course from json file
        private void CoursesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object item = CoursesBox.SelectedItem;
            if (CoursesBox.SelectedIndex != 0)
            {
                string filename = CoursesBox.SelectedValue.ToString();
                if (!filename.Contains(".json"))
                {
                    string[] jsonFiles = Directory.GetFiles(JsonFilesPath, "*.json");
                    foreach (string jsonFile in jsonFiles)
                    {
                        if (jsonFile.Contains(filename))
                        {
                            filename = jsonFile;
                        }
                    }

                }
                PutCourseOnView($"{JsonFilesPath}/{filename}");
                if (StudentDetailsList != null)
                {
                    StudentDetailsList.Clear();
                }
            }
            else
            {
                if (AverageGradeTextBox != null)
                {
                    AverageGradeTextBox.Clear();
                }
                if (ExcelPathTextBox != null)
                {
                    ExcelPathTextBox.Clear();
                }
                if (StudentsListView != null)
                {
                    StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
                    StudentsListView.ItemsSource = null;
                }
                if (StudentDetailsList != null)
                {
                    StudentDetailsList.Clear();
                }
                if (Grade != null)
                {
                    Grade.Items.Clear();
                    StudentFinalGrade.Content = "Final Grade:";
                }
                this.Title = "course avarge";

            }
        }
        #endregion

        #region viewOnChange

        //calculate final grade for provided student
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
                    sum += double.Parse(col.Detail) * percentValue;
                }
            }
          return sum;
        }

        //calculate the average grade for a course
        private double CalcAverage(List<Student> students)
        {
            double average = 0;

            foreach (Student student in students)
            {
                average += FinalGrade(student);
            }
            return average / students.Count();
        }

        //convert string with % to double
        static double ConvertToDecimalPercentage(string percentString)
        {
            // Remove the "%" symbol
            string numericPart = percentString.TrimEnd('%');

            // Convert to a double and divide by 100 to get the decimal equivalent
            double percentValue = double.Parse(numericPart) / 100.0;

            return percentValue;
        }

        //helper func for CoursesBox_SelectionChanged
        public void PutCourseOnView(string jsonPath)
        {

            int index = System.IO.Path.GetFileNameWithoutExtension(jsonPath).IndexOf("+");
            string sub = System.IO.Path.GetFileNameWithoutExtension(jsonPath).Substring(0,index);
            if (!CoursesBox.Items.Contains(sub))
            {
                CoursesBox.Items.Add(sub);
            }
            
            string filename = System.IO.Path.GetFileNameWithoutExtension(jsonPath);
            string text = File.ReadAllText($"{JsonFilesPath}/{filename}.json");
            CurrentJsonFilesPath = $"{JsonFilesPath}/{filename}";
            List<Student> students1 = JsonSerializer.Deserialize<List<Student>>(text);
            students1.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase));
            StudentsListView.ClearValue(ItemsControl.ItemsSourceProperty);
            StudentsListView.ItemsSource = students1;
            AverageGradeTextBox.Text = $"{sub} - Average Grade: " + CalcAverage(students1).ToString("0.##");
            this.Title = sub;
            if (Grade != null)
            {
                Grade.Items.Clear();
                StudentFinalGrade.Content = "Final Grade:";
            }

        }
        
        //listner for closing factor window
        private void factorwindow_OnClosed(object sender, EventArgs e)
        {
            // Call the function in the MainWindow
            PutCourseOnView(CurrentJsonFilesPath);
        }
        #endregion

    }
}