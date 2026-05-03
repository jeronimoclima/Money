namespace Money
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Definimos que a página inicial é a MainPage, 
            // envolvida em uma NavigationPage para permitir navegar para a tela de cadastro.
            MainPage = new NavigationPage(new Money.Views.MainPage());
        }

        // Você pode remover o método CreateWindow se quiser simplificar, 
        // ou deixá-lo assim para manter a compatibilidade:
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage);
        }
    }
}