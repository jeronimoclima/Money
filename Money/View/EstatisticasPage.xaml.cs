using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Money.Dados;
using SkiaSharp;

namespace Money.Views;

public partial class EstatisticasPage : ContentPage
{
    public ISeries[] SeriesGrafico { get; set; }
    public Axis[] XAxes { get; set; } = { new Axis { Labels = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" } } };

    public EstatisticasPage()
    {
        InitializeComponent();
        CarregarDados();
        BindingContext = this;
    }

    private void CarregarDados()
    {
        using var banco = new AppDbContexto();
        var todas = banco.Transacoes.ToList();
        int anoAtual = DateTime.Now.Year;

        var receitas = new double[12];
        var despesas = new double[12];

        for (int i = 0; i < 12; i++)
        {
            int mes = i + 1;
            receitas[i] = todas.Where(t => t.EhReceita && t.Data.Month == mes && t.Data.Year == anoAtual).Sum(t => t.Valor);
            despesas[i] = todas.Where(t => !t.EhReceita && t.Data.Month == mes && t.Data.Year == anoAtual).Sum(t => t.Valor);
        }

        SeriesGrafico = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = receitas,
                Name = "Ganhos",
                Fill = new SolidColorPaint(SKColors.Green)
            },
            new ColumnSeries<double>
            {
                Values = despesas,
                Name = "Gastos",
                Fill = new SolidColorPaint(SKColors.Red)
            }
        };
    }
}