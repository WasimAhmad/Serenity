import { EntityGrid, Decorators, htmlEncode } from "@serenity-is/corelib";
import { DocumentRow, DocumentColumns, DocumentService } from "../ServerTypes/Documents";
import { DocumentDialog } from "./DocumentDialog";
import { WorkflowService } from "../Workflow/Client/WorkflowService";

@Decorators.registerClass('Serene.Documents.DocumentGrid')
export class DocumentGrid extends EntityGrid<DocumentRow, any> {
    private historyPanel?: HTMLElement;
    private expandedId?: number;
    protected useAsync() { return true; }
    protected getColumnsKey() { return DocumentColumns.columnsKey; }
    protected getDialogType() { return DocumentDialog; }
    protected getIdProperty() { return DocumentRow.idProperty; }
    protected getLocalTextPrefix() { return DocumentRow.localTextPrefix; }
    protected getService() { return DocumentService.baseUrl; }

    protected onClick(e: MouseEvent, row: number, cell: number): void {
        super.onClick(e, row, cell);
        if ((e as any)?.isDefaultPrevented?.())
            return;

        const item = this.itemAt(row);
        const id = (item as any)[this.getIdProperty()];
        if (id == null)
            return;

        if (this.expandedId === id) {
            this.hideHistory();
            return;
        }

        this.showHistory(item, row);
    }

    private hideHistory() {
        if (this.historyPanel) {
            this.historyPanel.remove();
            this.historyPanel = undefined;
            this.expandedId = undefined;
        }
    }

    private async showHistory(item: DocumentRow, rowIndex: number) {
        this.hideHistory();
        const resp = await WorkflowService.GetHistory({
            WorkflowKey: 'DocumentWorkflow',
            EntityId: item.DocumentId
        });

        const panel = document.createElement('div');
        panel.classList.add('workflow-history-panel', 'bg-light', 'border');
        panel.style.position = 'absolute';
        panel.style.left = '0';
        panel.style.right = '0';

        const table = document.createElement('table');
        table.classList.add('table', 'table-sm', 'mb-0');
        table.innerHTML =
            '<thead><tr><th>Date</th><th>From State</th><th>To State</th><th>Trigger</th><th>Input</th></tr></thead>';
        const body = document.createElement('tbody');
        for (const h of resp.History ?? []) {
            const tr = document.createElement('tr');
            tr.innerHTML = `<td>${h.EventDate}</td><td>${h.FromState}</td><td>${h.ToState}</td>` +
                `<td>${h.Trigger}</td><td>${this.formatInput(h.Input)}</td>`;
            body.appendChild(tr);
        }
        table.appendChild(body);
        panel.appendChild(table);

        const container = this.slickContainer[0];
        container.appendChild(panel);
        const rowEl = container.querySelector('.slick-row[row="' + rowIndex + '"]') as HTMLElement ??
            container.querySelector('[data-row="' + rowIndex + '"]') as HTMLElement;
        const rect = rowEl?.getBoundingClientRect();
        const contRect = container.getBoundingClientRect();
        const top = rect ? rect.bottom - contRect.top : 0;
        panel.style.top = top + 'px';

        this.historyPanel = panel;
        this.expandedId = item.DocumentId;
    }

    private formatInput(input: any): string {
        if (input == null)
            return '';
        if (typeof input === 'object')
            return Object.entries(input)
                .map(([k, v]) => `<div><strong>${htmlEncode(k)}</strong>: ${htmlEncode(String(v))}</div>`)
                .join('');
        return htmlEncode(String(input));
    }
}
