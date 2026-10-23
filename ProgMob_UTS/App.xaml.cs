namespace ProgMob_UTS
{
    /// <summary>
    /// Kelas aplikasi utama .NET MAUI yang mengatur siklus hidup jendela aplikasi.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Konstruktor untuk menginisialisasi resource XAML aplikasi.
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Membuat jendela utama aplikasi yang menampilkan MainPage (BlazorWebView).
        /// </summary>
        /// <param name="activationState">Status aktivasi aplikasi dari sistem operasi.</param>
        /// <returns>Instance Window utama aplikasi.</returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Sistem Informasi Akademik Sederhana" };
        }
    }
}
