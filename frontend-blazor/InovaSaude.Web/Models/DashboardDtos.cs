namespace InovaSaude.Web.Models;

public class DashboardDto
{
    public decimal TotalDespesas { get; set; }
    public int TotalUbs { get; set; }
    public int TotalDespesasCount { get; set; }
    public int DespesasPendentes { get; set; }
    public List<DespesaPorCategoriaDto> DespesasPorCategoria { get; set; } = new();
    public List<DespesaPorMesDto> DespesasPorMes { get; set; } = new();
}

public class DespesaPorCategoriaDto
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Quantidade { get; set; }
}

public class DespesaPorMesDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string MesNome { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Quantidade { get; set; }
}

public class RelatorioPorUbsDto
{
    public Guid UbsId { get; set; }
    public string UbsNome { get; set; } = string.Empty;
    public decimal TotalDespesas { get; set; }
    public int QuantidadeDespesas { get; set; }
    public List<DespesaPorCategoriaDto> DespesasPorCategoria { get; set; } = new();
}

public class RelatorioFiltroDto
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public Guid? UbsId { get; set; }
    public Guid? CategoriaId { get; set; }
}
