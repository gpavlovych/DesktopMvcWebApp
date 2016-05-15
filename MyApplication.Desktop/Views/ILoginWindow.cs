namespace MyApplication.Desktop.Views
{
    /// <summary>The LoginWindow interface.</summary>
    public interface ILoginWindow
    {
        /// <summary>Gets or sets the dialog result.</summary>
        bool? DialogResult { get; set; }

        /// <summary>Shows dialog.</summary>
        /// <returns>The result.</returns>
        bool? ShowDialog();
    }
}