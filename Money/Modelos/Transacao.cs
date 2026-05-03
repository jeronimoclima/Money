namespace Money.Modelos;

public class Transacao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public double Valor { get; set; }
    public DateTime Data { get; set; }
    public string Categoria { get; set; } = "Outros";
    public bool EhReceita { get; set; }

    // --- NOVAS PROPRIEDADES AUXILIARES ---

    // 1. Data formatada para exibição discreta (ex: "02/05/2026")
    public string DataExibicao => Data.ToString("dd/MM/yyyy");

    // 2. Propriedade para facilitar a lógica de cores e filtros
    // Retorna o valor negativo se for despesa, facilitando cálculos de soma
    public double ValorCorrigido => EhReceita ? Valor : -Valor;

    // 3. Cor baseada no tipo (Opcional, se preferir não usar Triggers no XAML)
    public Color CorValor => EhReceita ? Colors.Green : Colors.Red;
}