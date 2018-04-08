using System;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataSource.Users.Instance.SessionUser != null &&
                DataSource.Users.Instance.SessionUser.GroupUsergroup == User.Usergroup.Admin)
                NewCourseButton.Visibility = Visibility.Visible;
        }

        private async void AddCourse()
        {
            var newCourse = new Course();
            AddCourseContentDialog.DataContext = newCourse;

            var result = await AddCourseContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    newCourse.Users = new List<User>{ DataSource.Users.Instance.SessionUser };
                    Course addedCourse;
                    if ((addedCourse = await DataSource.Courses.Instance.AddCourse(newCourse)) != null)
                    {
                        ViewModel.Courses.Add(addedCourse);
                    }
                }
                catch (Exception)
                {
                    // TODO: Exception handling.
                }
            }
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
