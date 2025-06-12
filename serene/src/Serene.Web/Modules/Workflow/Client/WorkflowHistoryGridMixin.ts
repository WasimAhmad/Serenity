import { DataGrid } from "@serenity-is/corelib";
import { WorkflowService } from "./WorkflowService";

export interface WorkflowHistoryGridMixinOptions<TItem> {
    workflowKey: string;
    idField: keyof TItem;
}

export class WorkflowHistoryGridMixin<TItem> {
    private detailRow?: HTMLElement;
    private rowIndex: number = -1;

    constructor(private grid: DataGrid<TItem, any>, private options: WorkflowHistoryGridMixinOptions<TItem>) {
        grid.slickGrid.onClick.subscribe((e: Event, p: any) => {
            if ((e.target as HTMLElement).closest('a'))
                return;
            this.toggleDetail(p.row);
        });
        (grid.getView() as any).onRowsChanged.subscribe(() => this.removeDetail());
    }

    private async toggleDetail(row: number) {
        if (this.rowIndex === row) {
            this.removeDetail();
            return;
        }
        this.removeDetail();
        const cell = this.grid.slickGrid.getCellNode(row, 0) as HTMLElement;
        if (!cell)
            return;
        const rowEl = cell.parentElement as HTMLElement;
        const item = this.grid.itemAt(row) as any;
        const id = item[this.options.idField];
        const detail = document.createElement('div');
        detail.classList.add('workflow-history-detail');
        detail.innerHTML = '<div>Loading...</div>';
        rowEl.after(detail);
        this.detailRow = detail;
        this.rowIndex = row;
        const resp = await WorkflowService.GetHistory({
            WorkflowKey: this.options.workflowKey,
            EntityId: id
        });
        const table = document.createElement('table');
        table.classList.add('table', 'table-striped', 'table-bordered', 'workflow-history-grid');
        table.innerHTML = '<thead><tr><th>Date</th><th>From State</th><th>To State</th><th>Trigger</th><th>Input</th></tr></thead>';
        const body = document.createElement('tbody');
        for (const h of resp.History ?? []) {
            const tr = document.createElement('tr');
            tr.innerHTML = `<td>${h.EventDate}</td><td>${h.FromState}</td><td>${h.ToState}</td><td>${h.Trigger}</td><td>${this.formatInput(h.Input)}</td>`;
            body.appendChild(tr);
        }
        table.appendChild(body);
        detail.innerHTML = '';
        detail.appendChild(table);
    }

    private removeDetail() {
        if (this.detailRow) {
            this.detailRow.remove();
            this.detailRow = undefined;
            this.rowIndex = -1;
        }
    }

    private formatInput(input: any): string {
        if (input == null)
            return '';
        if (typeof input === 'object') {
            return Object.entries(input)
                .map(([k, v]) => `<div><strong>${this.escape(k)}</strong>: ${this.escape(String(v))}</div>`)
                .join('');
        }
        return this.escape(String(input));
    }

    private escape(s: string): string {
        return s.replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
}
