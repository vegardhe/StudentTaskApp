﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoursesPage : Page
    {

        public ObservableCollection<Resource> CourseResources { get; set; }

        public ObservableCollection<Exercise> CourseExercises { get; set; }

        public CoursesPage()
        {
            this.InitializeComponent();
            CourseResources = new ObservableCollection<Resource>();
            CourseExercises = new ObservableCollection<Exercise>();
        }

        private async void CoursesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = (Course) CoursesListView.SelectedItem;
            if (selectedCourse != null)
            {
                if (selectedCourse.Resources == null)
                {
                    await DataSource.Courses.Instance.GetCourseResources(selectedCourse);
                }

                if (selectedCourse.Exercises == null)
                {
                    await DataSource.Courses.Instance.GetCourseExercises(selectedCourse);
                }
            }
            UpdateCourseResources(selectedCourse);
            UpdateCourseExercises(selectedCourse);
        }

        private void UpdateCourseResources(Course course)
        {
            CourseResources.Clear();
            foreach (var r in course.Resources)
            {
                CourseResources.Add(r);
            }
        }

        private void UpdateCourseExercises(Course course)
        {
            CourseExercises.Clear();
            foreach (var e in course.Exercises)
            {
                CourseExercises.Add(e);
            }
        }
    }
}
