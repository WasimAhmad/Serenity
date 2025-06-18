import * as React from 'react';
import { WorkflowGraph } from './WorkflowGraph';
import { StateEditor } from './StateEditor';
import { TriggerEditor, WorkflowTrigger, PermissionGrantType as ClientPermissionGrantType } from './TriggerEditor'; // Renamed to avoid conflict
import { TransitionEditor } from './TransitionEditor';
import { Node, Edge, applyNodeChanges, applyEdgeChanges, addEdge } from 'reactflow';

// Import Serenity service and DTOs
import { WorkflowDefinitionManagementService } from '~/Modules/ServerTypes/Workflow/WorkflowDefinitionManagementService';
import {
    ApiWorkflowDefinition,
    ApiWorkflowState,
    ApiWorkflowTrigger,
    ApiWorkflowTransition,
    ApiPermissionGrantType, // Server-side enum
    WorkflowDefinitionSaveRequest,
    WorkflowDefinitionRetrieveRequest,
    WorkflowDefinitionListRequest,
    DeleteRequest, // Added for delete
    WorkflowDefinitionDeleteRequest // Ensured this is imported if specific
} from '~/Modules/ServerTypes/Workflow';
import { serviceCall } from '@serenity-is/corelib'; // For making service calls

// Helper to convert client-side trigger to API trigger
const toApiTrigger = (trigger: WorkflowTrigger): ApiWorkflowTrigger => {
    return {
        Id: trigger.id,
        TriggerKey: trigger.triggerKey,
        DisplayName: trigger.displayName,
        HandlerKey: trigger.handlerKey,
        FormKey: trigger.formKey,
        RequiresInput: trigger.requiresInput,
        PermissionType: trigger.permissionType as number as ApiPermissionGrantType, // Cast needed
        Permissions: trigger.permissions,
        PermissionHandlerKey: trigger.permissionHandlerKey
    };
};

// Helper to convert API trigger to client-side trigger
const fromApiTrigger = (apiTrigger: ApiWorkflowTrigger): WorkflowTrigger => {
    return {
        id: apiTrigger.Id,
        triggerKey: apiTrigger.TriggerKey,
        displayName: apiTrigger.DisplayName,
        handlerKey: apiTrigger.HandlerKey,
        formKey: apiTrigger.FormKey,
        requiresInput: apiTrigger.RequiresInput ?? false,
        permissionType: apiTrigger.PermissionType as number as ClientPermissionGrantType, // Cast needed
        permissions: apiTrigger.Permissions,
        permissionHandlerKey: apiTrigger.PermissionHandlerKey
    };
};


const initialNodes: Node[] = [
    { id: 's1', type: 'default', position: { x: 0, y: 0 }, data: { label: 'Start' } },
    { id: 's2', type: 'default', position: { x: 0, y: 100 }, data: { label: 'End' } },
];
const initialEdges: Edge[] = [{ id: 'e_s1_s2', source: 's1', target: 's2', label: 'Initial Transition', data: { triggerId: '' } }];

let clientNodeIdCounter = 3;
// clientTriggerIdCounter is managed within TriggerEditor.tsx,
// but for loading from backend, IDs will come from there.

export default function WorkflowDesignerPage() {
    const [nodes, setNodes] = React.useState<Node[]>(initialNodes);
    const [edges, setEdges] = React.useState<Edge[]>(initialEdges);
    const [selectedNode, setSelectedNode] = React.useState<Node | null>(null);
    const [selectedEdge, setSelectedEdge] = React.useState<Edge | null>(null);
    const [triggers, setTriggers] = React.useState<WorkflowTrigger[]>([]);

    const [currentDefinitionId, setCurrentDefinitionId] = React.useState<string>("MyFirstWorkflow");
    const [currentDefinitionName, setCurrentDefinitionName] = React.useState<string>("My First Workflow Name");

    const onNodesChange = React.useCallback(
        (changes: any) => setNodes((nds) => applyNodeChanges(changes, nds)),
        [setNodes]
    );
    const onEdgesChange = React.useCallback(
        (changes: any) => setEdges((eds) => applyEdgeChanges(changes, eds)),
        [setEdges]
    );

    const onConnect = React.useCallback(
        (connection: any) => {
            const newEdge = {
                ...connection,
                label: 'New Transition',
                id: `client-edge-${Date.now()}`, // Ensure client-side IDs are distinct if not yet saved
                data: { triggerId: '' }
            };
            setEdges((eds) => addEdge(newEdge, eds));
        },
        [setEdges]
    );

    const addStateNode = React.useCallback(() => {
        const newNodeId = `client-node-${clientNodeIdCounter++}`;
        const newNode: Node = {
            id: newNodeId,
            type: 'default',
            position: { x: Math.random() * 400, y: Math.random() * 400 },
            data: { label: `State ${newNodeId}` },
        };
        setNodes((nds) => nds.concat(newNode));
    }, []);

    const onNodeClick = React.useCallback((event: React.MouseEvent, node: Node) => {
        setSelectedNode(node);
        setSelectedEdge(null);
    }, []);

    const onEdgeClick = React.useCallback((event: React.MouseEvent, edge: Edge) => {
        setSelectedEdge(edge);
        setSelectedNode(null);
    }, []);

    const handleNodeLabelChange = React.useCallback((nodeId: string, newLabel: string) => {
        setNodes((nds) =>
            nds.map((nodeItem) =>
                nodeItem.id === nodeId ? { ...nodeItem, data: { ...nodeItem.data, label: newLabel } } : nodeItem
            )
        );
        setSelectedNode(null);
    }, [setNodes]);

    const handleEdgeLabelChange = React.useCallback((edgeId: string, newLabel: string, triggerId?: string) => {
        setEdges((eds) =>
            eds.map((edge) =>
                edge.id === edgeId ? { ...edge, label: newLabel, data: { ...edge.data, triggerId: triggerId || '' } } : edge
            )
        );
        setSelectedEdge(null);
    }, [setEdges]);

    const closeEditor = React.useCallback(() => {
        setSelectedNode(null);
        setSelectedEdge(null);
    }, []);

    const handleAddTrigger = React.useCallback((trigger: WorkflowTrigger) => {
        setTriggers((prevTriggers) => [...prevTriggers, trigger]);
    }, [setTriggers]);

    const handleUpdateTrigger = React.useCallback((updatedTrigger: WorkflowTrigger) => {
        setTriggers((prevTriggers) =>
            prevTriggers.map((trigger) =>
                trigger.id === updatedTrigger.id ? updatedTrigger : trigger
            )
        );
    }, [setTriggers]);

    const handleDeleteTrigger = React.useCallback((triggerId: string) => {
        setTriggers((prevTriggers) =>
            prevTriggers.filter((trigger) => trigger.id !== triggerId)
        );
    }, [setTriggers]);

    const loadDefinition = React.useCallback(() => {
        if (!currentDefinitionId) {
            alert("Please enter a Definition ID to load.");
            return;
        }
        serviceCall({
            url: WorkflowDefinitionManagementService.Methods.Retrieve,
            request: { DefinitionId: currentDefinitionId } as WorkflowDefinitionRetrieveRequest,
            onSuccess: (response) => {
                console.log("Retrieve Response:", response);
                if (response.Definition) {
                    const def = response.Definition;
                    setCurrentDefinitionName(def.DefinitionName);
                    const loadedNodes: Node[] = (def.States || []).map(s => ({
                        id: s.Id,
                        type: 'default',
                        position: { x: s.X ?? Math.random() * 400, y: s.Y ?? Math.random() * 400 },
                        data: { label: s.DisplayName || s.StateKey }
                    }));
                    setNodes(loadedNodes);

                    const loadedTriggers: WorkflowTrigger[] = (def.Triggers || []).map(fromApiTrigger);
                    setTriggers(loadedTriggers);

                    const loadedEdges: Edge[] = (def.Transitions || []).map(t => {
                        const trigger = loadedTriggers.find(tr => tr.id === t.TriggerId);
                        return {
                            id: t.Id,
                            source: t.FromStateId,
                            target: t.ToStateId,
                            label: trigger?.displayName || trigger?.triggerKey || 'Transition',
                            data: { triggerId: t.TriggerId }
                        };
                    });
                    setEdges(loadedEdges);
                    alert('Workflow ' + currentDefinitionId + ' loaded.');
                } else {
                    alert('Workflow ' + currentDefinitionId + ' not found or empty.');
                    // Reset to initial state if definition is not found or empty
                    setNodes(initialNodes);
                    setEdges(initialEdges);
                    setTriggers([]);
                    setCurrentDefinitionName("New Workflow Name");
                }
            },
            onError: (error) => {
                console.error("Retrieve Error:", error);
                alert('Error loading workflow: ' + (error.message || 'Unknown error'));
            }
        });
    }, [currentDefinitionId]);

    const saveDefinition = React.useCallback(() => {
        if (!currentDefinitionId) {
            alert("Please enter a Definition ID to save.");
            return;
        }

        const apiDefinition: ApiWorkflowDefinition = {
            DefinitionId: currentDefinitionId,
            DefinitionName: currentDefinitionName,
            States: nodes.map(n => ({
                Id: n.id,
                StateKey: n.data.label, // Assuming label is used as StateKey for simplicity
                DisplayName: n.data.label,
                X: n.position.x,
                Y: n.position.y
            })),
            Triggers: triggers.map(toApiTrigger),
            Transitions: edges.map(e => ({
                Id: e.id,
                FromStateId: e.source,
                ToStateId: e.target,
                TriggerId: e.data.triggerId,
                GuardKey: "" // Placeholder for now
            }))
        };

        serviceCall({
            url: WorkflowDefinitionManagementService.Methods.Save,
            request: { Definition: apiDefinition } as WorkflowDefinitionSaveRequest,
            onSuccess: (response) => {
                console.log("Save Response:", response);
                alert('Workflow ' + currentDefinitionId + ' saved successfully.');
            },
            onError: (error) => {
                console.error("Save Error:", error);
                alert('Error saving workflow: ' + (error.message || 'Unknown error'));
            }
        });
    }, [currentDefinitionId, currentDefinitionName, nodes, edges, triggers]);

    const listDefinitions = React.useCallback(() => {
        serviceCall({
            url: WorkflowDefinitionManagementService.Methods.List,
            request: {} as WorkflowDefinitionListRequest,
            onSuccess: (response) => {
                console.log("List Response:", response);
                alert('Found ' + (response.Entities?.length || 0) + ' definitions. Check console for details.');
            },
            onError: (error) => {
                console.error("List Error:", error);
                alert('Error listing workflows: ' + (error.message || 'Unknown error'));
            }
        });
    }, []);

    const deleteDefinition = React.useCallback(() => {
        if (!currentDefinitionId) {
            alert("Please enter a Definition ID to delete.");
            return;
        }
        if (!confirm('Are you sure you want to delete workflow ' + currentDefinitionId + '?')) return;

        serviceCall({
            url: WorkflowDefinitionManagementService.Methods.Delete,
            request: { DefinitionId: currentDefinitionId } as WorkflowDefinitionDeleteRequest,
            onSuccess: (response) => {
                console.log("Delete Response:", response);
                alert('Workflow ' + currentDefinitionId + ' delete request sent.');
                setNodes(initialNodes);
                setEdges(initialEdges);
                setTriggers([]);
                setCurrentDefinitionId("NewWorkflow");
                setCurrentDefinitionName("New Workflow Name");
            },
            onError: (error) => {
                console.error("Delete Error:", error);
                alert('Error deleting workflow: ' + (error.message || 'Unknown error'));
            }
        });
    }, [currentDefinitionId]);


    return (
        <div style={{ padding: '10px' }}>
            <h1>Workflow Designer</h1>
            <div style={{ marginBottom: '10px', padding: '10px', border: '1px solid #ccc' }}>
                <h4>Load/Save Workflow Definition</h4>
                <input
                    type="text"
                    placeholder="Definition ID (e.g., MyWorkflow)"
                    value={currentDefinitionId}
                    onChange={e => setCurrentDefinitionId(e.target.value)}
                    style={{ marginRight: '5px' }}
                />
                <input
                    type="text"
                    placeholder="Definition Name"
                    value={currentDefinitionName}
                    onChange={e => setCurrentDefinitionName(e.target.value)}
                    style={{ marginRight: '5px' }}
                />
                <button onClick={loadDefinition} style={{ marginRight: '5px' }}>Load</button>
                <button onClick={saveDefinition} style={{ marginRight: '5px' }}>Save</button>
                <button onClick={listDefinitions} style={{ marginRight: '5px' }}>List All (Console)</button>
                <button onClick={deleteDefinition} style={{ color: 'red' }}>Delete Current</button>
            </div>

            <button onClick={addStateNode} style={{ marginBottom: '10px' }}>Add State</button>
            <div style={{ position: 'relative', border: '1px solid black', height: '500px' }}> {/* Ensure this div has a defined height */}
                <WorkflowGraph
                    nodes={nodes}
                    edges={edges}
                    onNodesChange={onNodesChange}
                    onEdgesChange={onEdgesChange}
                    onConnect={onConnect}
                    onNodeClick={onNodeClick}
                    onEdgeClick={onEdgeClick}
                />
                <StateEditor
                    selectedNode={selectedNode}
                    onNodeLabelChange={handleNodeLabelChange}
                    onClose={closeEditor}
                />
                <TransitionEditor
                    selectedEdge={selectedEdge}
                    triggers={triggers}
                    onEdgeLabelChange={handleEdgeLabelChange}
                    onClose={closeEditor}
                />
            </div>
            <TriggerEditor
                triggers={triggers}
                onAddTrigger={handleAddTrigger}
                onUpdateTrigger={handleUpdateTrigger}
                onDeleteTrigger={handleDeleteTrigger}
            />
        </div>
    );
}
