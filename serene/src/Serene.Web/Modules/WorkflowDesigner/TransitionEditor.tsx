import * as React from 'react';
import { Edge } from 'reactflow';
import { WorkflowTrigger } from './TriggerEditor'; // Import WorkflowTrigger

export interface TransitionEditorProps {
    selectedEdge: Edge | null;
    triggers: WorkflowTrigger[]; // List of available triggers
    onEdgeLabelChange: (edgeId: string, newLabel: string, triggerId?: string) => void;
    onClose: () => void;
}

export function TransitionEditor({ selectedEdge, triggers, onEdgeLabelChange, onClose }: TransitionEditorProps) {
    const [selectedTriggerId, setSelectedTriggerId] = React.useState<string | undefined>('');

    React.useEffect(() => {
        if (selectedEdge) {
            // Assuming edge.data.triggerId stores the ID of the selected trigger
            setSelectedTriggerId(selectedEdge.data?.triggerId || '');
        }
    }, [selectedEdge]);

    if (!selectedEdge) {
        return null;
    }

    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault();
        if (selectedEdge) {
            const selectedTrigger = triggers.find(t => t.id === selectedTriggerId);
            onEdgeLabelChange(selectedEdge.id, selectedTrigger?.displayName || selectedTrigger?.triggerKey || 'Select Trigger', selectedTriggerId);
        }
        onClose();
    };

    return (
        <div style={{
            position: 'absolute',
            top: '150px', // Adjust position as needed
            right: '10px',
            padding: '10px',
            background: 'white',
            border: '1px solid #ccc',
            zIndex: 10 // Ensure it's above the graph
        }}>
            <h3>Edit Transition</h3>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="triggerSelect">Trigger: </label>
                    <select
                        id="triggerSelect"
                        value={selectedTriggerId}
                        onChange={(e) => setSelectedTriggerId(e.target.value)}
                        style={{ minWidth: '150px' }}
                    >
                        <option value="">-- Select Trigger --</option>
                        {triggers.map(trigger => (
                            <option key={trigger.id} value={trigger.id}>
                                {trigger.displayName || trigger.triggerKey}
                            </option>
                        ))}
                    </select>
                </div>
                <button type="submit" style={{ marginTop: '10px' }}>Save</button>
                <button type="button" onClick={onClose} style={{ marginLeft: '5px', marginTop: '10px' }}>
                    Cancel
                </button>
            </form>
        </div>
    );
}
