using Indexr.MAUI.ViewModels;

namespace Indexr.MAUI.Views
{
    public partial class ProjectPage : ContentPage
    {
        public ProjectPage(ProjectViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}