import { Fluent } from "../../base";
import { Decorators } from "../../types/decorators";
import { Widget, WidgetProps } from "./widget";

export interface StepWizardOptions {
    startStep?: number;
}

@Decorators.registerClass('Serenity.StepWizard')
export class StepWizard<P extends StepWizardOptions = StepWizardOptions> extends Widget<P> {
    protected steps: HTMLElement[] = [];
    protected index = 0;
    protected toolbar: HTMLElement;
    protected prevBtn: HTMLButtonElement;
    protected nextBtn: HTMLButtonElement;
    protected content: HTMLElement;

    protected renderContents() {
        this.domNode.classList.add('s-StepWizard');
        this.steps = Array.from(this.domNode.querySelectorAll<HTMLElement>('.step-pane'));
        this.index = this.options.startStep ?? 0;
        this.content = this.domNode.appendChild(<div class="wizard-content" />);
        this.toolbar = this.domNode.appendChild(<div class="wizard-toolbar btn-group" />);
        this.prevBtn = this.toolbar.appendChild(<button type="button" class="btn btn-secondary">Back</button>);
        this.nextBtn = this.toolbar.appendChild(<button type="button" class="btn btn-primary">Next</button>);
        this.prevBtn.addEventListener('click', () => this.previous());
        this.nextBtn.addEventListener('click', () => this.next());
        this.showStep(this.index);
    }

    protected showStep(i: number) {
        this.content.innerHTML = '';
        if (this.steps[i])
            this.content.appendChild(this.steps[i]);
        this.prevBtn.disabled = i === 0;
        this.nextBtn.textContent = i >= this.steps.length - 1 ? 'Finish' : 'Next';
    }

    next() {
        if (this.index < this.steps.length - 1) {
            this.index++;
            this.showStep(this.index);
        } else {
            Fluent.trigger(this.domNode, 'finish');
        }
    }

    previous() {
        if (this.index > 0) {
            this.index--;
            this.showStep(this.index);
        }
    }
}
