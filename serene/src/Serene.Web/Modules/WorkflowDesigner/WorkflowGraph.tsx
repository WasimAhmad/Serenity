import * as React from 'react';
import ReactFlow, { MiniMap, Controls, Background, Node, Edge, OnNodesChange, OnEdgesChange, OnConnect, NodeMouseHandler, EdgeMouseHandler } from 'reactflow'; // Added EdgeMouseHandler
import 'reactflow/dist/style.css';

export interface WorkflowGraphProps {
    nodes: Node[];
    edges: Edge[];
    onNodesChange: OnNodesChange;
    onEdgesChange: OnEdgesChange;
    onConnect: OnConnect;
    onNodeClick?: NodeMouseHandler;
    onEdgeClick?: EdgeMouseHandler; // Optional: Handler for edge click
}

export function WorkflowGraph({ nodes, edges, onNodesChange, onEdgesChange, onConnect, onNodeClick, onEdgeClick }: WorkflowGraphProps) {
    return (
        <div style={{ width: '100%', height: '500px' }}>
            <ReactFlow
                nodes={nodes}
                edges={edges}
                onNodesChange={onNodesChange}
                onEdgesChange={onEdgesChange}
                onConnect={onConnect}
                onNodeClick={onNodeClick}
                onEdgeClick={onEdgeClick} // Pass to ReactFlow
                fitView
            >
                <Controls />
                <MiniMap />
                <Background gap={12} size={1} />
            </ReactFlow>
        </div>
    );
}
