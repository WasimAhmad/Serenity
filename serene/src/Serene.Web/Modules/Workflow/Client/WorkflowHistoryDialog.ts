import { BaseDialog, Decorators, htmlEncode } from "@serenity-is/corelib";
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
        header.innerHTML = `<tr><th>Date</th><th>User</th><th>From State</th><th>To State</th><th>Trigger</th><th>Input</th></tr>`;
        this.grid.appendChild(header);
        this.element.append(this.grid);
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
            const inputHtml = formatInput(h.Input);
            tr.innerHTML = `<td>${formatRelative(h.EventDate)}</td><td>${htmlEncode(h.User ?? '')}</td><td>${h.FromState}</td><td>${h.ToState}</td><td>${h.Trigger}</td><td>${inputHtml}</td>`;
            body.appendChild(tr);
        }
        this.grid.appendChild(body);
    }
}

function formatInput(input: any): string {
    if (input == null)
        return '';

    if (typeof input === 'object') {
        return Object.entries(input)
            .map(([k, v]) => `<div><strong>${htmlEncode(k)}</strong>: ${htmlEncode(v)}</div>`)
            .join('');
    }

    return htmlEncode(String(input));
}

function formatRelative(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (seconds < 60)
        return 'just now';
    const minutes = Math.floor(seconds / 60);
    if (minutes < 60)
        return `${minutes} minute${minutes !== 1 ? 's' : ''} ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24)
        return `${hours} hour${hours !== 1 ? 's' : ''} ago`;
    const days = Math.floor(hours / 24);
    if (days < 7)
        return `${days} day${days !== 1 ? 's' : ''} ago`;
    return date.toLocaleString();
}

