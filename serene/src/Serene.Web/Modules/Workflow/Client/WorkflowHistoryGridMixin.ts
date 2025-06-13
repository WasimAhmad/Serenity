import { DataGrid, htmlEncode } from "@serenity-is/corelib";
import { WorkflowService } from "./WorkflowService";

export interface WorkflowHistoryGridMixinOptions<TItem> {
    workflowKey: string;
    idField: keyof TItem;
}

export class WorkflowHistoryGridMixin<TItem> {
    private detailRow?: HTMLElement;
    private rowIndex: number = -1;
    private scrollContainer?: HTMLElement;
    private reposition?: () => void;

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
        detail.style.position = 'absolute';
        const viewport = this.grid.slickContainer.getNode().querySelector('.slick-viewport') as HTMLElement;
        if (viewport) {
            this.scrollContainer = viewport;
        }
        const reposition = () => {
            detail.style.top = (rowEl.offsetTop + rowEl.offsetHeight) + 'px';
            detail.style.left = '0';
            detail.style.width = rowEl.clientWidth + 'px';
        };
        reposition();
        this.reposition = reposition;
        this.scrollContainer?.addEventListener('scroll', reposition);
        rowEl.after(detail);
        this.detailRow = detail;
        this.rowIndex = row;
        const resp = await WorkflowService.GetHistory({
            WorkflowKey: this.options.workflowKey,
            EntityId: id
        });
        const table = document.createElement('table');
        table.classList.add('table', 'table-striped', 'table-bordered', 'workflow-history-grid');
        table.innerHTML = '<thead><tr><th>Date</th><th>User</th><th>From State</th><th>To State</th><th>Trigger</th><th>Input</th></tr></thead>';
        const body = document.createElement('tbody');
        for (const h of resp.History ?? []) {
            const tr = document.createElement('tr');
            tr.innerHTML = `<td>${this.formatRelative(h.EventDate)}</td><td>${htmlEncode(h.User ?? '')}</td><td>${h.FromState}</td><td>${h.ToState}</td><td>${h.Trigger}</td><td>${this.formatInput(h.Input)}</td>`;
            body.appendChild(tr);
        }
        table.appendChild(body);
        detail.innerHTML = '';
        detail.appendChild(table);
    }

    private removeDetail() {
        if (this.detailRow) {
            if (this.scrollContainer && this.reposition)
                this.scrollContainer.removeEventListener('scroll', this.reposition);
            this.detailRow.remove();
            this.detailRow = undefined;
            this.scrollContainer = undefined;
            this.reposition = undefined;
            this.rowIndex = -1;
        }
    }

    private formatInput(input: any): string {
        if (input == null)
            return '';
        if (typeof input === 'object') {
            return Object.entries(input)
                .map(([k, v]) => `<div><strong>${htmlEncode(k)}</strong>: ${htmlEncode(v)}</div>`)
                .join('');
        }
        return htmlEncode(String(input));
    }

    private formatRelative(dateStr: string): string {
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
}
