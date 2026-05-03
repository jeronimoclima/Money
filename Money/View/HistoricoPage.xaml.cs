using System.Collections.ObjectModel;
using Money.Modelos;

namespace Money.Views;

public partial class HistoricoPage : ContentPage
{
    public ObservableCollection<Transacao> Transacoes { get; set; }

    public HistoricoPage(IEnumerable<Transacao> lista)
    {
        InitializeComponent();

        Transacoes = new ObservableCollection<Transacao>(lista);
        BindingContext = this;
    }
}