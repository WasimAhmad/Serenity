import * as React from 'react';

// Define PermissionGrantType (maps to the C# enum PermissionGrantType)
export enum PermissionGrantType {
    Explicit = 0,
    Hierarchy = 1,
    Handler = 2,
}

// Update the structure of a Trigger
export interface WorkflowTrigger {
    id: string;
    triggerKey: string;
    displayName?: string;
    handlerKey?: string; // For action handler
    formKey?: string;
    requiresInput?: boolean;
    permissionType?: PermissionGrantType;
    permissions?: string; // Comma-separated list for Explicit type
    permissionHandlerKey?: string; // Key for IWorkflowPermissionHandler
}

interface TriggerEditorProps {
    triggers: WorkflowTrigger[];
    onAddTrigger: (trigger: WorkflowTrigger) => void;
    onUpdateTrigger: (trigger: WorkflowTrigger) => void;
    onDeleteTrigger: (triggerId: string) => void;
}

let triggerIdCounter = 1; // This should ideally be managed more robustly if data is persisted

export function TriggerEditor({ triggers, onAddTrigger, onUpdateTrigger, onDeleteTrigger }: TriggerEditorProps) {
    const [editingTrigger, setEditingTrigger] = React.useState<WorkflowTrigger | null>(null);
    const [triggerKey, setTriggerKey] = React.useState('');
    const [displayName, setDisplayName] = React.useState('');
    const [handlerKey, setHandlerKey] = React.useState(''); // For action handler
    const [formKey, setFormKey] = React.useState('');
    const [requiresInput, setRequiresInput] = React.useState(false);
    const [permissionType, setPermissionType] = React.useState<PermissionGrantType>(PermissionGrantType.Explicit);
    const [permissions, setPermissions] = React.useState('');
    const [permissionHandlerKey, setPermissionHandlerKey] = React.useState('');

    const resetForm = () => {
        setEditingTrigger(null);
        setTriggerKey('');
        setDisplayName('');
        setHandlerKey('');
        setFormKey('');
        setRequiresInput(false);
        setPermissionType(PermissionGrantType.Explicit);
        setPermissions('');
        setPermissionHandlerKey('');
    };

    React.useEffect(() => {
        if (editingTrigger) {
            setTriggerKey(editingTrigger.triggerKey);
            setDisplayName(editingTrigger.displayName || '');
            setHandlerKey(editingTrigger.handlerKey || '');
            setFormKey(editingTrigger.formKey || '');
            setRequiresInput(editingTrigger.requiresInput || false);
            setPermissionType(editingTrigger.permissionType ?? PermissionGrantType.Explicit);
            setPermissions(editingTrigger.permissions || '');
            setPermissionHandlerKey(editingTrigger.permissionHandlerKey || '');
        } else {
            resetForm();
        }
    }, [editingTrigger]);

    const handleSaveTrigger = () => {
        if (!triggerKey) {
            alert('Trigger Key is required.');
            return;
        }

        const triggerData: Omit<WorkflowTrigger, 'id'> = {
            triggerKey,
            displayName,
            handlerKey,
            formKey,
            requiresInput,
            permissionType,
            permissions: permissionType === PermissionGrantType.Explicit ? permissions : '',
            permissionHandlerKey: permissionType === PermissionGrantType.Handler ? permissionHandlerKey : '',
        };

        if (editingTrigger) {
            onUpdateTrigger({ ...editingTrigger, ...triggerData });
        } else {
            onAddTrigger({
                id: `trigger-${triggerIdCounter++}`, // Ensure unique ID generation
                ...triggerData,
            });
        }
        resetForm();
    };

    const handleEditTrigger = (trigger: WorkflowTrigger) => {
        setEditingTrigger(trigger);
    };

    const handleCancelEdit = () => {
        resetForm();
    };

    return (
        <div style={{ marginTop: '20px', padding: '10px', border: '1px solid #ddd' }}>
            <h3>Manage Triggers</h3>
            <div style={{ borderBottom: '1px solid #eee', paddingBottom: '10px', marginBottom: '10px' }}>
                <h4>{editingTrigger ? 'Edit Trigger' : 'Add New Trigger'}</h4>
                <div>
                    <input type="text" placeholder="Trigger Key*" value={triggerKey} onChange={(e) => setTriggerKey(e.target.value)} style={{ marginRight: '5px', marginBottom: '5px' }} />
                    <input type="text" placeholder="Display Name" value={displayName} onChange={(e) => setDisplayName(e.target.value)} style={{ marginRight: '5px', marginBottom: '5px' }} />
                </div>
                <div>
                    <input type="text" placeholder="Action Handler Key" value={handlerKey} onChange={(e) => setHandlerKey(e.target.value)} style={{ marginRight: '5px', marginBottom: '5px' }} />
                    <input type="text" placeholder="Form Key" value={formKey} onChange={(e) => setFormKey(e.target.value)} style={{ marginRight: '5px', marginBottom: '5px' }} />
                    <label style={{ marginRight: '5px' }}>
                        <input type="checkbox" checked={requiresInput} onChange={(e) => setRequiresInput(e.target.checked)} />
                        Requires Input
                    </label>
                </div>
                <div style={{ marginTop: '10px' }}>
                    <h5>Trigger Permissions</h5>
                    <select value={permissionType} onChange={(e) => setPermissionType(Number(e.target.value) as PermissionGrantType)} style={{ marginRight: '5px', marginBottom: '5px' }}>
                        <option value={PermissionGrantType.Explicit}>Explicit</option>
                        <option value={PermissionGrantType.Hierarchy}>Hierarchy (Not Yet Implemented)</option>
                        <option value={PermissionGrantType.Handler}>Handler</option>
                    </select>
                    {permissionType === PermissionGrantType.Explicit && (
                        <input
                            type="text"
                            placeholder="Permissions (comma-separated, e.g., user1,role:Admin)"
                            value={permissions}
                            onChange={(e) => setPermissions(e.target.value)}
                            style={{ width: '300px', marginBottom: '5px' }}
                        />
                    )}
                    {permissionType === PermissionGrantType.Handler && (
                        <input
                            type="text"
                            placeholder="Permission Handler Key"
                            value={permissionHandlerKey}
                            onChange={(e) => setPermissionHandlerKey(e.target.value)}
                             style={{ width: '200px', marginBottom: '5px' }}
                        />
                    )}
                </div>
                <button onClick={handleSaveTrigger} style={{ marginRight: '5px', marginTop: '10px' }}>
                    {editingTrigger ? 'Save Changes' : 'Add Trigger'}
                </button>
                {editingTrigger && (
                    <button onClick={handleCancelEdit} style={{ marginTop: '10px' }}>Cancel</button>
                )}
            </div>
            <div style={{ marginTop: '15px' }}>
                <h4>Available Triggers</h4>
                {triggers.length === 0 ? (
                    <p>No triggers defined yet.</p>
                ) : (
                    <ul style={{ listStyleType: 'none', paddingLeft: 0 }}>
                        {triggers.map((trigger) => (
                            <li key={trigger.id} style={{ marginBottom: '10px', paddingBottom: '10px', borderBottom: '1px solid #f0f0f0' }}>
                                <strong>{trigger.displayName || trigger.triggerKey}</strong> ({trigger.triggerKey}) <br />
                                <small>
                                    Action Handler: {trigger.handlerKey || 'N/A'} | Form: {trigger.formKey || 'N/A'} | Requires Input: {trigger.requiresInput ? 'Yes' : 'No'} <br />
                                    Permission Type: {PermissionGrantType[trigger.permissionType ?? PermissionGrantType.Explicit]}
                                    {trigger.permissionType === PermissionGrantType.Explicit && ` | Permissions: ${trigger.permissions || 'N/A'}`}
                                    {trigger.permissionType === PermissionGrantType.Handler && ` | Handler Key: ${trigger.permissionHandlerKey || 'N/A'}`}
                                </small>
                                <button onClick={() => handleEditTrigger(trigger)} style={{ marginLeft: '10px', marginRight: '5px', display: 'block', marginTop: '5px' }}>
                                    Edit
                                </button>
                                <button onClick={() => onDeleteTrigger(trigger.id)} style={{ display: 'block', marginTop: '5px' }}>Delete</button>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    );
}
