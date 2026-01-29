using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class WorkflowService
{
    private readonly ApplicationDbContext _context;

    public WorkflowService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Workflow>> GetAllWorkflowsAsync()
    {
        return await _context.Workflows
            .Include(w => w.Steps)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<Workflow?> GetWorkflowByIdAsync(string id)
    {
        return await _context.Workflows
            .Include(w => w.Steps)
            .Include(w => w.Instances)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task CreateWorkflowAsync(Workflow workflow)
    {
        _context.Workflows.Add(workflow);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWorkflowAsync(Workflow workflow)
    {
        _context.Workflows.Update(workflow);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkflowAsync(string id)
    {
        var workflow = await _context.Workflows.FindAsync(id);
        if (workflow != null)
        {
            _context.Workflows.Remove(workflow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstancesAsync(string? workflowId = null)
    {
        var query = _context.WorkflowInstances
            .Include(w => w.Workflow)
            .AsQueryable();

        if (!string.IsNullOrEmpty(workflowId))
            query = query.Where(w => w.WorkflowId == workflowId);

        return await query
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task ExecuteWorkflowStepAsync(string instanceId, string stepId, string userId, ApprovalAction action, string? comments = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.StepInstances)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null) throw new Exception("Instância do workflow não encontrada");

            var stepInstance = instance.StepInstances.FirstOrDefault(s => s.StepId == stepId);
            if (stepInstance == null) throw new Exception("Instância do passo não encontrada");

            stepInstance.Status = WorkflowStepStatus.COMPLETED;
            stepInstance.CompletedBy = userId;
            stepInstance.CompletedAt = DateTime.UtcNow;
            stepInstance.Action = action;
            stepInstance.Comments = comments;

            // Verificar se o workflow está completo
            var allStepsCompleted = instance.StepInstances.All(s => s.Status == WorkflowStepStatus.COMPLETED);
            if (allStepsCompleted)
            {
                instance.Status = WorkflowStepStatus.COMPLETED;
                instance.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}