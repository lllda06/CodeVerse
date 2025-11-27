using System;
using System.Collections.Generic;

namespace MyMauiApp.Models
{
    /// <summary>
    /// Роль пользователя в системе.
    /// </summary>
    public enum UserRole
    {
        Teacher,
        Student
    }

    /// <summary>
    /// Пользователь (учитель или ученик).
    /// </summary>
    public class User
    {
        /// <summary>
        /// Строковый Id (используем строку, чтобы было проще сериализовать и сравнивать).
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// Курс, которым владеет учитель.
    /// </summary>
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Language { get; set; } = "";
        public int LessonsCount { get; set; }

        /// <summary>
        /// Id учителя-владельца (User.Id).
        /// </summary>
        public string OwnerTeacherId { get; set; } = "";

        /// <summary>
        /// Материалы курса (лекции, pdf, ссылки и т.п.).
        /// </summary>
        public List<MaterialItem> Materials { get; set; } = new();

        /// <summary>
        /// Имя файла иконки курса (python_icon.png, java_icon.png и т.п.).
        /// По умолчанию — логотип школы.
        /// </summary>
        public string Icon { get; set; } = "zsm_logo.png";
    }

    /// <summary>
    /// Отдельный материал курса.
    /// </summary>
    public class MaterialItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Id курса, к которому относится материал.
        /// </summary>
        public Guid CourseId { get; set; }

        public string Title { get; set; } = "";
        public string Type { get; set; } = "";        // тип: PDF, Prezentacja, Wideo и т.п.
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";         // ссылка или путь к файлу
    }

    /// <summary>
    /// Запись о том, что ученик записан на курс + прогресс.
    /// </summary>
    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CourseId { get; set; }
        public string StudentId { get; set; } = "";   // User.Id ученика

        /// <summary>
        /// Набор Id материалов, которые ученик уже прошёл.
        /// </summary>
        public HashSet<Guid> CompletedMaterialIds { get; set; } = new();
    }
}