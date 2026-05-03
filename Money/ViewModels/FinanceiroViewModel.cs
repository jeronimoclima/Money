using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Money.Dados;
using Money.Modelos;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Money.ViewModels;

public partial class FinanceiroViewModel : ObservableObject
{
    private readonly AppDbContexto _contexto;

    [ObservableProperty]
    private double saldoGeral;

    [ObservableProperty]
    private string conselhoFinanceiro = "Analisando dados...";

    [ObservableProperty]
    private DateTime dataNavegacao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    public string MesAnoExtenso => DataNavegacao.ToString("MMMM yyyy").ToUpper();

    public ObservableCollection<Transacao> TransacoesRecentes { get; set; } = new();

    [ObservableProperty]
    private ISeries[] seriesGraficoAnual = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] seriesGraficoPizza = Array.Empty<ISeries>();

    public Axis[] XAxes { get; set; } = { new Axis { Labels = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" } } };

    public FinanceiroViewModel()
    {
        _contexto = new AppDbContexto();
        _contexto.Database.EnsureCreated();
        AtualizarPainel();
    }

    // Dentro do método AtualizarPainel()
    public void AtualizarPainel()
    {
        var todasHistorico = _contexto.Transacoes.ToList();
        SaldoGeral = todasHistorico.Sum(t => t.EhReceita ? t.Valor : -t.Valor);

        // 1. Pegamos todas as transações do mês para o histórico completo
        var todasDoMes = todasHistorico
            .Where(t => t.Data.Month == DataNavegacao.Month && t.Data.Year == DataNavegacao.Year)
            .OrderByDescending(t => t.Data)
            .ToList();

        // 2. Limitamos a exibição da tela inicial em apenas 5 itens
        TransacoesRecentes.Clear();
        foreach (var t in todasDoMes.Take(5))
        {
            TransacoesRecentes.Add(t);
        }

        GerarConselho(todasDoMes);
        AtualizarGraficos(todasHistorico);
        OnPropertyChanged(nameof(MesAnoExtenso));
    }

    public async Task ExcluirTransacao(Transacao t)
    {
        _contexto.Transacoes.Remove(t);
        await _contexto.SaveChangesAsync();
        AtualizarPainel();
    }

    private void AtualizarGraficos(List<Transacao> todas)
    {
        int ano = DataNavegacao.Year;
        var receitasAnuais = new double[12];
        var despesasAnuais = new double[12];

        for (int i = 0; i < 12; i++)
        {
            int m = i + 1;
            receitasAnuais[i] = todas.Where(t => t.EhReceita && t.Data.Month == m && t.Data.Year == ano).Sum(t => t.Valor);
            despesasAnuais[i] = todas.Where(t => !t.EhReceita && t.Data.Month == m && t.Data.Year == ano).Sum(t => t.Valor);
        }

        SeriesGraficoAnual = new ISeries[] {
            new LineSeries<double> { Values = receitasAnuais, Name = "Receitas", Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 }, GeometrySize = 10, Fill = null },
            new LineSeries<double> { Values = despesasAnuais, Name = "Despesas", Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 }, GeometrySize = 10, Fill = null }
        };

        SeriesGraficoPizza = todas
            .Where(t => !t.EhReceita && t.Data.Month == DataNavegacao.Month && t.Data.Year == ano)
            .GroupBy(t => t.Categoria)
            .Select(g => new PieSeries<double> { Values = new double[] { g.Sum(x => x.Valor) }, Name = g.Key })
            .ToArray();
    }

    public void AvancarMes() { DataNavegacao = DataNavegacao.AddMonths(1); AtualizarPainel(); }
    public void VoltarMes() { DataNavegacao = DataNavegacao.AddMonths(-1); AtualizarPainel(); }

    private void GerarConselho(List<Transacao> listaDoMes)
    {
        var maiorGasto = listaDoMes.Where(t => !t.EhReceita).GroupBy(t => t.Categoria).OrderByDescending(g => g.Sum(x => x.Valor)).FirstOrDefault();
        ConselhoFinanceiro = maiorGasto != null ? $"Sua maior despesa em {MesAnoExtenso} é '{maiorGasto.Key}'." : "Tudo sob controle por aqui!";
    }
}