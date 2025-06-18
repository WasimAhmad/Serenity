import * as React from 'react';
import { Node } from 'reactflow';

export interface StateEditorProps {
    selectedNode: Node | null;
    onNodeLabelChange: (nodeId: string, newLabel: string) => void;
    onClose: () => void;
}

export function StateEditor({ selectedNode, onNodeLabelChange, onClose }: StateEditorProps) {
    const [label, setLabel] = React.useState('');

    React.useEffect(() => {
        if (selectedNode) {
            setLabel(selectedNode.data.label);
        }
    }, [selectedNode]);

    if (!selectedNode) {
        return null;
    }

    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault();
        if (selectedNode) {
            onNodeLabelChange(selectedNode.id, label);
        }
        onClose();
    };

    return (
        <div style={{
            position: 'absolute',
            top: '10px',
            right: '10px',
            padding: '10px',
            background: 'white',
            border: '1px solid #ccc',
            zIndex: 10 // Ensure it's above the graph
        }}>
            <h3>Edit State</h3>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="nodeLabel">Label: </label>
                    <input
                        id="nodeLabel"
                        type="text"
                        value={label}
                        onChange={(e) => setLabel(e.target.value)}
                    />
                </div>
                <button type="submit" style={{ marginTop: '5px' }}>Save</button>
                <button type="button" onClick={onClose} style={{ marginLeft: '5px', marginTop: '5px' }}>
                    Cancel
                </button>
            </form>
        </div>
    );
}
