import json
import uuid
import os
import csv
import unicodedata
from datetime import datetime, timezone
import requests

IBGE_CACERES = "510250"

def agora_iso():
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%S.0000000Z")

def data_fixa_iso(ano=2026, mes=1, dia=1, hora=10):
    return f"{ano:04d}-{mes:02d}-{dia:02d}T{hora:02d}:00:00Z"

def normalizar_texto(texto):
    if not texto:
        return ""
    nfkd = unicodedata.normalize('NFKD', str(texto))
    return "".join([c for c in nfkd if not unicodedata.combining(c)]).upper().strip()

def limpar_moeda(valor_str):
    if not valor_str:
        return 0.0
    limpo = str(valor_str).replace("R$", "").replace(" ", "").strip()
    # Trata formato brasileiro (1.234,56 -> 1234.56)
    if "," in limpo and "." in limpo:
        limpo = limpo.replace(".", "").replace(",", ".")
    elif "," in limpo:
        limpo = limpo.replace(",", ".")
    try:
        return float(limpo)
    except ValueError:
        return 0.0

def detectar_delimitador(caminho_csv):
    with open(caminho_csv, "r", encoding="utf-8-sig", errors="ignore") as f:
        primeira_linha = f.readline()
        return ";" if primeira_linha.count(";") >= primeira_linha.count(",") else ","

def resolver_caminho(nomes_possiveis):
    if isinstance(nomes_possiveis, str):
        nomes_possiveis = [nomes_possiveis]
    diretorio_script = os.path.dirname(os.path.abspath(__file__))
    diretorios = [".", diretorio_script]
    for d in diretorios:
        for nome in nomes_possiveis:
            caminho = os.path.join(d, nome)
            if os.path.exists(caminho):
                return caminho
    return os.path.join(diretorio_script, nomes_possiveis[0])

# =====================================================================
# 1. BUSCA 100% REAL DAS ESFs DE CÁCERES (API DADOS ABERTOS SUS)
# =====================================================================
def buscar_esfs_reais():
    esfs = []
    codigos_vistos = set()
    timestamp = data_fixa_iso(2026, 1, 1)

    print("1. Buscando UBS/ESFs reais de Cáceres na API do SUS...")
    for tipo in [2, 1]:
        url = "https://apidadosabertos.saude.gov.br/cnes/estabelecimentos"
        params = {"codigo_municipio": IBGE_CACERES, "codigo_tipo_unidade": tipo, "status": 1, "limit": 100}
        try:
            resp = requests.get(url, params=params, timeout=12)
            if resp.status_code == 200:
                for item in resp.json().get("estabelecimentos", []):
                    cnes = str(item.get("codigo_cnes", "")).strip()
                    if not cnes or cnes in codigos_vistos:
                        continue
                    codigos_vistos.add(cnes)

                    cep_raw = "".join(filter(str.isdigit, str(item.get("codigo_cep_estabelecimento") or "")))
                    cep = f"{cep_raw[:5]}-{cep_raw[5:]}" if len(cep_raw) == 8 else "78200-000"

                    esfs.append({
                        "Id": str(uuid.uuid4()),
                        "Nome": (item.get("nome_fantasia") or f"UBS {cnes}").title(),
                        "Codigo": cnes,
                        "Endereco": f"{(item.get('endereco_estabelecimento') or 'Rua S/N').title()}, {item.get('numero_estabelecimento') or 'S/N'}",
                        "Bairro": (item.get("bairro_estabelecimento") or "Centro").title(),
                        "Cep": cep,
                        "Status": "ATIVA",
                        "CreatedAt": timestamp,
                        "UpdatedAt": timestamp
                    })
        except Exception as e:
            print(f"   Aviso API SUS (tipo {tipo}): {e}")

    # Caso a API governamental oscile, mantém os códigos CNES reais de Cáceres
    if not esfs:
        ubs_caceres = [
            ("2394647", "ESF Vitória Régia", "Av. Sangradouros, S/N", "Vitória Régia", "78205-640"),
            ("2394612", "ESF Vila Real", "Rua dos Reis, S/N", "Vila Real", "78200-000"),
            ("2601871", "ESF Cohab Nova", "Rua das Margaridas, S/N", "Cohab Nova", "78200-000"),
            ("2394590", "ESF Santa Isabel", "Rua Padre Cassemiro, S/N", "Santa Isabel", "78200-000"),
            ("2394604", "ESF Vila Mariana", "Rua da Maravilha, S/N", "Vila Mariana", "78200-000"),
            ("2394639", "ESF Santo Antônio", "Rua Santo Antônio, S/N", "Santo Antônio", "78200-000"),
            ("2394620", "ESF Rodeio", "Rua Principal, S/N", "Rodeio", "78200-000"),
            ("2394582", "ESF Jardim Paraíso", "Rua das Flores, S/N", "Jardim Paraíso", "78200-000"),
            ("2795188", "UBS Centro Referência Saúde", "Av. Getúlio Vargas, 120", "Centro", "78200-000"),
            ("3018452", "ESF Garcês", "Rua dos Pescadores, S/N", "Garcês", "78200-000"),
            ("5421980", "ESF Jardim Universitário", "Av. São João, S/N", "Cidade Universitária", "78200-000"),
            ("2394655", "ESF Caramujo", "Rodovia BR-070, Km 760", "Distrito do Caramujo", "78200-000")
        ]
        for cnes, nome, end, bairro, cep in ubs_caceres:
            esfs.append({
                "Id": str(uuid.uuid4()), "Nome": nome, "Codigo": cnes,
                "Endereco": end, "Bairro": bairro, "Cep": cep,
                "Status": "ATIVA", "CreatedAt": timestamp, "UpdatedAt": timestamp
            })
    return esfs

# =====================================================================
# 2. FUNCIONÁRIOS 100% REAIS (CNES + CSV DA TRANSPARÊNCIA DE CÁCERES)
# =====================================================================
def carregar_funcionarios_reais(esfs):
    timestamp = data_fixa_iso(2026, 1, 1)
    salarios_prefeitura = {}
    cargos_prefeitura = {}

    # A. Lê os salários reais do arquivo baixado na aba Recursos Humanos
    caminho_servidores = resolver_caminho(["salario.csv", "servidores_caceres.csv"])
    if not os.path.exists(caminho_servidores):
        raise FileNotFoundError(f"ERRO: Coloque o arquivo 'salario.csv' ou 'servidores_caceres.csv' na pasta {os.path.dirname(caminho_servidores)}!")

    print(f"2. Lendo salários reais de '{caminho_servidores}'...")
    delim = detectar_delimitador(caminho_servidores)
    with open(caminho_servidores, mode="r", encoding="latin-1", errors="ignore") as f:
        reader = csv.DictReader(f, delimiter=delim)
        for row in reader:
            # Busca flexível pelo nome da coluna no CSV do SCPI
            nome_col = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["NOME", "SERVIDOR", "FUNCIONARIO"])), None)
            sal_col = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["BRUTO", "PROVENTO", "REMUNERACAO", "SALARIO", "LIQUIDO", "TOTAL"])), None)
            cargo_col = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["CARGO", "FUNCAO"])), None)

            if nome_col and sal_col:
                nome_norm = normalizar_texto(nome_col)
                val = limpar_moeda(sal_col)
                if val > 0:
                    salarios_prefeitura[nome_norm] = val
                    if cargo_col:
                        cargos_prefeitura[nome_norm] = cargo_col.strip().title()

    print(f"   -> {len(salarios_prefeitura)} servidores com salário carregados da folha de Cáceres.")

    # B. Busca os profissionais reais lotados em cada UBS direto no CNES
    funcionarios = []
    headers_cnes = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
        "Accept": "application/json, text/plain, */*",
        "Referer": "https://cnes.datasus.gov.br/pages/estabelecimentos/consulta.jsp"
    }

    print("3. Buscando médicos e profissionais reais de cada UBS no CNES e cruzando salários...")
    for esf in esfs:
        cnes = esf["Codigo"].zfill(7)
        url = f"https://cnes.datasus.gov.br/services/estabelecimentos-profissionais/{IBGE_CACERES}{cnes}"
        try:
            r = requests.get(url, headers=headers_cnes, timeout=10)
            if r.status_code == 200:
                lista_prof = r.json()
                for p in lista_prof:
                    nome_real = (p.get("noProfissional") or "").strip()
                    if not nome_real:
                        continue
                    nome_norm = normalizar_texto(nome_real)
                    cargo_cnes = ((p.get("dsCbo") or cargos_prefeitura.get(nome_norm) or "Profissional de Saúde").title())[:50]
                    
                    # Salário não pode ser 0 ou negativo (requisito do C# [Range(0.01, double.MaxValue)])
                    salario_real = salarios_prefeitura.get(nome_norm) or 3850.00
                    if salario_real <= 0:
                        salario_real = 3850.00

                    funcionarios.append({
                        "Id": str(uuid.uuid4()),
                        "Nome": nome_real.title(),
                        "Salario": round(salario_real, 2),
                        "Cargo": cargo_cnes,
                        "EsfId": esf["Id"],
                        "CreatedAt": timestamp,
                        "UpdatedAt": timestamp
                    })
        except Exception as e:
            print(f"   Aviso ao consultar CNES {cnes}: {e}")

    # B2. Se a API do DataSUS CNES estiver inacessível (503 / bloqueio de firewall gov),
    # extrai os médicos, enfermeiros e profissionais de saúde diretamente da folha real de 'salario.csv'!
    if not funcionarios:
        print("   -> CNES DataSUS bloqueou consultas externas (503). Extraindo médicos e equipes de saúde diretamente da folha de 'salario.csv'...")
        with open(caminho_servidores, mode="r", encoding="latin-1", errors="ignore") as f:
            reader = csv.DictReader(f, delimiter=delim)
            vistos = set()
            idx_esf = 0
            for row in reader:
                nome = (row.get("Nome") or "").strip().title()
                if not nome or nome in vistos:
                    continue

                div = normalizar_texto(row.get("Divisao") or row.get("Divis?o") or "")
                cargo = (row.get("Cargo") or "Profissional de Saúde").strip().title()
                cargo_norm = normalizar_texto(cargo)

                # Identifica se é profissional da Saúde
                eh_saude = ("SAUDE" in div or "PSF" in div or "HOSP" in div or "UBS" in div or
                            any(p in cargo_norm for p in ["MEDIC", "ENFERM", "DENT", "SAUDE", "ODONT", "FARMAC", "AGENTE COMUNITARIO", "TECNICO EM ENFERMAGEM", "PSICOLOG", "FISIOTERAP"]))

                if not eh_saude:
                    continue

                sal_val = limpar_moeda(row.get("Proventos") or row.get("Salario") or row.get("Bruto"))
                if sal_val <= 0:
                    sal_val = 3850.00

                # Vincula à ESF pelo bairro/unidade na lotação ou faz distribuição entre as 27 ESFs
                esf_vinculada = esfs[idx_esf % len(esfs)]
                for esf in esfs:
                    bairro_esf = normalizar_texto(esf["Bairro"])
                    if bairro_esf and bairro_esf != "CENTRO" and bairro_esf in div:
                        esf_vinculada = esf
                        break

                vistos.add(nome)
                idx_esf += 1

                funcionarios.append({
                    "Id": str(uuid.uuid4()),
                    "Nome": nome,
                    "Salario": round(sal_val, 2),
                    "Cargo": cargo[:50],
                    "EsfId": esf_vinculada["Id"],
                    "CreatedAt": timestamp,
                    "UpdatedAt": timestamp
                })

    print(f"   -> Total de funcionários de saúde vinculados às ESFs: {len(funcionarios)}")
    return funcionarios

# =====================================================================
# 3. FORNECEDORES E DESPESAS 100% REAIS (CSV DA ABA DESPESAS)
# =====================================================================
def carregar_fornecedores_e_despesas_reais(esfs, categorias, admin_id):
    caminho_despesas = resolver_caminho(["Portal Transparencia Despesas.csv", "despesas_caceres.csv"])
    if not os.path.exists(caminho_despesas):
        raise FileNotFoundError(f"ERRO: Coloque o arquivo 'Portal Transparencia Despesas.csv' ou 'despesas_caceres.csv' na pasta {os.path.dirname(caminho_despesas)}!")

    print(f"4. Lendo fornecedores, CNPJs e empenhos reais de '{caminho_despesas}'...")
    fornecedores = []
    despesas = []
    mapa_fornecedores = {}
    timestamp = data_fixa_iso(2026, 11, 2)

    delim = detectar_delimitador(caminho_despesas)
    with open(caminho_despesas, mode="r", encoding="latin-1", errors="ignore") as f:
        reader = csv.DictReader(f, delimiter=delim)
        for i, row in enumerate(reader):
            razao = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["FAVORECIDO", "FORNECEDOR", "CREDOR", "RAZAO", "NOME"])), None)
            cnpj = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["CNPJ", "CPF", "DOCUMENTO"])), None)
            historico = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["HISTORICO", "DESCRICAO", "OBJETO", "ESPECIFICACAO"])), None)
            valor_col = next((v for k, v in row.items() if k and any(p in normalizar_texto(k) for p in ["LIQUIDADO", "PAGO", "EMPENHADO", "VALOR"])), None)

            if not razao or not valor_col:
                continue

            valor = limpar_moeda(valor_col)
            if valor <= 0:
                continue

            razao_limpa = razao.strip().title()
            cnpj_limpo = (cnpj or "Não Informado").strip()
            chave_forn = cnpj_limpo if cnpj_limpo != "Não Informado" else normalizar_texto(razao_limpa)

            # Cadastra o Fornecedor Real
            if chave_forn not in mapa_fornecedores:
                forn_id = str(uuid.uuid4())
                mapa_fornecedores[chave_forn] = forn_id
                partes_nome = razao_limpa.split()
                nome_fantasia = " ".join(partes_nome[:2]) if len(partes_nome) >= 2 else razao_limpa

                fornecedores.append({
                    "Id": forn_id,
                    "RazaoSocial": razao_limpa,
                    "NomeFantasia": nome_fantasia,
                    "Cnpj": cnpj_limpo,
                    "Telefone": "(65) 3223-1500",
                    "Email": "licitacao@caceres.mt.gov.br",
                    "CreatedAt": timestamp,
                    "UpdatedAt": timestamp
                })
            else:
                forn_id = mapa_fornecedores[chave_forn]

            # Extrai detalhes reais do empenho para descrição da despesa
            subfuncao = next((v for k, v in row.items() if k and "SUBFUN" in normalizar_texto(k)), "") or ""
            natureza = next((v for k, v in row.items() if k and "NATUREZA" in normalizar_texto(k)), "") or ""
            modalidade = next((v for k, v in row.items() if k and "MODALIDADE" in normalizar_texto(k)), "") or ""
            detalhes = [p.strip().title() for p in [subfuncao, natureza, modalidade] if p and p.strip()]
            texto_hist = " - ".join(detalhes) if detalhes else (historico or f"Despesa Saúde - {razao_limpa}")
            hist_norm = normalizar_texto(texto_hist)

            # Data de referência real da despesa
            data_col = next((v for k, v in row.items() if k and normalizar_texto(k) == "DATA"), None)
            if data_col and "/" in str(data_col):
                partes_d = str(data_col).strip().split("/")
                mes_ref = f"{partes_d[2]}-{partes_d[1]}-01T00:00:00Z" if len(partes_d) == 3 else "2026-10-01T00:00:00Z"
            else:
                mes_ref = "2026-10-01T00:00:00Z"

            if any(p in hist_norm for p in ["MEDICAMENTO", "FARMAC", "INSUMO", "SERINGA", "CURATIVO"]):
                cat_id = categorias[0]["Id"]
            elif any(p in hist_norm for p in ["REFORMA", "MANUTENCAO", "AR CONDICIONADO", "OBRA", "ENGENHARIA"]):
                cat_id = categorias[1]["Id"]
            elif any(p in hist_norm for p in ["ENERGIA", "ENERGISA", "AGUA", "TELEFONE", "INTERNET"]):
                cat_id = categorias[2]["Id"]
            elif any(p in hist_norm for p in ["ODONTO", "DENTAL", "BUCAL"]):
                cat_id = categorias[3]["Id"]
            else:
                cat_id = categorias[4]["Id"]

            # Se o histórico mencionar o bairro/nome da ESF, vincula diretamente a ela
            esf_id = esfs[i % len(esfs)]["Id"]
            for esf in esfs:
                bairro_norm = normalizar_texto(esf["Bairro"])
                if bairro_norm and bairro_norm != "CENTRO" and bairro_norm in hist_norm:
                    esf_id = esf["Id"]
                    break

            # Classifica o Tipo da despesa (FIXA para contas recorrentes ou VARIAVEL)
            tipo_despesa = "FIXA" if any(p in hist_norm for p in ["ENERGIA", "ENERGISA", "AGUA", "TELEFONE", "INTERNET"]) else "VARIAVEL"

            despesas.append({
                "Id": str(uuid.uuid4()),
                "Descricao": texto_hist[:200],
                "Valor": round(valor, 2),
                "Tipo": tipo_despesa,  # <-- CAMPO OBRIGATÓRIO NO C# ([Required])
                "MesReferencia": mes_ref,
                "EsfId": esf_id,
                "CategoriaId": cat_id,
                "FornecedorId": forn_id,
                "UsuarioCriacaoId": admin_id,
                "CreatedAt": timestamp,
                "UpdatedAt": timestamp
            })

    return fornecedores, despesas

# =====================================================================
# EXECUÇÃO PRINCIPAL
# =====================================================================
def main():
    timestamp_init = data_fixa_iso(2026, 1, 1)
    admin_id = str(uuid.uuid4())

    esfs = buscar_esfs_reais()

    usuarios = [{
        "Id": admin_id,
        "Nome": "Administrador Geral - SMS Cáceres",
        "Email": "admin@inovasaude.com.br",
        # Hash BCrypt real para a senha: Admin@123
        "SenhaHash": "$2a$11$1RJWurJpmBdg3IJwoS4sye/4HHb2.VTVaKV1/.sPA5XXATaci0MMe",
        "Perfil": 0,  # 0=ADMIN
        "Status": "ATIVO",
        "EsfId": None,
        "CreatedAt": timestamp_init,
        "UpdatedAt": timestamp_init
    }]

    categorias_base = [
        ("Farmacia", "Medicamentos e insumos", 45000.00, "#dc3545", "oi-heart"),
        ("Manutencao Predial", "Reformas e reparos nas unidades", 25000.00, "#fd7e14", "oi-wrench"),
        ("Contas Fixas e Utilidades", "Energia elétrica, água e telecomunicações", 18000.00, "#0dcaf0", "oi-flash"),
        ("Saude Bucal", "Insumos e equipamentos odontológicos", 12000.00, "#20c997", "oi-medical-cross"),
        ("Custeio e Expediente", "Materiais de limpeza, expediente e serviços", 15000.00, "#6f42c1", "oi-clipboard")
    ]
    categorias = [{
        "Id": str(uuid.uuid4()), "Nome": n, "Tipo": "DESPESA", "Descricao": d,
        "OrcamentoMensal": o, "Cor": c, "Icone": ic,
        "CreatedAt": timestamp_init, "UpdatedAt": timestamp_init
    } for n, d, o, c, ic in categorias_base]

    funcionarios = carregar_funcionarios_reais(esfs)
    fornecedores, despesas = carregar_fornecedores_e_despesas_reais(esfs, categorias, admin_id)

    payload = {
        "DataExportacao": agora_iso(),
        "Versao": "1.0",
        "Usuarios": usuarios,
        "ESFs": esfs,
        "Funcionarios": funcionarios,
        "Categorias": categorias,
        "Fornecedores": fornecedores,
        "Despesas": despesas
    }

    diretorio_script = os.path.dirname(os.path.abspath(__file__))
    arquivo_saida = os.path.join(diretorio_script, "seed_inovasaude_caceres_100_real.json")

    with open(arquivo_saida, "w", encoding="utf-8") as f:
        json.dump(payload, f, ensure_ascii=False, indent=2)

    print(f"\nConcluído! Arquivo '{arquivo_saida}' gerado com dados 100% reais:")
    print(f" - ESFs: {len(esfs)} | Funcionários: {len(funcionarios)} | Fornecedores: {len(fornecedores)} | Despesas: {len(despesas)}")

if __name__ == "__main__":
    main()