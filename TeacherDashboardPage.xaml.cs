using MyMauiApp.Models;
using MyMauiApp.Services;
using System.Collections.ObjectModel;

namespace MyMauiApp
{
    public partial class TeacherDashboardPage : ContentPage
    {
        public ObservableCollection<Course> TeacherCourses { get; } = new();

        public TeacherDashboardPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private void LoadData()
        {
            var data = AppData.Instance;
            var current = data.CurrentUser;

            if (current == null || current.Role != UserRole.Teacher)
                return;

            // имя в белой таблетке
            TeacherNameLabel.Text = current.Name;

            // подгружаем курсы
            TeacherCourses.Clear();
            var courses = data.GetCoursesForTeacher(current.Id);

            foreach (var c in courses)
                TeacherCourses.Add(c);

            // счётчик курсов
            var count = TeacherCourses.Count;
            CoursesCountLabel.Text = count switch
            {
                1 => "Zarządzasz 1 kursem",
                2 or 3 or 4 => $"Zarządzasz {count} kursami",
                _ => $"Zarządzasz {count} kursów"
            };
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewCoursePage());
        }

        private async void OnAddMaterialClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Course course)
            {
                await Navigation.PushAsync(new AddMaterialPage(course.Id));
            }
        }

        private async void OnStudentsProgressClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Course course)
            {
                await Navigation.PushAsync(new StudentsProgressPage(course.Id));
            }
        }

        // Обработчик для удаления курса
        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Course course)
            {
                var result = await DisplayAlert("Удалить курс", "Вы уверены, что хотите удалить этот курс?", "Да", "Нет");
                if (result)
                {
                    // Удаление курса
                    AppData.Instance.DeleteCourse(course.Id);

                    // Обновление списка курсов
                    LoadData();
                }
            }
        }
    }
}