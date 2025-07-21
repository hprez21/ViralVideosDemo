using ViralVideosDemo.Pages;

namespace ViralVideosDemo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Registrar rutas para navegación
            Routing.RegisterRoute("AddVideoIdea", typeof(AddVideoIdeaPage));
            Routing.RegisterRoute("VideoPrompts", typeof(VideoPromptsPage));
            Routing.RegisterRoute("Settings", typeof(SettingsPage));
        }
    }
}
