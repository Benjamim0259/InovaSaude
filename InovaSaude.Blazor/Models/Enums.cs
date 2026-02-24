namespace InovaSaude.Blazor.Models;

public enum PerfilUsuario
{
    ADMIN,
    COORDENADOR,
    GESTOR,
    AUDITOR,
    OPERADOR,
    VISUALIZADOR
}

public enum Permissao
{
    USUARIOS_VISUALIZAR,
    USUARIOS_CRIAR,
    USUARIOS_EDITAR,
    USUARIOS_EXCLUIR,
    USUARIOS_BLOQUEAR,
    ESF_VISUALIZAR,
    ESF_CRIAR,
    ESF_EDITAR,
    ESF_EXCLUIR,
    ESF_GERENCIAR_COORDENADORES,
    DESPESAS_VISUALIZAR,
    DESPESAS_CRIAR,
    DESPESAS_EDITAR,
    DESPESAS_EXCLUIR,
    RELATORIOS_VISUALIZAR,
    RELATORIOS_EXPORTAR
}

public enum WebhookEventType
{
    DESPESA_CREATED,
    DESPESA_UPDATED,
    DESPESA_DELETED,
    DESPESA_APPROVED,
    DESPESA_REJECTED,
    UBS_CREATED,
    UBS_UPDATED,
    UBS_DELETED,
    USER_CREATED,
    USER_UPDATED,
    WORKFLOW_COMPLETED,
    PAYMENT_RECEIVED,
    INTEGRATION_SYNC
}

public enum WebhookStatus
{
    ACTIVE,
    INACTIVE,
    FAILED
}

public enum WorkflowStatus
{
    DRAFT,
    ACTIVE,
    INACTIVE
}

public enum WorkflowTriggerType
{
    MANUAL,
    AUTOMATIC,
    SCHEDULED
}

public enum WorkflowStepType
{
    APPROVAL,
    REVIEW,
    NOTIFICATION,
    ACTION,
    CONDITION
}

public enum WorkflowStepStatus
{
    PENDING,
    IN_PROGRESS,
    COMPLETED,
    FAILED,
    SKIPPED
}

public enum ApprovalAction
{
    APPROVE,
    REJECT,
    RETURN,
    ESCALATE
}

public enum CargoFuncionario
{
    Medico,
    Enfermeiro,
    TecnicoEnfermagem,
    AgenteComunitario,
    Dentista,
    Farmaceutico,
    Psicologo,
    Nutricionista,
    Fisioterapeuta,
    AssistenteSocial,
    AdministrativoGeral,
    Gerente,
    Coordenador,
    Recepcionista,
    Seguranca,
    Limpeza,
    Manutencao,
    Outros
}
