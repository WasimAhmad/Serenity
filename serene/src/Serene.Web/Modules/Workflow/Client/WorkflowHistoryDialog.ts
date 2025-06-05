import { BaseDialog, Decorators } from "@serenity-is/corelib";
import { WorkflowService, GetWorkflowHistoryRequest } from "./WorkflowService";

@Decorators.registerClass('Serene.Workflow.WorkflowHistoryDialog')
export class WorkflowHistoryDialog extends BaseDialog<GetWorkflowHistoryRequest> {
    private grid: HTMLTableElement;

    constructor() {
        super();
        this.dialogTitle = 'Workflow History';
        this.grid = document.createElement('table');
        this.grid.classList.add('table', 'table-striped', 'table-bordered', 'workflow-history-grid');
        const header = document.createElement('thead');
        header.innerHTML = `<tr><th>Date</th><th>From State</th><th>To State</th><th>Trigger</th></tr>`;
        this.grid.appendChild(header);
        this.element.appendChild(this.grid);
    }

    public loadAndOpenDialog(request: GetWorkflowHistoryRequest, asPanel?: boolean) {
        Object.assign(this.options as any, request);
        this.dialogOpen(asPanel);
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

