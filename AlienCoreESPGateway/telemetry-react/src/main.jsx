import React from 'react';
import { createRoot } from 'react-dom/client';
import Dashboard from './Dashboard.jsx';

window.renderTelemetry = (elementId, dotnetHelper) => {
    const container = document.getElementById(elementId);
    const root = createRoot(container);
    root.render(<Dashboard dotnetHelper={dotnetHelper} />);
};