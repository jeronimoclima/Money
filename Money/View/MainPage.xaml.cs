using Money.ViewModels;
using Money.Modelos;

namespace Money.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new FinanceiroViewModel();
    }

    private void OnMesAnteriorClicked(object sender, EventArgs e) => ((FinanceiroViewModel)BindingContext).VoltarMes();
    private void OnProximoMesClicked(object sender, EventArgs e) => ((FinanceiroViewModel)BindingContext).AvancarMes();
    private async void OnNovoClicked(object sender, EventArgs e) => await Navigation.PushAsync(new NovaTransacaoPage());

    private async void OnExcluirSwipe(object sender, EventArgs e)
    {
        var transacao = (sender as SwipeItem)?.CommandParameter as Transacao;
        if (transacao != null && await DisplayAlert("Confirmação", "Deseja excluir esta transação?", "Sim", "Não"))
        {
            await ((FinanceiroViewModel)BindingContext).ExcluirTransacao(transacao);
        }
    }

    private async void OnEditarSwipe(object sender, EventArgs e)
    {
        var transacao = (sender as SwipeItem)?.CommandParameter as Transacao;
        if (transacao != null) await Navigation.PushAsync(new NovaTransacaoPage(transacao));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((FinanceiroViewModel)BindingContext).AtualizarPainel();
    }

    private async void OnEstatisticasClicked(object sender, EventArgs e)
    {
        // Navega para a página de estatísticas
        await Navigation.PushAsync(new EstatisticasPage());
    }
    private async void OnVerTudoClicked(object sender, EventArgs e)
    {
        using var banco = new Money.Dados.AppDbContexto();
        var vm = (FinanceiroViewModel)BindingContext;

        // Busca todas as transações do mês atual para passar para a próxima tela
        var listaCompleta = banco.Transacoes
            .Where(t => t.Data.Month == vm.DataNavegacao.Month && t.Data.Year == vm.DataNavegacao.Year)
            .OrderByDescending(t => t.Data)
            .ToList();

        await Navigation.PushAsync(new HistoricoPage(listaCompleta));
    }
}