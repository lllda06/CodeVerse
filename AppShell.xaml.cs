namespace MyMauiApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
        Routing.RegisterRoute(nameof(TeacherLoginPage), typeof(TeacherLoginPage));
        Routing.RegisterRoute(nameof(TeacherRegisterPage), typeof(TeacherRegisterPage));
        
        
        Routing.RegisterRoute(nameof(TeacherDashboardPage), typeof(TeacherDashboardPage));
        Routing.RegisterRoute(nameof(NewCoursePage), typeof(NewCoursePage));
        Routing.RegisterRoute(nameof(AddMaterialPage), typeof(AddMaterialPage));
        Routing.RegisterRoute(nameof(StudentsProgressPage), typeof(StudentsProgressPage));
        
        Routing.RegisterRoute(nameof(StudentLoginPage), typeof(StudentLoginPage));
        Routing.RegisterRoute(nameof(StudentRegisterPage), typeof(StudentRegisterPage));
        Routing.RegisterRoute(nameof(StudentDashboardPage), typeof(StudentDashboardPage));
        Routing.RegisterRoute(nameof(StudentCoursePage), typeof(StudentCoursePage));
    }
}
