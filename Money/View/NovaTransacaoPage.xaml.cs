using Money.Dados;
using Money.Modelos;

namespace Money.Views;

public partial class NovaTransacaoPage : ContentPage
{
    private Transacao _edicao;
    private bool _ehReceita;

    // Listas de Categorias conforme solicitado
    private readonly List<string> _categoriasReceita = new()
    {
        "Salário", "Freelance / Serviços", "Vendas", "Investimentos", "Cashback", "Presentes", "Outras receitas"
    };

    private readonly List<string> _categoriasDespesa = new()
    {
        "🏠 Moradia", "🍔 Alimentação", "🚗 Transporte", "🧾 Contas e Assinaturas",
        "🛍️ Compras", "💊 Saúde", "🎓 Educação", "🎉 Lazer", "💳 Financeiro",
        "🧒 Família", "📦 Outros"
    };

    public NovaTransacaoPage(Transacao transacao = null)
    {
        InitializeComponent();
        _edicao = transacao;

        if (_edicao != null)
        {
            CarregarDadosEdicao();
        }
        else
        {
            // Valor padrão inicial
            SelecionarTipo(false); // Inicia como despesa por padrão
        }
    }

    private void OnTipoSelected(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        SelecionarTipo(btn == btnReceita);
    }

    private void SelecionarTipo(bool ehReceita)
    {
        _ehReceita = ehReceita;

        // Ajuste visual dos botões
        btnReceita.BackgroundColor = _ehReceita ? Colors.Green : Color.FromArgb("#EEF7EE");
        btnReceita.TextColor = _ehReceita ? Colors.White : Colors.Green;

        btnDespesa.BackgroundColor = !_ehReceita ? Colors.Red : Color.FromArgb("#FEEEEE");
        btnDespesa.TextColor = !_ehReceita ? Colors.White : Colors.Red;

        // Atualiza o Picker de categorias
        pckCategoria.ItemsSource = _ehReceita ? _categoriasReceita : _categoriasDespesa;
        pckCategoria.SelectedIndex = 0;
    }

    private void CarregarDadosEdicao()
    {
        Title = "Editar Movimentação";
        txtDescricao.Text = _edicao.Descricao;
        txtValor.Text = _edicao.Valor.ToString();
        SelecionarTipo(_edicao.EhReceita);
        pckCategoria.SelectedItem = _edicao.Categoria;
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtValor.Text) || string.IsNullOrWhiteSpace(txtDescricao.Text))
        {
            await DisplayAlert("Erro", "Preencha a descrição e o valor.", "OK");
            return;
        }

        using var banco = new AppDbContexto();

        if (_edicao == null)
        {
            var nova = new Transacao
            {
                Descricao = txtDescricao.Text,
                Valor = double.Parse(txtValor.Text),
                Categoria = pckCategoria.SelectedItem?.ToString(),
                EhReceita = _ehReceita,
                Data = DateTime.Now
            };
            banco.Transacoes.Add(nova);
        }
        else
        {
            _edicao.Descricao = txtDescricao.Text;
            _edicao.Valor = double.Parse(txtValor.Text);
            _edicao.EhReceita = _ehReceita;
            _edicao.Categoria = pckCategoria.SelectedItem?.ToString();
            banco.Transacoes.Update(_edicao);
        }

        await banco.SaveChangesAsync();
        await Navigation.PopAsync();
    }
}