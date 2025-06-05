import { Dialog, Decorators } from "@serenity-is/corelib";
import { WorkflowService, GetWorkflowHistoryRequest } from "./WorkflowService";

@Decorators.registerClass('Serene.Workflow.WorkflowHistoryDialog')
export class WorkflowHistoryDialog extends Dialog<GetWorkflowHistoryRequest, any> {
    private grid: HTMLTableElement;

    constructor() {
        super();
        this.dialogTitle = 'Workflow History';
        this.grid = document.createElement('table');
        this.element.appendChild(this.grid);
    }

    protected async onDialogOpen() {
        super.onDialogOpen();
        const req = this.options as GetWorkflowHistoryRequest;
        const r = await WorkflowService.GetHistory(req);
        const body = document.createElement('tbody');
        for (const h of r.History ?? []) {
            const tr = document.createElement('tr');
            tr.innerHTML = `<td>${h.EventDate}</td><td>${h.FromState}</td><td>${h.ToState}</td><td>${h.Trigger}</td>`;
            body.appendChild(tr);
        }
        this.grid.appendChild(body);
    }
}

